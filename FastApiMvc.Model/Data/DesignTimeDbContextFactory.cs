using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.Extensions.Configuration.Json;
using System.Data.Common;

namespace FastApiMvc.Model.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public class DatabaseConfiguration
        {
            public string? Provider { get; set; }
            public string? ConnectionString { get; set; }
        }

        public AppDbContext CreateDbContext(string[] args)
        {
            try
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.Development.json", optional: true)
                    .Build();

                var builder = new DbContextOptionsBuilder<AppDbContext>();
                var dbConfig = configuration.GetSection("Database").Get<DatabaseConfiguration>();
                var connectionString = dbConfig?.ConnectionString ?? configuration.GetConnectionString("DefaultConnection");

                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException(
                        "Connection string is not configured. Please check both 'Database:ConnectionString' and 'ConnectionStrings:DefaultConnection' in your appsettings.json");
                }

                var provider = (dbConfig?.Provider ?? "mysql").ToLower();
                Console.WriteLine($"Using database provider: {provider}");
                Console.WriteLine($"Connection string: {connectionString}");

                try
                {
                    switch (provider)
                    {
                        case "mysql":
                            var serverVersion = ServerVersion.AutoDetect(connectionString);
                            Console.WriteLine($"Detected MySQL version: {serverVersion}");
                            builder.UseMySql(connectionString, serverVersion, options =>
                            {
                                options.EnableRetryOnFailure(
                                    maxRetryCount: 3,
                                    maxRetryDelay: TimeSpan.FromSeconds(5),
                                    errorNumbersToAdd: null);
                            });
                            break;
                        case "sqlserver":
                            builder.UseSqlServer(connectionString, options =>
                            {
                                options.EnableRetryOnFailure(
                                    maxRetryCount: 3,
                                    maxRetryDelay: TimeSpan.FromSeconds(5),
                                    errorNumbersToAdd: null);
                            });
                            break;
                        default:
                            throw new InvalidOperationException(
                                $"Unsupported database provider: {provider}. Supported providers are: mysql, sqlserver");
                    }

                    return new AppDbContext(builder.Options);
                }
                catch (DbException ex)
                {
                    throw new InvalidOperationException(
                        $"Failed to connect to the database using provider '{provider}'. Please check your connection string and ensure the database server is running.", ex);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating DbContext: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }
    }
}
