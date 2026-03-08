using System.Diagnostics;
using Microsoft.Data.SqlClient;
using Memlane.Api.Services;
using System.Text.Json;

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

        public async Task<string> CreateBackupAsync(string connectionString, string targetDirectory, string? optionsJson = null)
        {
            // SQL Server T-SQL backup doesn't need external tools if using SMO or direct T-SQL
            // But if we ever use sqlcmd, we'd check options.SqlToolPath here.
            
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

        public async Task<string> CreateBackupAsync(string connectionString, string targetDirectory, string? optionsJson = null)
        {
            var databaseName = GetDatabaseName(connectionString);
            var fileName = _filenameGenerator.Generate(databaseName, "sql");
            var fullPath = Path.Combine(targetDirectory, fileName);

            var config = !string.IsNullOrEmpty(optionsJson) 
                ? JsonSerializer.Deserialize<BackupJobConfiguration>(optionsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                : null;

            string toolName = ResolveToolPath(config?.SqlToolPath);
            
            _logger.LogInformation("Starting MariaDB backup for {Database} using {Tool} to {Path}", databaseName, toolName, fullPath);

            var builder = new MySqlConnector.MySqlConnectionStringBuilder(connectionString);
            
            var startInfo = new ProcessStartInfo
            {
                FileName = toolName,
                Arguments = $"--host={builder.Server} --port={builder.Port} --user={builder.UserID} --password={builder.Password} {databaseName}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = startInfo };
            using var fileStream = File.Create(fullPath);
            
            try {
                process.Start();

                var copyTask = process.StandardOutput.BaseStream.CopyToAsync(fileStream);
                var errorTask = process.StandardError.ReadToEndAsync();

                await Task.WhenAll(copyTask, process.WaitForExitAsync());

                if (process.ExitCode != 0)
                {
                    var error = await errorTask;
                    _logger.LogError("{Tool} failed with exit code {ExitCode}: {Error}", toolName, process.ExitCode, error);
                    throw new Exception($"{toolName} failed: {error}");
                }

                _logger.LogInformation("MariaDB backup completed successfully.");
                return fullPath;
            } catch (Exception ex) {
                _logger.LogError(ex, "Failed to execute database backup tool: {Tool}", toolName);
                throw;
            }
        }

        private string ResolveToolPath(string? configuredPath)
        {
            // 1. Priority: User-configured path
            if (!string.IsNullOrEmpty(configuredPath))
            {
                if (File.Exists(configuredPath)) return configuredPath;
                
                // If it's a directory, try appending known tool names
                if (Directory.Exists(configuredPath))
                {
                    var mDump = Path.Combine(configuredPath, "mariadb-dump.exe");
                    if (File.Exists(mDump)) return mDump;
                    
                    var myDump = Path.Combine(configuredPath, "mysqldump.exe");
                    if (File.Exists(myDump)) return myDump;
                }
            }

            // 2. Fallback: Search in system PATH
            // Try mariadb-dump first (modern)
            if (CanRunTool("mariadb-dump")) return "mariadb-dump";
            
            // Then mysqldump (legacy/alias)
            return "mysqldump";
        }

        private bool CanRunTool(string name)
        {
            try {
                var startInfo = new ProcessStartInfo {
                    FileName = name,
                    Arguments = "--version",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using var p = Process.Start(startInfo);
                return p != null;
            } catch { return false; }
        }

        private string GetDatabaseName(string connectionString)
        {
            var builder = new MySqlConnector.MySqlConnectionStringBuilder(connectionString);
            return builder.Database;
        }
    }
}
