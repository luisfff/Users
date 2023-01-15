using EventSourcing.Interfaces;
using Force.DeepCloner;

namespace EventSourcing;

public abstract class AggregateState<T> : IAggregateState<T>
    where T : class, new()
{
    public abstract T When(T state, object @event);
        
    public string GetStreamName(Guid id) => $"{typeof(T).Name}-{id:N}";

    public string StreamName => GetStreamName(Id);

    public long Version { get; protected set; }

    public Guid Id { get; protected set; }

    protected T With(T state, Action<T> update)
    {
        update(state);
        return state;
    }

    protected abstract bool EnsureValidState(T newState);

    private T Apply(T state, object @event)
    {
        var newState = state.DeepClone();
        newState = When(newState, @event);

        if (!EnsureValidState(newState))
            throw new InvalidEntityState(
                this, "Post-checks failed"
            );

        return newState;
    }

    public class Result
    {
        public T State { get; }
        public IEnumerable<object> Events { get; }

        public Result(T state, IEnumerable<object> events)
        {
            State = state;
            Events = events;
        }
    }

    public Result NoEvents() => new(this as T, new List<object>());

    public Result Apply(params object[] events)
    {
        var newState = this as T;
        newState = events.Aggregate(newState, Apply);
        return new(newState, events);
    }

}