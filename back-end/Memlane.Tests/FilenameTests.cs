using Memlane.Api.Services;
using Xunit;

namespace Memlane.Tests
{
    public class FilenameTests
    {
        [Fact]
        public void SortableFilenameGenerator_ShouldGenerateCorrectPattern()
        {
            var generator = new SortableFilenameGenerator();
            var source = "My Database";
            var extension = "bak";

            var result = generator.Generate(source, extension);

            // Pattern: {Source}_Full_{Timestamp}_{ShortHash}.{extension}
            Assert.StartsWith("My_Database_Full_", result);
            Assert.EndsWith(".bak", result);
            Assert.Contains("_", result);
            
            // Verify timestamp length (yyyyMMdd_HHmmss = 15 chars)
            var parts = result.Split('_');
            Assert.Equal(6, parts.Length); // My, Database, Full, Timestamp, (ShortHash.bak split logic)
        }

        [Fact]
        public void SortableFilenameGenerator_ShouldSanitizeSource()
        {
            var generator = new SortableFilenameGenerator();
            var source = "Data/Base:Test*";
            var extension = ".zip";

            var result = generator.Generate(source, extension);

            Assert.DoesNotContain("/", result);
            Assert.DoesNotContain(":", result);
            Assert.DoesNotContain("*", result);
            Assert.StartsWith("DataBaseTest_Full_", result);
        }

        [Fact]
        public void SortableFilenameGenerator_ShouldBeUnique()
        {
            var generator = new SortableFilenameGenerator();
            var source = "Test";
            
            var name1 = generator.Generate(source, "bak");
            Thread.Sleep(1100); // Ensure timestamp changes
            var name2 = generator.Generate(source, "bak");

            Assert.NotEqual(name1, name2);
        }
    }
}