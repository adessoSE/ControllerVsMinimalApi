namespace Presentation.MinimalApi.Common;

using System.Linq;
using System.Reflection;

using Microsoft.AspNetCore.Routing;

using Presentation.MinimalApi.Common.net7.EndpointMapper;

public static class IEndpointRouteBuilderExtensions
{
    private static MethodInfo mapMethod = typeof(IEndpointRouteBuilderExtensions)
        .GetMethod(nameof(Map), BindingFlags.NonPublic | BindingFlags.Static)!;

    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder builder, params Assembly[] assemblies)
    {
        if (assemblies.Length == 0)
        {
            assemblies = new[] { Assembly.GetCallingAssembly() };
        }

        // Get all types of the assemblies that can be assigned to the IEndpointMapper
        var types = assemblies.SelectMany(a => a.GetTypes());

        var mapperTypes = assemblies
            .SelectMany(a => a.GetTypes())            
            .Where(t => typeof(IEndpointMapper).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);

        foreach (var mapperType in mapperTypes)
        {
            // Execute the Map<TMapper> Method with the current mapper type
            var genericMethod = mapMethod.MakeGenericMethod(mapperType);
            genericMethod.Invoke(null, new object[] { builder });
        }

        return builder;
    }

    private static void Map<TMapper>(IEndpointRouteBuilder builder)
        where TMapper : IEndpointMapper
    {
        TMapper.MapEndpoints(builder);
    }
}
