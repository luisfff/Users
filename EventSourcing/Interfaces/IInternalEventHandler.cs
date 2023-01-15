namespace EventSourcing.Interfaces;

public interface IInternalEventHandler
{
    void Handle(object @event);
}