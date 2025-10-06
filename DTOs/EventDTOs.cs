using System.ComponentModel.DataAnnotations;

namespace PartyPlannerAPI.DTOs
{
    public class CreateEventDto
    {
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

    public class UpdateEventDto
    {
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

    public class EventDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public string Theme { get; set; }
        public int Capacity { get; set; }
    }
}