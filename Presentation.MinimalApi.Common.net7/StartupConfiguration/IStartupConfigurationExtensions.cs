namespace Presentation.MinimalApi.Common;

using System;
using System.Reflection;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Presentation.MinimalApi.Common.net7.StartupConfiguration;

public static class IStartupConfigurationExtensions
{
    private static MethodInfo createMethod = typeof(IStartupConfigurationExtensions)
        .GetMethod(nameof(CreateGenericStartupConfigMethods), BindingFlags.NonPublic | BindingFlags.Static)!;

    private static List<Assembly> scanningAssemblies = new();

    public static IServiceCollection AddStartupConfigurations(this IServiceCollection services, IConfiguration configuration, params Assembly[] assemblies)
    {
        if (assemblies.Length == 0)
        {
            assemblies = new[] { Assembly.GetCallingAssembly() };
        }

        scanningAssemblies.AddRange(assemblies);

        var startupConfigurations = GetStartupConfigurations();

        foreach (var startupConfiguration in startupConfigurations)
        {
            startupConfiguration.ConfigureServices(services, configuration);
        }

        return services;
    }

    public static WebApplication UseStartupConfigurations(this WebApplication app)
    {
        var startupConfigurations = GetStartupConfigurations();

        foreach (var startupConfiguration in startupConfigurations)
        {
            startupConfiguration.ConfigureApp(app);
        }

        scanningAssemblies.Clear();

        return app;
    }

    private static IEnumerable<StartupConfigMethods> GetStartupConfigurations()
    {
        var types = scanningAssemblies.SelectMany(a => a.GetTypes());

        return scanningAssemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IStartupConfiguration).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
            .Select(CreateStartupConfigMethods)
            .OrderBy(c => c.Order);
    }

    private static StartupConfigMethods CreateStartupConfigMethods(Type type)
    {
        var genericMethod = createMethod.MakeGenericMethod(type);
        return (StartupConfigMethods)genericMethod.Invoke(null, null)!;
    }

    private static StartupConfigMethods CreateGenericStartupConfigMethods<TStartupConfig>()
        where TStartupConfig : IStartupConfiguration
    {
        return new StartupConfigMethods(
            TStartupConfig.Order, 
            typeof(TStartupConfig).Name, 
            TStartupConfig.ConfigureApp, 
            TStartupConfig.ConfigureServices);
    }

    private record StartupConfigMethods(int Order, string Name, Action<WebApplication> ConfigureApp, Action<IServiceCollection, IConfiguration> ConfigureServices);
}
