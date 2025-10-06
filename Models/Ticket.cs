using System.ComponentModel.DataAnnotations;

namespace Recuperatorio.Models
{
    public class Ticket
    {

        public Guid Id {  get; set; }
        public Guid GuestId { get; set; }
        public Guid EventId { get; set; }

        [Required, StringLength(100)]
        public string Type { get; set; } = string.Empty; // general | vip | backstage
       
        public double Price { get; set; }

        [Required, StringLength(100)]
        public string Status { get; set; } = "VALID"; // valid | used | canceled
        [Required, StringLength(100)]
        public string Notes { get; set; } = string.Empty;
    }
    public record CreateTicketDto
    {

        [Required, StringLength(100)]
        public string Type { get; set; } = string.Empty; // general | vip | backstage

        public double Price { get; set; }

        [Required, StringLength(100)]
        public string Status { get; set; } = "VALID"; // valid | used | canceled
        [Required, StringLength(100)]
        public string Notes { get; set; } = string.Empty;
    }
}
