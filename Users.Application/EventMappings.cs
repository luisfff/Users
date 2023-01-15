using Users.Messages.Events;
using static EventSourcing.TypeMapper;

namespace Users.Application
{
    public static class EventMappings
    {
        public static void MapEventTypes()
        {
            Map<UserRegistered>("UserRegistered");
            Map<UserRegistered>("UserFullNameUpdated");
        }
    }
}