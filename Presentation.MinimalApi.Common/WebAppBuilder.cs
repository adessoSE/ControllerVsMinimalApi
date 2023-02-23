namespace Presentation.MinimalApi.Common;

using Application;
using Application.Todos;

using Infrastructure;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Presentation.MinimalApi.Common.Decorator.Caching;
using Presentation.MinimalApi.Common.Decorator.PerformanceLogging;
using Presentation.MinimalApi.Common.Swagger;

public static class WebAppBuilder
{
    public static WebApplication BuildApp(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.ConfigureLogging();

        // Add services to the container.
        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(builder.Configuration);
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(config =>
        {
            config.ParameterFilter<GuidParameterFilter>();
        });
              
        builder.Services.Decorate<ITodoRepository, CachedTodoRespository>().AddMemoryCache();
        builder.Services.Decorate<ITodoRepository, PerformanceLoggedTodoRepository>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        return app;
    }
}
