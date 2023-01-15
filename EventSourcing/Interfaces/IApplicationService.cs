namespace EventSourcing.Interfaces;

public interface IApplicationService
{
    Task Handle(object command);
}