using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PartyPlannerAPI.DTOs;
using PartyPlannerAPI.Models;

namespace PartyPlannerAPI.Controllers
{
    [ApiController]
    [Route("api/v1/events")]
    [EnableRateLimiting("fixed")]
    public class EventsController : ControllerBase
    {
        private static List<Event> _events = new();

        // GET: api/v1/events
        [HttpGet]
        public IActionResult GetEvents([FromQuery] int page = 1, [FromQuery] int limit = 10,
                                     [FromQuery] string sort = "id", [FromQuery] string order = "asc")
        {
            // Normalizar parámetros
            if (page < 1) page = 1;
            if (limit < 1) limit = 10;
            if (limit > 100) limit = 100;
            if (order != "asc" && order != "desc") order = "asc";

            // Ordenamiento seguro
            var query = _events.AsQueryable();
            query = sort.ToLower() switch
            {
                "title" => order == "asc" ? query.OrderBy(e => e.Title) : query.OrderByDescending(e => e.Title),
                "date" => order == "asc" ? query.OrderBy(e => e.Date) : query.OrderByDescending(e => e.Date),
                "location" => order == "asc" ? query.OrderBy(e => e.Location) : query.OrderByDescending(e => e.Location),
                "theme" => order == "asc" ? query.OrderBy(e => e.Theme) : query.OrderByDescending(e => e.Theme),
                "capacity" => order == "asc" ? query.OrderBy(e => e.Capacity) : query.OrderByDescending(e => e.Capacity),
                _ => order == "asc" ? query.OrderBy(e => e.Id) : query.OrderByDescending(e => e.Id)
            };

            var total = query.Count();
            var items = query.Skip((page - 1) * limit).Take(limit).Select(e => new EventDto
            {
                Id = e.Id,
                Title = e.Title,
                Date = e.Date,
                Location = e.Location,
                Theme = e.Theme,
                Capacity = e.Capacity
            }).ToList();

            var response = new
            {
                data = items,
                meta = new { page, limit, total }
            };

            return Ok(response);
        }

        // GET: api/v1/events/{id}
        [HttpGet("{id}")]
        public IActionResult GetEvent(Guid id)
        {
            var eventItem = _events.FirstOrDefault(e => e.Id == id);
            if (eventItem == null)
                return NotFound(new { error = "Event not found", status = 404 });

            var eventDto = new EventDto
            {
                Id = eventItem.Id,
                Title = eventItem.Title,
                Date = eventItem.Date,
                Location = eventItem.Location,
                Theme = eventItem.Theme,
                Capacity = eventItem.Capacity
            };

            return Ok(eventDto);
        }

        // POST: api/v1/events
        [HttpPost]
        public IActionResult CreateEvent([FromBody] CreateEventDto createEventDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var newEvent = new Event
            {
                Id = Guid.NewGuid(),
                Title = createEventDto.Title,
                Date = createEventDto.Date,
                Location = createEventDto.Location,
                Theme = createEventDto.Theme,
                Capacity = createEventDto.Capacity
            };

            _events.Add(newEvent);

            var eventDto = new EventDto
            {
                Id = newEvent.Id,
                Title = newEvent.Title,
                Date = newEvent.Date,
                Location = newEvent.Location,
                Theme = newEvent.Theme,
                Capacity = newEvent.Capacity
            };

            return CreatedAtAction(nameof(GetEvent), new { id = newEvent.Id }, eventDto);
        }

        // PUT: api/v1/events/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateEvent(Guid id, [FromBody] UpdateEventDto updateEventDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var eventItem = _events.FirstOrDefault(e => e.Id == id);
            if (eventItem == null)
                return NotFound(new { error = "Event not found", status = 404 });

            eventItem.Title = updateEventDto.Title;
            eventItem.Date = updateEventDto.Date;
            eventItem.Location = updateEventDto.Location;
            eventItem.Theme = updateEventDto.Theme;
            eventItem.Capacity = updateEventDto.Capacity;

            return Ok(new EventDto
            {
                Id = eventItem.Id,
                Title = eventItem.Title,
                Date = eventItem.Date,
                Location = eventItem.Location,
                Theme = eventItem.Theme,
                Capacity = eventItem.Capacity
            });
        }

        // DELETE: api/v1/events/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteEvent(Guid id)
        {
            var eventItem = _events.FirstOrDefault(e => e.Id == id);
            if (eventItem == null)
                return NotFound(new { error = "Event not found", status = 404 });

            _events.Remove(eventItem);
            return NoContent();
        }
    }
}
