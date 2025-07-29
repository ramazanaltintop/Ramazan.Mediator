# Ramazan.Mediator

A lightweight, extensible Mediator implementation for .NET - designed with Clean Architecture and Domain-Driven Design (DDD) in mind.

Use it to decouple your application layers while supporting CQRS, request/response messaging, pipeline behaviors, and domain events - all with minimal setup.

## Features

- **Command/Query Separation**
  - Send commands and queries via a central mediator
- **Pipeline Behaviors**
  - Add cross-cutting concerns like logging, validation, and transactions
- **Domain Events**
  - Publish events to multiple handlers with full async support
- **Dependency Injection**
  - Works with Microsoft.Extensions.DependencyInjection
- **Async/Await Friendly**
  - Fully asynchronous from top to bottom

## Installation

```bash
dotnet add package Ramazan.Mediator
```

## Setup

```csharp
services.AddMediator(config =>
{
    config.AddRegisterAssemblies(
        typeof(Program).Assembly,
        typeof(CreateCategoryCommand).Assembly);

    config.AddOpenBehavior(typeof(LoggingBehavior<>));        
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});
```

## Commands & Queries

### Command without Response

```csharp
public class DeleteCategoryCommand : ICommand
{
    public int Id { get; set; }
}

public class DeleteCategoryCommandHandler : ICommandHandler<DeleteCategoryCommand>
{
    public Task Handle(DeleteCategoryCommand cmd, CancellationToken ct)
    {
        Console.WriteLine($"Deleted {cmd.Id}");
        return Task.CompletedTask;
    }
}
```

```csharp
await sender.Send(new DeleteCategoryCommand { Id = 42 }, cancellationToken);
```

### Command with Response

```csharp
public class CreateCategoryCommand : ICommand<CreateCategoryCommandResponse>
{
    public string Name { get; set; }
}

public class CreateCategoryCommandHandler : ICommandHandler<CreateCategoryCommand, CreateCategoryCommandResponse>
{
    public Task<CreateCategoryCommandResponse> Handle(CreateCategoryCommand cmd, CancellationToken ct)
        => Task.FromResult(new CreateCategoryCommandResponse { Id = 1, Name = cmd.Name });
}
```

```csharp
var result = await sender.Send(new CreateCategoryCommand { Name = "Tech" }, cancellationToken);
```

### Query

```csharp
public class GetCategoryByIdQuery : IQuery<GetCategoryByIdQueryResponse>
{
    public int Id { get; set; }
}

public class GetCategoryByIdQueryHandler : IQueryHandler<GetCategoryByIdQuery, GetCategoryByIdQueryResponse>
{
    public Task<GetCategoryByIdQueryResponse> Handle(GetCategoryByIdQuery query, CancellationToken ct)
        => Task.FromResult(new GetCategoryByIdQueryResponse { Id = query.Id, Name = "Sample" });
}
```

```csharp
var category = await sender.Send(new GetCategoryByIdQuery { Id = 1 }, cancellationToken);
```

## Pipeline Behaviors

### Example: Logging without Response

```csharp
public class LoggingBehavior<TRequest> : IPipelineBehavior<TRequest>
    where TRequest : IRequest
{
    public async Task Handle(TRequest request, RequestHandlerDelegate next, CancellationToken ct)
    {
        Console.WriteLine($"Handling {typeof(TRequest).Name}");
        await next();
        Console.WriteLine($"Handled {typeof(TRequest).Name}");
    }
}
```

### Example: Logging with Response

```csharp
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        Console.WriteLine($"Handling {typeof(TRequest).Name}");
        var response = await next();
        Console.WriteLine($"Handled {typeof(TRequest).Name}");
        return response;
    }
}
```

## Notifications (Domain Events)

```csharp
public class CategoryCreatedNotification : INotification
{
    public string CategoryName { get; init; }
}

public class EmailHandler : INotificationHandler<CategoryCreatedNotification>
{
    public Task Handle(CategoryCreatedNotification notification, CancellationToken ct)
        => Console.Out.WriteLineAsync($"Email sent for {notification.CategoryName}");
}
```

```csharp
await sender.Publish(new CategoryCreatedNotification { CategoryName = "Tech" }, cancellationToken);
```

## Requirements

* .NET 9.0 or higher
* Microsoft.Extensions.DependencyInjection.Abstractions

## Contributing

Contributions, suggestions, and pull requests are welcome. Feel free to fork and improve!

## License

MIT [LICENSE](LICENSE)