using FastApiMvc.Model.Data;
using FastApiMvc.Service.Cache;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Microsoft.Extensions.Logging;

namespace FastApiMvc.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public class DatabaseConfiguration
        {
            public string? Provider { get; set; }
            public string? ConnectionString { get; set; }
        }

        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var dbConfig = configuration.GetSection("Database").Get<DatabaseConfiguration>();
            var connectionString = dbConfig?.ConnectionString ?? configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(
                    "Database connection string is not configured. Please check both 'Database:ConnectionString' and 'ConnectionStrings:DefaultConnection' in your appsettings.json");
            }

            var provider = (dbConfig?.Provider ?? "mysql").ToLower();
            var logger = services.BuildServiceProvider().GetService<ILogger<AppDbContext>>();
            logger?.LogInformation("Configuring database with provider: {Provider}", provider);

            services.AddDbContext<AppDbContext>(options =>
            {
                try
                {
                    switch (provider)
                    {
                        case "mysql":
                            var serverVersion = ServerVersion.AutoDetect(connectionString);
                            logger?.LogInformation("Detected MySQL version: {Version}", serverVersion);
                            options.UseMySql(connectionString, serverVersion, mysqlOptions =>
                            {
                                mysqlOptions.EnableRetryOnFailure(
                                    maxRetryCount: 3,
                                    maxRetryDelay: TimeSpan.FromSeconds(5),
                                    errorNumbersToAdd: null);
                            });
                            break;

                        case "sqlserver":
                            options.UseSqlServer(connectionString, sqlOptions =>
                            {
                                sqlOptions.EnableRetryOnFailure(
                                    maxRetryCount: 3,
                                    maxRetryDelay: TimeSpan.FromSeconds(5),
                                    errorNumbersToAdd: null);
                            });
                            break;

                        default:
                            throw new InvalidOperationException(
                                $"Unsupported database provider: {provider}. Supported providers are: mysql, sqlserver");
                    }
                }
                catch (Exception ex)
                {
                    logger?.LogError(ex, "Failed to configure database connection");
                    throw new InvalidOperationException(
                        $"Failed to configure database connection with provider '{provider}'. Please check your connection string and ensure the database server is running.", ex);
                }
            });

            return services;
        }

        public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
        {
            var redisConfig = configuration.GetSection("Redis").Get<RedisConfiguration>();
            var configOptions = ConfigurationOptions.Parse(redisConfig?.ConnectionString ?? "localhost");
            configOptions.AbortOnConnectFail = false;  // 允许在连接失败时继续重试
            
            var multiplexer = ConnectionMultiplexer.Connect(configOptions);
            services.AddSingleton<IConnectionMultiplexer>(multiplexer);
            services.AddScoped<ICacheService, RedisCacheService>();

            return services;
        }

        public static IServiceCollection AddAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            return services;
        }

        public static IServiceCollection AddValidation(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(typeof(Program).Assembly);
            return services;
        }
    }

    public class RedisConfiguration
    {
        public string? ConnectionString { get; set; }
    }
}
