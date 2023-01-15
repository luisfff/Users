namespace Users.Messages.Events;

public class UserFullNameUpdated
{
    public long UserId { get; set; }
    public string FullName { get; set; }
}