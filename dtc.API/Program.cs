
using dtc.Infrastructure;
using dtc.Infrastructure.Pesistence.SQLServer;
using dtc.Infrastructure.Persistence.MongoDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using dtc.Application;
using dtc.API.Middlewares;
using dtc.API.Filters;
using Serilog;

namespace dtc.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                var builder = WebApplication.CreateBuilder(args);
                
                builder.Host.UseSerilog();

            // Add services to the container.
            builder.Services.AddDbContext<SQLDBContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions => sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null)));

            // Register MongoDBContext
            builder.Services.AddSingleton<MongoDBContext>();
            builder.Services.AddInfrastructureServices(builder.Configuration);
            builder.Services.AddApplicationServices();

            // Distributed Caching - use Redis in production, InMemory for local/fallback
            var redisConnStr = builder.Configuration.GetConnectionString("RedisConnection");
            if (!string.IsNullOrEmpty(redisConnStr))
            {
                builder.Services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = redisConnStr;
                    options.InstanceName = "DTC_";
                });
            }
            else
            {
                // Fallback to in-memory cache for local development without Redis
                builder.Services.AddDistributedMemoryCache();
            }

            // Register IdempotencyFilter
            builder.Services.AddScoped<IdempotencyFilter>();


            builder.Services.AddHealthChecks()
                .AddSqlServer(
                    connectionString: builder.Configuration.GetConnectionString("DefaultConnection") ?? "",
                    name: "sqlserver")
                .AddMongoDb(
                    clientFactory: sp =>
                    {
                        var connectionString = builder.Configuration.GetConnectionString("MongoDbConnection");
                        return new MongoClient(connectionString);
                    },
                    databaseNameFactory: sp => "dtcproject",
                    name: "mongodb");

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Configure CORS — allow Next.js frontend
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowNextJs", policy =>
                {
                    policy.WithOrigins(
                              "http://localhost:3000",
                              "https://localhost:3000")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            // Configure Clerk Authentication (via DependencyInjection)
            // Legacy JWT configuration removed to avoid conflicts.

            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var mongoContext = scope.ServiceProvider.GetRequiredService<MongoDBContext>();
                mongoContext.SeedSampleDataAsync().GetAwaiter().GetResult();
            }

            // Configure the HTTP request pipeline.
            app.UseMiddleware<GlobalExceptionMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Apply CORS before Auth — ORDER MATTERS
            app.UseCors("AllowNextJs");
            app.MapHealthChecks("/health");

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
