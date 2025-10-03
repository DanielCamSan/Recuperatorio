using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Recuperatorio.Controllers
{
    [ApiController]
    [Route("api/v1/{controller}")]
    public class EventsController : ControllerBase
    {
        private static readonly List<Event> _events = new()
        {
            new Event {Id = Guid.NewGuid(), Title = "Fiesta", Location = "America", Theme = "Infantil", Capacity = 100 },
            new Event {Id = Guid.NewGuid(), Title = "Matrimonio", Location = "America2", Theme = "Boda", Capacity = 150 },
            new Event {Id = Guid.NewGuid(), Title = "Cumpleaños", Location = "Circunvalacion", Theme = "Luces", Capacity = 100}
        };

        [HttpGet("{id:guid}")]
        public IActionResult GetOne(Guid id)
        {
            var events = _events.FirstOrDefault(x => x.Id == id);
            return events is null ? NotFound(new { error = "Event not found", status = 404 })
                : Ok(events);
        }
        [HttpPost]
        public IActionResult Create([FromBody] CreateEventDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var events = new Event
            {
                Id = Guid.NewGuid(),
                Title = dto.Title.Trim(),
                Location = dto.Location.Trim(),
                Theme = dto.Theme.Trim()
            };
            _events.Add(events);
            return CreatedAtAction(nameof(GetOne), new { id = events.Id }, events);
        }

        [HttpPut("{id:guid}")]
        public IActionResult Update(Guid id, [FromBody] UpdateEventDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var index = _events.FindIndex(x => x.Id == id);
            if (index == -1) return NotFound(new { error = "Event not found", status = 404 });
            var updated = new Event
            {
                Id = id,
                Title = dto.Title.Trim(),
                Location = dto.Location.Trim(),
                Theme = dto.Theme.Trim()

            };
            _events[index] = updated;
            return Ok(updated);
        }

        [HttpDelete("{id:guid}")]
        public IActionResult Delete(Guid id)
        {
            var removed = _events.RemoveAll(x => x.Id == id);
            return removed == 0
                ? NotFound(new { error = "Event not found", status = 404 })
                : NoContent();
        }

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
        public IActionResult GetAll(
            [FromQuery] int? page,
            [FromQuery] int? limit,
            [FromQuery] string? sort,
            [FromQuery] string? order,
            [FromQuery] string? q)
        {
            var (p, l) = NormalizePage(page, limit);
            IEnumerable<Event> query = _events;
            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(g =>
                g.Title.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                g.Location.Contains(q, StringComparison.OrdinalIgnoreCase));
            }
            query = OrderByProp(query, sort, order);
            var total = query.Count();
            var data = query.Skip((p - 1) * l).Take(l).ToList();
            return Ok(new
            {
                data,
                meta = new { page = p, limit = l, total }
            });
        }




    }
}
