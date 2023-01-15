using EventSourcing;
using Users.Messages.Events;

namespace Users.Domain;

public class UserProfile : AggregateRoot
{
    public static UserProfile Create(
        UserId id,
        FullName fullName,
        DisplayName displayName)
    {
        var profile = new UserProfile();

        profile.Apply(
            new UserRegistered
            {
                UserId = id,
                FullName = fullName,
                DisplayName = displayName
            }
        );
        return profile;
    }

    // Aggregate state properties
    private FullName FullName { get; set; }
    private DisplayName DisplayName { get; set; }
    private string PhotoUrl { get; set; }

    public void UpdateFullName(FullName fullName)
        => Apply(
            new UserRegistered
            {
                UserId = Id,
                FullName = fullName
            }
        );

    protected override void When(object @event)
    {
        switch (@event)
        {
            case UserRegistered e:
                Id = e.UserId;
                FullName = new(e.FullName);
                DisplayName = new(e.DisplayName);
                break;
            case UserFullNameUpdated e:
                FullName = new(e.FullName);
                break;
        }
    }

    protected override void EnsureValidState() { }
}