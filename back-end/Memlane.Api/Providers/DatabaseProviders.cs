using Memlane.Api.Services;

namespace Memlane.Api.Providers
{
    public class SqlServerBackupProvider : IBackupProvider
    {
        private readonly IFilenameGenerator _filenameGenerator;
        public string ProviderName => "SQL Server";

        public SqlServerBackupProvider(IFilenameGenerator filenameGenerator)
        {
            _filenameGenerator = filenameGenerator;
        }

        public Task<string> CreateBackupAsync(string connectionString, string targetDirectory)
        {
            var databaseName = GetDatabaseName(connectionString);
            var fileName = _filenameGenerator.Generate(databaseName, "bak");
            var fullPath = Path.Combine(targetDirectory, fileName);
            
            return Task.FromResult(fullPath);
        }

        private string GetDatabaseName(string connectionString)
        {
            var parts = connectionString.Split(';');
            foreach (var part in parts)
            {
                if (part.Trim().StartsWith("Database=", StringComparison.OrdinalIgnoreCase))
                {
                    return part.Split('=')[1].Trim();
                }
                if (part.Trim().StartsWith("Initial Catalog=", StringComparison.OrdinalIgnoreCase))
                {
                    return part.Split('=')[1].Trim();
                }
            }
            return "UnknownDB";
        }
    }

    public class MariaDbBackupProvider : IBackupProvider
    {
        private readonly IFilenameGenerator _filenameGenerator;
        public string ProviderName => "MariaDB";

        public MariaDbBackupProvider(IFilenameGenerator filenameGenerator)
        {
            _filenameGenerator = filenameGenerator;
        }

        public Task<string> CreateBackupAsync(string connectionString, string targetDirectory)
        {
            var databaseName = GetDatabaseName(connectionString);
            var fileName = _filenameGenerator.Generate(databaseName, "sql");
            var fullPath = Path.Combine(targetDirectory, fileName);

            return Task.FromResult(fullPath);
        }

        private string GetDatabaseName(string connectionString)
        {
            var parts = connectionString.Split(';');
            foreach (var part in parts)
            {
                if (part.Trim().StartsWith("Database=", StringComparison.OrdinalIgnoreCase))
                {
                    return part.Split('=')[1].Trim();
                }
            }
            return "UnknownDB";
        }
    }
}