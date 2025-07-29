namespace CleanArchitecture_2025.Application.Abstractions.Messaging;

public interface ICommand : IRequest;

public interface ICommand<TResponse> : IRequest<TResponse>;