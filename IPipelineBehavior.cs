namespace Ramazan.Mediator;

public interface IPipelineBehavior<TRequest>
    where TRequest : IRequest
{
    Task Handle(TRequest request,
        RequestHandlerDelegate next,
        CancellationToken cancellationToken = default);
}

public interface IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default);
}
