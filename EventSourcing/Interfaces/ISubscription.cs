namespace EventSourcing.Interfaces;

public interface ISubscription
{
    Task Project(object @event);
}