namespace Users.Messages.Commands;

public class UpdateUserFullName
{
    public long UserId { get; set; }
    public string FullName { get; set; }
}