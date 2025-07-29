namespace Ramazan.Mediator;

public interface ICommand : IRequest;

public interface ICommand<TResponse> : IRequest<TResponse>;