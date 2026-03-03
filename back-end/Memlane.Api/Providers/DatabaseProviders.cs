using System.Diagnostics;
using Microsoft.Data.SqlClient;
using Memlane.Api.Services;

namespace Memlane.Api.Providers
{
    public class SqlServerBackupProvider : IBackupProvider
    {
        private readonly IFilenameGenerator _filenameGenerator;
        private readonly ILogger<SqlServerBackupProvider> _logger;

        public SqlServerBackupProvider(IFilenameGenerator filenameGenerator, ILogger<SqlServerBackupProvider> logger)
        {
            _filenameGenerator = filenameGenerator;
            _logger = logger;
        }

        public string ProviderName => "SQL Server";

        public async Task<string> CreateBackupAsync(string connectionString, string targetDirectory)
        {
            var databaseName = GetDatabaseName(connectionString);
            var fileName = _filenameGenerator.Generate(databaseName, "bak");
            var fullPath = Path.Combine(targetDirectory, fileName);

            _logger.LogInformation("Starting SQL Server backup for {Database} to {Path}", databaseName, fullPath);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = $"BACKUP DATABASE [{databaseName}] TO DISK = @path WITH FORMAT, MEDIANAME = 'MemlaneBackup', NAME = 'Full Backup of {databaseName}'";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@path", fullPath);
            command.CommandTimeout = 300; // 5 minutes

            await command.ExecuteNonQueryAsync();

            _logger.LogInformation("SQL Server backup completed successfully.");
            return fullPath;
        }

        private string GetDatabaseName(string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            return builder.InitialCatalog;
        }
    }

    public class MariaDbBackupProvider : IBackupProvider
    {
        private readonly IFilenameGenerator _filenameGenerator;
        private readonly ILogger<MariaDbBackupProvider> _logger;

        public MariaDbBackupProvider(IFilenameGenerator filenameGenerator, ILogger<MariaDbBackupProvider> logger)
        {
            _filenameGenerator = filenameGenerator;
            _logger = logger;
        }

        public string ProviderName => "MariaDB";

        public async Task<string> CreateBackupAsync(string connectionString, string targetDirectory)
        {
            // Note: This requires 'mysqldump' to be in the system PATH
            var databaseName = GetDatabaseName(connectionString);
            var fileName = _filenameGenerator.Generate(databaseName, "sql");
            var fullPath = Path.Combine(targetDirectory, fileName);

            _logger.LogInformation("Starting MariaDB backup for {Database} using mysqldump to {Path}", databaseName, fullPath);

            var builder = new MySqlConnector.MySqlConnectionStringBuilder(connectionString);
            
            var startInfo = new ProcessStartInfo
            {
                FileName = "mysqldump",
                Arguments = $"--host={builder.Server} --port={builder.Port} --user={builder.UserID} --password={builder.Password} {databaseName}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = startInfo };
            using var fileStream = File.Create(fullPath);
            
            process.Start();

            var copyTask = process.StandardOutput.BaseStream.CopyToAsync(fileStream);
            var errorTask = process.StandardError.ReadToEndAsync();

            await Task.WhenAll(copyTask, process.WaitForExitAsync());

            if (process.ExitCode != 0)
            {
                var error = await errorTask;
                _logger.LogError("mysqldump failed with exit code {ExitCode}: {Error}", process.ExitCode, error);
                throw new Exception($"mysqldump failed: {error}");
            }

            _logger.LogInformation("MariaDB backup completed successfully.");
            return fullPath;
        }

        private string GetDatabaseName(string connectionString)
        {
            var builder = new MySqlConnector.MySqlConnectionStringBuilder(connectionString);
            return builder.Database;
        }
    }
}