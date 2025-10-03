using System.ComponentModel.DataAnnotations;
public class Ticket
{
    public Guid Id { get; set; }
    public Guid GuestId { get; set; }
    public Guid EventId { get; set; }
    [Required]
    public string Type { get; set; } = string.Empty; // general | vip | backstage
    public decimal Price { get; set; }
    [Required]
    public string Status { get; set; } = "valid"; // valid | used | canceled

    public string? Notes { get; set; } = string.Empty;


}

public record CreatedTicketDto
{
    [Required]
    public required string Type { get; set; }  // general | vip | backstage
    public decimal Price { get; set; }
    [Required]
    public required string Status { get; set; }  // valid | used | canceled

    public required string? Notes { get; set; } 

}

public record UpdateTicketDto
{
    [Required]
    public required string Type { get; set; }  // general | vip | backstage
    public decimal Price { get; set; }
    [Required]
    public required string Status { get; set; }  // valid | used | canceled

    public required string? Notes { get; set; } 

}