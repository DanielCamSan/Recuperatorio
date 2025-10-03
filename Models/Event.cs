using System.ComponentModel.DataAnnotations;
public class Event
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Theme { get; set; } = string.Empty;
    public int Capacity { get; set; }
}

public record CreateEventDto
{
    public string Title { get; set;} = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Theme { get; set; } = string.Empty;
    [Range(0,100)]
    public int Capacity { get; set; }


}

public record UpdateEventDto
{
    public string Title { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Theme { get; set; } = string.Empty;
    public int Capacity { get; set; }

}