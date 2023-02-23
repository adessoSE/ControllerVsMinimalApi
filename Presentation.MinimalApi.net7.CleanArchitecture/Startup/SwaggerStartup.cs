namespace Presentation.MinimalApi.net7.CleanArchitecture.Startup;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

using Presentation.MinimalApi.Common.net7.StartupConfiguration;
using Presentation.MinimalApi.Common.Swagger;

using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

public class SwaggerStartup : IStartupConfiguration
{
    public static int Order => 500;

    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(config =>
        {
            config.SwaggerDoc(
                    "v1",
                    new OpenApiInfo
                    {
                        Version = "v1",
                        Title = "Minimal Todo API",
                        Description = "Todo API Swagger Surface",
                        Contact = new OpenApiContact
                        {
                            Name = "Marcus Kaseder",
                            Email = "marcus.kaseder@adesso.de",
                            Url = new Uri("https://entwickler.de/reader/my-events/basta-spring-2023-001/microservices-apis/639047b3d088ee001fe3e55c"),
                            Extensions = new Dictionary<string, IOpenApiExtension>
                            {
                                { "x-github", new OpenApiString("https://github.com/MarcusKaseder")},
                                { "x-linkedin", new OpenApiString("https://www.linkedin.com/in/marcus-kaseder-903394263/")},
                                { "x-xing", new OpenApiString("https://www.xing.com/profile/Marcus_Kaseder/cv")},
                                { "x-stackoverflow", new OpenApiString("https://stackoverflow.com/users/4549486/marcus-kaseder")}
                            }
                        },                       
                    });

            var jwtSecurityScheme = new OpenApiSecurityScheme
            {
                Scheme = "bearer",
                BearerFormat = "JWT",
                Name = "JWT Authentication",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Description = "Put **_ONLY_** your JWT Bearer token on textbox below! (Example: eyJhbGciOiJIUz)",

                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };

            config.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

            config.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        { jwtSecurityScheme, Array.Empty<string>() }
                    });

            // Mark non-nullable reference types as non-nullable and required 
            config.SupportNonNullableReferenceTypes();
            config.UseAllOfToExtendReferenceSchemas();
            config.SchemaFilter<RequireNonNullablePropertiesSchemaFilter>();

            config.OperationFilter<ApiKeyHeaderFilter>();
            config.ParameterFilter<GuidParameterFilter>();
        });
    }

    public static void ConfigureApp(WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
    }

    private sealed class RequireNonNullablePropertiesSchemaFilter : ISchemaFilter
    {
        /// <summary>
        /// Add to model.Required all properties where Nullable is false.
        /// </summary>
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            var additionalRequiredProps = schema.Properties
                .Where(x => !x.Value.Nullable && !schema.Required.Contains(x.Key))
                .Select(x => x.Key);
            foreach (var propKey in additionalRequiredProps)
            {
                schema.Required.Add(propKey);
            }
        }
    }

    /// <summary>
    /// Add an (optional) API-Key-Header to every endpoint in Swagger docu
    /// </summary>
    private sealed class ApiKeyHeaderFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters ??= new List<OpenApiParameter>();
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "X-API-Key",
                In = ParameterLocation.Header,
                Description = "Optional API Key for calls between services without user token",
                Required = false,
                Schema = new OpenApiSchema
                {
                    Type = "string"
                }
            });
        }
    }
}
