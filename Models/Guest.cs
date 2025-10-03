using System;
using System.ComponentModel.DataAnnotations;
public class Guest
{
    public Guid Id { get; set; }
    [Required, StringLength(100)]
    public string FullName { get; set; }=string.Empty;
    [Required, StringLength(100)]
    public string Email { get; set; } = string.Empty;
    [Required, StringLength(100)]
    public string PhoneNumber { get; set; } = string.Empty;
    public bool Confirmed { get; set; }
}

public record CreateUserDto
{
    [Required, StringLength(100)]
    public string FullName { get; set; } = string.Empty;
    [Required, StringLength(100)]
    public string Email { get; set; } = string.Empty;
    [Required, StringLength(100)]
    public string PhoneNumber { get; set; } = string.Empty;
    public bool Confirmed { get; set; }
}

public record UpdateUserDto
{
    [Required, StringLength(100)]
    public string FullName { get; set; } = string.Empty;
    [Required, StringLength(100)]
    public string Email { get; set; } = string.Empty;
    [Required, StringLength(100)]
    public string PhoneNumber { get; set; } = string.Empty;
    public bool Confirmed { get; set; }
}