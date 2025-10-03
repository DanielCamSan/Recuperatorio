using System;
using System.ComponentModel.DataAnnotations;

namespace Recuperatorio.Models
{
    public class Events
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, StringLength(120)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public DateTime Date { get; set; }

        [Required, StringLength(160)]
        public string Location { get; set; } = string.Empty;

        [Required, StringLength(40)]
        public string Theme { get; set; } = "formal";

        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be > 0")]
        public int Capacity { get; set; }
    }

    public record CreateEventDto(
        [property: Required, StringLength(120)] string Title,
        [property: Required] DateTime Date,
        [property: Required, StringLength(160)] string Location,
        [property: Required, StringLength(40)] string Theme,
        [property: Range(1, int.MaxValue)] int Capacity
    );

    public record UpdateEventDto(
        [property: Required, StringLength(120)] string Title,
        [property: Required] DateTime Date,
        [property: Required, StringLength(160)] string Location,
        [property: Required, StringLength(40)] string Theme,
        [property: Range(1, int.MaxValue)] int Capacity
    );
}
