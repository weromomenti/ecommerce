
using Serilog;

namespace ApiGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddReverseProxy()
                .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

            builder.Services.AddControllers();

            builder.Services.AddHealthChecks();

            builder.Host.UseSerilog((context, configuration) =>
                configuration.ReadFrom.Configuration(context.Configuration));

            builder.Host.UseSerilog();

            var app = builder.Build();

            app.MapReverseProxy();

            app.MapHealthChecks("/health");

            app.Run();
        }
    }
}
