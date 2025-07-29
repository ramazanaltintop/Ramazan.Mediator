using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture_2025.Application.Abstractions.Messaging;

public static class MediatorExtensions
{
    public static IServiceCollection AddMediator(
        this IServiceCollection services,
        Action<MediatorOptions> options)
    {
        if (options is null) throw new ArgumentNullException(nameof(options));

        var config = new MediatorOptions();
        options(config);

        foreach (var assembly in config.Assemblies)
        {
            var concreteTypes = assembly.GetTypes()
            .Where(t => !t.IsInterface && !t.IsAbstract);

            var serviceRegistrations = concreteTypes
                .SelectMany(t => t.GetInterfaces()
                    .Where(i => i.IsGenericType && (
                        i.GetGenericTypeDefinition() == typeof(ICommandHandler<>) ||
                        i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>) ||
                        i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)))
                    .Select(s => new { Interface = s, Implementation = t }));

            services.AddTransient<ISender, Sender>();

            foreach (var registration in serviceRegistrations)
            {
                services.AddTransient(registration.Interface, registration.Implementation);
            }
        }

        foreach (var pipeline in config.PipelineBehaviors)
        {
            var genericArgsLength = pipeline.GetGenericArguments().Length;

            if (genericArgsLength == 1)
            {
                services.AddTransient(typeof(IPipelineBehavior<>), pipeline);
            }
            else if (genericArgsLength == 2)
            {
                services.AddTransient(typeof(IPipelineBehavior<,>), pipeline);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(genericArgsLength));
            }
        }

        foreach (var assembly in config.Assemblies)
        {
            var concreteTypes = assembly.GetTypes()
                .Where(t => !t.IsInterface && !t.IsAbstract);

            var serviceRegistrations = concreteTypes
                .SelectMany(t => t.GetInterfaces()
                    .Where(i => i.IsGenericType && (
                        i.GetGenericTypeDefinition() == typeof(INotificationHandler<>)))
                    .Select(s => new { Interface = s, Implementation = t }));

            foreach (var registration in serviceRegistrations)
            {
                services.AddTransient(registration.Interface, registration.Implementation);
            }
        }

        return services;
    }
}
