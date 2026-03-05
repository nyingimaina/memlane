using Cronos;
using Xunit;

namespace Memlane.Tests
{
    public class CronTests
    {
        [Fact]
        public void CronExpression_ShouldParseCorrectly()
        {
            // Arrange
            var cron = "* * * * *"; // Every minute

            // Act
            var expression = CronExpression.Parse(cron);
            var nextOccurrence = expression.GetNextOccurrence(DateTime.UtcNow);

            // Assert
            Assert.NotNull(nextOccurrence);
            Assert.True(nextOccurrence > DateTime.UtcNow);
        }

        [Fact]
        public void CronExpression_Invalid_ShouldThrow()
        {
            // Arrange
            var cron = "invalid-cron";

            // Act & Assert
            Assert.Throws<CronFormatException>(() => CronExpression.Parse(cron));
        }

        [Fact]
        public void CronExpression_NextOccurrence_ShouldBeCorrect()
        {
            // Arrange
            // 0 0 * * * = Midnight every day
            var cron = "0 0 * * *";
            var expression = CronExpression.Parse(cron);
            var now = new DateTime(2026, 3, 4, 12, 0, 0, DateTimeKind.Utc); // Wednesday noon

            // Act
            var next = expression.GetNextOccurrence(now);

            // Assert
            Assert.Equal(new DateTime(2026, 3, 5, 0, 0, 0, DateTimeKind.Utc), next);
        }
    }
}