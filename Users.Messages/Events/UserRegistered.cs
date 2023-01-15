namespace Users.Messages.Events;

public class UserRegistered
{
    public long UserId { get; set; }
    public string FullName { get; set; }
    public string DisplayName { get; set; }
}