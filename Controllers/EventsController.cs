using Microsoft.AspNetCore.Mvc;
using PartyPlannerAPI.DTOs;
using PartyPlannerAPI.Models;
using System.Reflection;

namespace Recuperatorio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private static readonly List<Event> _Events = new()
        {
            new Event { Id = Guid.NewGuid(), Title = "Fiesta Neon", Date = DateTime.Now.AddDays(10), Location = "Club Nocturno X", Theme = "neon", Capacity = 300 },
            new Event { Id = Guid.NewGuid(), Title = "Cena Formal", Date = DateTime.Now.AddDays(15), Location = "Hotel Luxury", Theme = "formal", Capacity = 150 },
            new Event { Id = Guid.NewGuid(), Title = "Halloween Party", Date = DateTime.Now.AddDays(25), Location = "Casa Embrujada", Theme = "halloween", Capacity = 200 },
            new Event { Id = Guid.NewGuid(), Title = "Fiesta Casual", Date = DateTime.Now.AddDays(5), Location = "Playa Central", Theme = "casual", Capacity = 100 },
        };

        private static (int page, int limit) NormalizePage(int? page, int? limit)
        {
            var p = page.GetValueOrDefault(1); if (p < 1) p = 1;
            var l = limit.GetValueOrDefault(10); if (l < 1) l = 1; if (l > 100) l = 100;
            return (p, l);
        }

        private static IEnumerable<T> OrderByProp<T>(IEnumerable<T> src, string? sort, string? order)
        {
            if (string.IsNullOrWhiteSpace(sort)) return src;
            var prop = typeof(T).GetProperty(sort, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (prop is null) return src;

            return string.Equals(order, "desc", StringComparison.OrdinalIgnoreCase)
                ? src.OrderByDescending(x => prop.GetValue(x))
                : src.OrderBy(x => prop.GetValue(x));
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] int? page,
            [FromQuery] int? limit,
            [FromQuery] string? sort,
            [FromQuery] string? order,
            [FromQuery] string? q)
        {
            var (p, l) = NormalizePage(page, limit);
            IEnumerable<Event> query = _Events;

            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(e => e.Title.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                                         e.Location.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                                         e.Theme.Contains(q, StringComparison.OrdinalIgnoreCase));
            }

            query = OrderByProp(query, sort, order);
            var total = query.Count();
            var data = query.Skip((p - 1) * l).Take(l).ToList();

            return Ok(new { data, meta = new { page = p, limit = l, total } });
        }

        [HttpGet("{id:guid}")]
        public ActionResult<Event> GetOne(Guid id)
        {
            var eventItem = _Events.FirstOrDefault(e => e.Id == id);
            return eventItem is null
                ? NotFound(new { error = "Event not found", status = 404 })
                : Ok(eventItem);
        }

        [HttpPost]
        public ActionResult<Event> Create([FromBody] CreateEventDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var eventItem = new Event
            {
                Id = Guid.NewGuid(),
                Title = dto.Title.Trim(),
                Date = dto.Date,
                Location = dto.Location.Trim(),
                Theme = dto.Theme.Trim(),
                Capacity = dto.Capacity
            };

            _Events.Add(eventItem);
            return CreatedAtAction(nameof(GetOne), new { id = eventItem.Id }, eventItem);
        }

        [HttpPut("{id:guid}")]
        public ActionResult<Event> Update(Guid id, [FromBody] UpdateEventDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var eventItem = _Events.FirstOrDefault(e => e.Id == id);
            if (eventItem is null)
                return NotFound(new { error = "Event not found", status = 404 });

            eventItem.Title = dto.Title.Trim();
            eventItem.Date = dto.Date;
            eventItem.Location = dto.Location.Trim();
            eventItem.Theme = dto.Theme.Trim();
            eventItem.Capacity = dto.Capacity;

            return Ok(eventItem);
        }

        [HttpDelete("{id:guid}")]
        public IActionResult Delete(Guid id)
        {
            var eventItem = _Events.FirstOrDefault(e => e.Id == id);
            if (eventItem is null)
                return NotFound(new { error = "Event not found", status = 404 });

            _Events.Remove(eventItem);
            return NoContent();
        }
    }
}
