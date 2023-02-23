namespace Presentation.MinimalApi.net7.CleanArchitecture.Startup;

using Presentation.MinimalApi.Common.net7.StartupConfiguration;

public class CorsStartup : IStartupConfiguration
{
    public static int Order => 0;

    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder.AllowCredentials();
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
                builder.SetIsOriginAllowed(origin => true);
            });
        });
    }

    public static void ConfigureApp(WebApplication app)
    {
        app.UseCors();
    }
}
