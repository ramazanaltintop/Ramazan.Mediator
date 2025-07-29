namespace CleanArchitecture_2025.Application.Abstractions.Messaging;

public interface INotification;

public interface INotificationHandler<TNotification> where TNotification : INotification
{
    Task Handle(TNotification notification, CancellationToken cancellationToken);
}