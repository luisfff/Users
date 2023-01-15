using EventSourcing;
using EventSourcing.Interfaces;
using Users.Domain;
using Users.Messages.Commands;

namespace Users.Application
{
    public class UserProfileCommandService : ApplicationService<UserProfile>
    {
        public UserProfileCommandService(
            IAggregateStore store
        ) : base(store)
        {
            CreateWhen<RegisterUser>(
                cmd => new UserId(cmd.UserId),
                (cmd, id) => UserProfile.Create(
                    new(id), FullName.FromString(cmd.FullName),
                    DisplayName.FromString(cmd.DisplayName)
                )
            );

            UpdateWhen<UpdateUserFullName>(
                cmd => new UserId(cmd.UserId),
                (user, cmd)
                    => user.UpdateFullName(FullName.FromString(cmd.FullName))
            );
        }
    }
}