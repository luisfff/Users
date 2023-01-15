using EventSourcing.Interfaces;

namespace EventSourcing;

public abstract class CommandService<T> where T : class, IAggregateState<T>, new()
{
    private IFunctionalAggregateStore Store { get; }

    protected CommandService(IFunctionalAggregateStore store) 
        => Store = store;

    protected async Task Handle(Guid id, Func<T, AggregateState<T>.Result> update)
    {
        var state = await Store.Load<T>(id);
        await Store.Save(state.Version, update(state));
    }
}