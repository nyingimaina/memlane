using Memlane.Api.Infrastructure;
using Memlane.Api.Models;
using Polly;

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
                        var pendingJobs = await repository.GetPendingJobsAsync();

                        foreach (var job in pendingJobs)
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
                await retryPolicy.ExecuteAsync(async () =>
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var orchestrator = scope.ServiceProvider.GetRequiredService<IJobOrchestrator>();
                        await orchestrator.ExecuteJobAsync(job, stoppingToken);
                    }
                });

                await repository.UpdateJobStatusAsync(job.Id, JobStatus.Completed);
                _logger.LogInformation("Job {JobId} completed successfully.", job.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Job {JobId} failed after retries.", job.Id);
                await repository.UpdateJobStatusAsync(job.Id, JobStatus.Failed, ex.Message);
            }
        }
    }
}