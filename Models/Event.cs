using System.ComponentModel.DataAnnotations;

namespace PartyPlannerAPI.Models
{
    public class Event
    {
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public string Theme { get; set; }

        [Range(1, int.MaxValue)]
        public int Capacity { get; set; }
    }
}