namespace Infrastructure;

using Application.Todos;

using Infrastructure.EntityFramework;
using Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Serilog;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        bool useDatabase = configuration.GetValue("UseDatabase", false);

        Log.Information("UseDatabase: {useDatabase}", useDatabase);

        if (useDatabase)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("SqlDb"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName))
                .EnableSensitiveDataLogging(false);
            });

            services.AddScoped<ITodoRepository, TodoEfCoreRepository>();
        }
        else
        {
            services.AddSingleton<ITodoRepository, TodoInMemoryRepository>();
        }

        return services;
    }
}
