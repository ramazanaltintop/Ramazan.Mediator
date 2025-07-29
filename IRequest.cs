namespace CleanArchitecture_2025.Application.Abstractions.Messaging;

public interface IRequest;

public interface IRequest<TResponse>;

public delegate Task RequestHandlerDelegate();

public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();