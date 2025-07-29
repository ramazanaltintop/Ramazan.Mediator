namespace Ramazan.Mediator;

public interface IRequest;

public interface IRequest<TResponse>;

public delegate Task RequestHandlerDelegate();

public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();