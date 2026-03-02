namespace Memlane.Api.Providers
{
    public class SqlServerBackupProvider : IBackupProvider
    {
        public string ProviderName => "SQL Server";

        public Task<string> CreateBackupAsync(string connectionString, string targetDirectory)
        {
            // In a real implementation, this would use Microsoft.Data.SqlClient 
            // and execute a BACKUP DATABASE command.
            // For now, we'll simulate the creation of a .bak file.
            
            var databaseName = GetDatabaseName(connectionString);
            var fileName = $"{databaseName}_{DateTime.Now:yyyyMMddHHmmss}.bak";
            var fullPath = Path.Combine(targetDirectory, fileName);

            // Simulate the backup process
            // await connection.ExecuteAsync($"BACKUP DATABASE [{databaseName}] TO DISK = '{fullPath}'");
            
            return Task.FromResult(fullPath);
        }

        private string GetDatabaseName(string connectionString)
        {
            // Simple logic to extract database name from connection string
            // e.g., "Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;"
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
        public string ProviderName => "MariaDB";

        public Task<string> CreateBackupAsync(string connectionString, string targetDirectory)
        {
            // In a real implementation, this would likely involve running 'mysqldump' 
            // via Process.Start or a library.
            
            var databaseName = GetDatabaseName(connectionString);
            var fileName = $"{databaseName}_{DateTime.Now:yyyyMMddHHmmss}.sql";
            var fullPath = Path.Combine(targetDirectory, fileName);

            // Simulate the backup process
            // Process.Start("mysqldump", $"--user={user} --password={pass} {databaseName} > {fullPath}");

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
                if (part.Trim().StartsWith("Server=", StringComparison.OrdinalIgnoreCase))
                {
                    // For some connection strings, the database might be part of the Server parameter or elsewhere
                }
            }
            return "UnknownDB";
        }
    }
}