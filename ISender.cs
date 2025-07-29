using Microsoft.Extensions.DependencyInjection;

namespace Ramazan.Mediator;

public interface ISender
{
    Task Send<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand;
    Task<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default);
    Task<TResponse> Send<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default);
    Task Publish(INotification notification, CancellationToken cancellationToken = default);
}

public sealed class Sender(
    IServiceScopeFactory scopeFactory) : ISender
{
    public async Task Send<TCommand>(
        TCommand command,
        CancellationToken cancellationToken = default) where TCommand : ICommand
    {
        using var scoped = scopeFactory.CreateScope();
        var sp = scoped.ServiceProvider;
        var reflectionType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
        var pipelineType = typeof(IPipelineBehavior<>).MakeGenericType(command.GetType());

        RequestHandlerDelegate handlerDelegate = () =>
        {
            var handler = sp.GetRequiredService(reflectionType);
            var method = reflectionType.GetMethod("Handle")!;
            return (Task)method.Invoke(handler, new object[] { command, cancellationToken })!;
        };

        var behaviors = (IEnumerable<object>)sp.GetServices(pipelineType);

        var pipeline = behaviors
            .Reverse()
            .Aggregate(
                handlerDelegate,
                (next, behavior) =>
                {
                    return () =>
                    {
                        var method = pipelineType.GetMethod("Handle")!;
                        return (Task)method.Invoke(
                            behavior,
                            new object[] { command, next, cancellationToken })!;
                    };
                }
            );

        await pipeline();
    }

    public async Task<TResponse> Send<TResponse>(
        ICommand<TResponse> command,
        CancellationToken cancellationToken = default)
    {
        using var scoped = scopeFactory.CreateScope();
        var sp = scoped.ServiceProvider;
        var reflectionType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResponse));
        var pipelineType = typeof(IPipelineBehavior<,>).MakeGenericType(command.GetType(), typeof(TResponse));

        RequestHandlerDelegate<TResponse> handlerDelegate = () =>
        {
            var handler = sp.GetRequiredService(reflectionType);
            var method = reflectionType.GetMethod("Handle")!;
            return (Task<TResponse>)method.Invoke(handler, new object[] { command, cancellationToken })!;
        };

        var behaviors = (IEnumerable<object>)sp.GetServices(pipelineType);

        var pipeline = behaviors
            .Reverse()
            .Aggregate(
                handlerDelegate,
                (next, behavior) =>
                {
                    return () =>
                    {
                        var method = pipelineType.GetMethod("Handle")!;
                        return (Task<TResponse>)method.Invoke(
                            behavior,
                            new object[] { command, next, cancellationToken })!;
                    };
                }
            );

        return await pipeline();
    }

    public async Task<TResponse> Send<TResponse>(
        IQuery<TResponse> query,
        CancellationToken cancellationToken = default)
    {
        using var scoped = scopeFactory.CreateScope();
        var sp = scoped.ServiceProvider;
        var reflectionType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResponse));
        var pipelineType = typeof(IPipelineBehavior<,>).MakeGenericType(query.GetType(), typeof(TResponse));

        RequestHandlerDelegate<TResponse> handlerDelegate = () =>
        {
            var handler = sp.GetRequiredService(reflectionType);
            var method = reflectionType.GetMethod("Handle")!;
            return (Task<TResponse>)method.Invoke(handler, new object[] { query, cancellationToken })!;
        };

        var behaviors = (IEnumerable<object>)sp.GetServices(pipelineType);

        var pipeline = behaviors
            .Reverse()
            .Aggregate(
                handlerDelegate,
                (next, behavior) =>
                {
                    return () =>
                    {
                        var method = pipelineType.GetMethod("Handle")!;
                        return (Task<TResponse>)method.Invoke(
                            behavior,
                            new object[] { query, next, cancellationToken })!;
                    };
                }
            );

        return await pipeline();
    }

    public async Task Publish(INotification notification, CancellationToken cancellationToken = default)
    {
        using var scoped = scopeFactory.CreateScope();
        var sp = scoped.ServiceProvider;

        var reflectionType = typeof(INotificationHandler<>).MakeGenericType(notification.GetType());
        var handlers = (IEnumerable<object>)sp.GetServices(reflectionType);

        var tasks = handlers
            .Select(handler =>
            {
                var method = reflectionType.GetMethod("Handle")!;
                return (Task)method.Invoke(handler, new object[] { notification, cancellationToken })!;
            })
            .ToArray();

        await Task.WhenAll(tasks);
    }
}
