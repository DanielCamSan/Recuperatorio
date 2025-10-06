using System.ComponentModel.DataAnnotations;

namespace Recuperatorio.Models
{
    public class Ticket
    {

        Guid Id {  get; set; }
        Guid GuestId { get; set; }
        Guid EventId { get; set; }
        [Required, StringLength(100)]
        string Type { get; set; } // general | vip | backstage
        [Required, StringLength(100)]
        float Price { get; set; }
        [Required, StringLength(100)]
        string Status { get; set; } // valid | used | canceled
        [Required, StringLength(100)]
        string? Notes { get; set; }
    }
}
