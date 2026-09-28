
using AgentAssignment.Server.Contracts;

namespace AgentAssignment.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var port = builder.Configuration.GetValue<int?>("Server:Port") ?? 5080;

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenLocalhost(port);
            });

            // Register the concrete Foundry Local service.
            builder.Services.AddSingleton<FoundryLocalChatCompletionService>();

            // Expose the same service through the required interface.
            builder.Services.AddSingleton<IChatCompletionService>(
                sp => sp.GetRequiredService<FoundryLocalChatCompletionService>());

            builder.Services.AddControllers();

            // OpenAPI
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Get the same singleton instance registered above.
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
