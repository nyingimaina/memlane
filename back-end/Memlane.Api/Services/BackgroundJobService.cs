using Memlane.Api.Infrastructure;
using Memlane.Api.Models;
using Polly;
using Cronos;

namespace Memlane.Api.Services
{
    public class BackgroundJobService : BackgroundService
    {
        private readonly ILogger<BackgroundJobService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public BackgroundJobService(ILogger<BackgroundJobService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Background Job Service starting...");

            using (var scope = _serviceProvider.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<IJobRepository>();
                await repository.InitializeAsync();
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var repository = scope.ServiceProvider.GetRequiredService<IJobRepository>();
                        
                        // 1. Check for manual "Pending" jobs
                        var pendingJobs = await repository.GetPendingJobsAsync();
                        foreach (var job in pendingJobs)
                        {
                            await ProcessJobAsync(job, repository, stoppingToken);
                        }

                        // 2. Check for "Scheduled" jobs that are due
                        var scheduledJobs = await repository.GetScheduledJobsToRunAsync();
                        foreach (var job in scheduledJobs)
                        {
                            await ProcessJobAsync(job, repository, stoppingToken);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while polling for jobs.");
                }

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }

            _logger.LogInformation("Background Job Service stopping...");
        }

        private async Task ProcessJobAsync(JobMetadata job, IJobRepository repository, CancellationToken stoppingToken)
        {
            _logger.LogInformation("Processing job {JobId}: {JobName}", job.Id, job.Name);

            await repository.UpdateJobStatusAsync(job.Id, JobStatus.InProgress);

            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retry, context) =>
                    {
                        _logger.LogWarning(exception, "Retry {Retry} for job {JobId} after {TimeSpan}ms", retry, job.Id, timeSpan.TotalMilliseconds);
                    });

            try
            {
                JobExecutionResult result = JobExecutionResult.Failed;
                await retryPolicy.ExecuteAsync(async () =>
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var orchestrator = scope.ServiceProvider.GetRequiredService<IJobOrchestrator>();
                        result = await orchestrator.ExecuteJobAsync(job, stoppingToken);
                    }
                });

                var finalStatus = result == JobExecutionResult.Skipped ? JobStatus.Skipped : JobStatus.Completed;
                await repository.UpdateJobStatusAsync(job.Id, finalStatus);
                
                // If it's a scheduled job, calculate the next run time
                if (!string.IsNullOrEmpty(job.CronExpression))
                {
                    try {
                        var cron = CronExpression.Parse(job.CronExpression);
                        var nextUtc = cron.GetNextOccurrence(DateTime.UtcNow);
                        await repository.UpdateNextRunTimeAsync(job.Id, nextUtc);
                        _logger.LogInformation("Job {JobId} scheduled for next run at {NextRun}", job.Id, nextUtc);
                    } catch (Exception ex) {
                        _logger.LogError(ex, "Failed to parse cron expression for job {JobId}", job.Id);
                    }
                }

                _logger.LogInformation("Job {JobId} finished with result: {Result}", job.Id, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Job {JobId} failed after retries.", job.Id);
                await repository.UpdateJobStatusAsync(job.Id, JobStatus.Failed, ex.Message);
                
                // Still schedule next run if it's a cron job
                if (!string.IsNullOrEmpty(job.CronExpression))
                {
                    var cron = CronExpression.Parse(job.CronExpression);
                    var nextUtc = cron.GetNextOccurrence(DateTime.UtcNow);
                    await repository.UpdateNextRunTimeAsync(job.Id, nextUtc);
                }
            }
        }
    }
}