using EventSourcing;

namespace Users.Domain;

public class UserId : AggregateId<UserProfile>
{
    public UserId(long value) : base(value) { }
}