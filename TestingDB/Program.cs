using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using dtc.Infrastructure;
using dtc.Application;
using dtc.Infrastructure.Pesistence.SQLServer;
using Microsoft.EntityFrameworkCore;
using dtc.Application.Interfaces;
using dtc.Application.DTOs.Auth;
using MongoDB.Driver;
using dtc.Infrastructure.Persistence.MongoDB;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("========================================");
        Console.WriteLine("Testing User Registration (DEV-11)");
        Console.WriteLine("========================================");

        try
        {
            // 1. Build Configuration pointing to dtc.API's appsettings.json
            var basePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\..\\dtc.API"));
            
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            string sqlConnectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
            string mongoConnectionString = configuration.GetConnectionString("MongoDbConnection") ?? "";

            Console.WriteLine($"[Config] SQL Connection String loaded length: {sqlConnectionString.Length}");
            
            // 2. Setup Dependency Injection
            var services = new ServiceCollection();
            
            // SQL DB Context
            services.AddDbContext<SQLDBContext>(options =>
                options.UseSqlServer(sqlConnectionString));
            
            // Mongo DB Context
            services.AddSingleton<MongoDBContext>(sp => {
                var config = new ConfigurationBuilder().AddInMemoryCollection(new[] {
                    new System.Collections.Generic.KeyValuePair<string, string>("ConnectionStrings:MongoDbConnection", mongoConnectionString)
                }).Build();
                return new MongoDBContext(config);
            });

            // Repositories and Services
            services.AddInfrastructureServices();
            services.AddApplicationServices();

            var serviceProvider = services.BuildServiceProvider();

            // 3. Test Registration Logic
            Console.WriteLine("[Process] Resolving IAuthService...");
            var authService = serviceProvider.GetRequiredService<IAuthService>();
            
            var testEmail = $"testuser_{DateTime.Now.Ticks}@example.com";
            
            var request = new RegisterRequestDto 
            {
                Email = testEmail,
                FullName = "System Test User",
                Password = "SuperSecretPassword123!",
                Phone = "0987654321"
            };

            Console.WriteLine($"[Process] Calling RegisterAsync for Email: {request.Email}");
            var response = await authService.RegisterAsync(request);

            Console.WriteLine("========================================");
            Console.WriteLine("✅ REGISTRATION SUCCESSFUL!");
            Console.WriteLine($"New User ID: {response.UserId}");
            Console.WriteLine($"Email: {response.Email}");
            Console.WriteLine($"FullName: {response.FullName}");
            Console.WriteLine("========================================");

        }
        catch (Exception ex)
        {
            Console.WriteLine("========================================");
            Console.WriteLine("❌ REGISTRATION FAILED!");
            Console.WriteLine($"Error: {ex.Message}");
            if (ex.InnerException != null)
                Console.WriteLine($"Inner Error: {ex.InnerException.Message}");
            Console.WriteLine("========================================");
        }
    }
}
