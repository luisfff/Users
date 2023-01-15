namespace EventSourcing.Interfaces;

public interface ICheckpointStore
{
    Task<long?> GetCheckpoint();
    Task StoreCheckpoint(long? checkpoint);
}