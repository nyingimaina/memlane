using Memlane.Api.Infrastructure;
using Xunit;

namespace Memlane.Tests
{
    public class ScaffoldingTests
    {
        [Fact]
        public void Project_ShouldBuild_AndRunTests()
        {
            Assert.True(true);
        }

        [Fact]
        public void DbConnectionFactory_ShouldCreateConnection()
        {
            var factory = new SqliteConnectionFactory("Data Source=:memory:");
            using var connection = factory.CreateConnection();
            Assert.NotNull(connection);
            Assert.Equal("Microsoft.Data.Sqlite.SqliteConnection", connection.GetType().FullName);
        }
    }
}