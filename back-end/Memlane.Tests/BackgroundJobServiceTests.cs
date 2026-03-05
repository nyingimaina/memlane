using Memlane.Api.Infrastructure;
using Memlane.Api.Models;
using Memlane.Api.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Memlane.Tests
{
    public class BackgroundJobServiceTests
    {
        private readonly Mock<ILogger<BackgroundJobService>> _mockLogger;
        private readonly Mock<IServiceProvider> _mockServiceProvider;
        private readonly Mock<IServiceScopeFactory> _mockScopeFactory;
        private readonly Mock<IServiceScope> _mockScope;
        private readonly Mock<IJobRepository> _mockRepository;
        private readonly Mock<IJobOrchestrator> _mockOrchestrator;

        public BackgroundJobServiceTests()
        {
            _mockLogger = new Mock<ILogger<BackgroundJobService>>();
            _mockServiceProvider = new Mock<IServiceProvider>();
            _mockScopeFactory = new Mock<IServiceScopeFactory>();
            _mockScope = new Mock<IServiceScope>();
            _mockRepository = new Mock<IJobRepository>();
            _mockOrchestrator = new Mock<IJobOrchestrator>();

            _mockServiceProvider.Setup(x => x.GetService(typeof(IServiceScopeFactory))).Returns(_mockScopeFactory.Object);
            _mockScopeFactory.Setup(x => x.CreateScope()).Returns(_mockScope.Object);
            _mockScope.Setup(x => x.ServiceProvider).Returns(_mockServiceProvider.Object);
            _mockServiceProvider.Setup(x => x.GetService(typeof(IJobRepository))).Returns(_mockRepository.Object);
            _mockServiceProvider.Setup(x => x.GetService(typeof(IJobOrchestrator))).Returns(_mockOrchestrator.Object);
        }

        [Fact]
        public async Task ProcessJobAsync_ShouldCalculateNextRunTime_ForCronJobs()
        {
            // Arrange
            var service = new BackgroundJobService(_mockLogger.Object, _mockServiceProvider.Object);
            var job = new JobMetadata
            {
                Id = 1,
                Name = "Cron Job",
                CronExpression = "0 0 * * *" // Daily at midnight
            };

            _mockOrchestrator.Setup(x => x.ExecuteJobAsync(It.IsAny<JobMetadata>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(JobExecutionResult.Completed);

            // Act
            await service.ProcessJobAsync(job, _mockRepository.Object, CancellationToken.None);

            // Assert
            _mockOrchestrator.Verify(x => x.ExecuteJobAsync(It.Is<JobMetadata>(j => j.Id == 1), It.IsAny<CancellationToken>()), Times.Once);
            _mockRepository.Verify(x => x.UpdateNextRunTimeAsync(It.IsAny<int>(), It.Is<DateTime?>(dt => dt != null && dt > DateTime.UtcNow)), Times.Once);
        }
    }
}