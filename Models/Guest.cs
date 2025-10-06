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
        public bool Confirmed { get; set; }=true;
    }
}
