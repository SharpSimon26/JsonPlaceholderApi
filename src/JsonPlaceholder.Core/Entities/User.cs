namespace JsonPlaceholder.Core.Entities;

public class User
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public string? Phone { get; set; }
    public string? Website { get; set; }
}
