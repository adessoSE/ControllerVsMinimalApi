namespace Presentation.MinimalApi.Common.net7.EndpointMapper;

using Microsoft.AspNetCore.Routing;

public interface IEndpointMapper
{
    static abstract void MapEndpoints(IEndpointRouteBuilder app);
}
