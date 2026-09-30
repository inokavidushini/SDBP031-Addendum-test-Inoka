
using AgentAssignment.Server.Contracts;
using AgentAssignment.Server.Data;
using AgentAssignment.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace AgentAssignment.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            if (builder.Environment.IsDevelopment())
            {

                var port = builder.Configuration.GetValue<int?>("Server:Port") ?? 5080;

                builder.WebHost.ConfigureKestrel(options =>
                {
                    options.ListenLocalhost(port);
                });
            }
            // Register the concrete Foundry Local service.
            builder.Services.AddSingleton<FoundryLocalChatCompletionService>();

            // Expose the same service through the required interface.
            builder.Services.AddSingleton<IChatCompletionService>(
                sp => sp.GetRequiredService<FoundryLocalChatCompletionService>());
            // Register Entity Framework Core.
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddControllers();

            // OpenAPI
            builder.Services.AddOpenApi();

            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                db.Database.EnsureCreated();

                if (!db.Employees.Any())
                {
                    db.Employees.AddRange(
                        new Employee
                        {
                            Name = "Inoka Gamage",
                            Department = "IT",
                            Email = "Inoka@gmail.com"
                        },
                        new Employee
                        {
                            Name = "Vinod Fer",
                            Department = "Finance",
                            Email = "Vinod@gmail.com"
                        },
                        new Employee
                        {
                            Name = "Nusha Gamage",
                            Department = "HR",
                            Email = "Nushan@gmail.com"
                        }
                    );

                    db.SaveChanges();
                }
            }

            // Get the same singleton instance registered above.
            if (app.Environment.IsDevelopment())
            {
                var chatService =
                app.Services.GetRequiredService<IChatCompletionService>();


                if (chatService is FoundryLocalChatCompletionService foundryService)
                {
                    _ = Task.Run(async () =>
                {
                    try
                    {
                        await foundryService.EnsureStartedAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(
                            $"Foundry Local startup failed: {ex.Message}");
                    }
                });
                }
            }

            // Configure HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseAuthorization();

            app.MapControllers();

            // ------------------------------------------------------------
            // GET /health
            // ------------------------------------------------------------
          // app.MapGet("/health", () =>
          
            // ------------------------------------------------------------
            // POST /v1/chat/completions
            // ------------------------------------------------------------
          //  app.MapPost(
            //    "/v1/chat/completions",
            

            app.Run();
        }
    }
}
