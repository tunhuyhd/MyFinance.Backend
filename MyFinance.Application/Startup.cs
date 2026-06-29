using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MyFinance.Application.Common.Behaviors;

namespace MyFinance.Application;

public static class Startup
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Startup).Assembly));
        services.AddValidatorsFromAssembly(typeof(Startup).Assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}
