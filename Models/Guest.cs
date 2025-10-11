using System.ComponentModel.DataAnnotations;

namespace Recuperatorio.Models
{
    public class Guest
    {
        public Guid Id { get; set; }

        public string FullName { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required, StringLength(7-20)]
        public string Phone { get; set; } = string.Empty;
        //valor por defecto false
        public bool Confirmed { get; set; }=true;
    }
    public record CreateGuestDto
    {

        public string FullName { get; init; } = string.Empty;
        [Required]
        public string Email { get; init; } = string.Empty;
        [Required, StringLength(7 - 20)]
        public string Phone { get; init; } = string.Empty;
        public bool Confirmed { get; init; } = true;
    }
}
