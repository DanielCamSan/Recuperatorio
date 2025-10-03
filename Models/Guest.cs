using System.ComponentModel.DataAnnotations;

public class Guest
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    [EmailAddress]
    public string Email {  get; set; } = string.Empty;
    [Range(7,20)]
    public string Phone {  get; set; } = string.Empty;
    public bool Confirmed { get; set; } = false;
}
public record CreateGuestDto
{
    public string FullName {  set; get; } = string.Empty;
    [EmailAddress]
    public string Email { get; set; } =string.Empty;
    
    public string Phone { get; set; } = string.Empty;
    public bool Confirmed { get; set; } = false;
}
public record UpdateGuestDto
{
    public required string FullName { set; get; }
    [EmailAddress]
    public  required string Email { get; set; }
    
    public  required string Phone { get; set; }
    public bool Confirmed { get; set; } = false;
}