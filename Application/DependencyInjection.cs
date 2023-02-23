namespace Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Application.Mediator;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static Assembly Assembly { get; } = typeof(DependencyInjection).Assembly;

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(DependencyInjection.Assembly);
        services.AddValidatorsFromAssembly(DependencyInjection.Assembly, includeInternalTypes: true);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnexpectedExceptionPipelineBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));

        return services;
    }
}
