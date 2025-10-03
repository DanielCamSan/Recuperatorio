using Microsoft.AspNetCore.Mvc;
using Recuperatorio.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Recuperatorio.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")] // si quieres idéntico a tu imagen cambia a: api/[controller]
    public class EventsController : ControllerBase
    {
        private static readonly List<Events> _db = new()
        {
            new Events { Title="Fiesta Neon",     Date=DateTime.UtcNow.AddMonths(2), Location="Club X",   Theme="neon",      Capacity=300 },
            new Events { Title="Gala Formal",     Date=DateTime.UtcNow.AddMonths(5), Location="Hotel Y",  Theme="formal",    Capacity=150 },
            new Events { Title="Noche Halloween", Date=DateTime.UtcNow.AddDays(25),  Location="Salon Z",  Theme="halloween", Capacity=400 }
        };
        private static (int page, int limit) Normalize(int page, int limit)
        {
            page = Math.Max(1, page);
            limit = Math.Clamp(limit, 1, 100);
            return (page, limit);
        }

        private static IEnumerable<Events> ApplySort(IEnumerable<Events> q, string? sort, string order)
        {
            bool desc = string.Equals(order, "desc", StringComparison.OrdinalIgnoreCase);
            return (sort?.ToLowerInvariant()) switch
            {
                "title" => desc ? q.OrderByDescending(x => x.Title) : q.OrderBy(x => x.Title),
                "date" => desc ? q.OrderByDescending(x => x.Date) : q.OrderBy(x => x.Date),
                "capacity" => desc ? q.OrderByDescending(x => x.Capacity) : q.OrderBy(x => x.Capacity),
                "location" => desc ? q.OrderByDescending(x => x.Location) : q.OrderBy(x => x.Location),
                "theme" => desc ? q.OrderByDescending(x => x.Theme) : q.OrderBy(x => x.Theme),
                _ => q
            };
        }
        private static object Paged<T>(IEnumerable<T> source, int page, int limit)
        {
            var total = source.Count();
            var items = source.Skip((page - 1) * limit).Take(limit).ToList();
            return new { data = items, meta = new { page, limit, total } };
        }

        // GET: /api/v1/events?page=&limit=&sort=&order=
        [HttpGet]
        public IActionResult GetAll([FromQuery] int page = 1, [FromQuery] int limit = 10,
                                    [FromQuery] string? sort = null, [FromQuery] string order = "asc")
        {
            (page, limit) = Normalize(page, limit);
            IEnumerable<Events> q = ApplySort(_db, sort, order);
            return Ok(Paged(q, page, limit)); // 200
        }

        // GET: /api/v1/events/{id}
        [HttpGet("{id:guid}")]
        public IActionResult GetById(Guid id)
        {
            var ev = _db.FirstOrDefault(e => e.Id == id);
            return ev is null
                ? NotFound(new { error = "Event not found", status = 404 })
                : Ok(new { data = ev, meta = (object?)null }); // 200
        }

        // POST: /api/v1/events
        [HttpPost]
        public IActionResult Create([FromBody] CreateEventDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            if (dto.Capacity <= 0)
                return BadRequest(new { error = "Capacity must be > 0", status = 400 });

            var ev = new Events
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Date = dto.Date,
                Location = dto.Location,
                Theme = dto.Theme,
                Capacity = dto.Capacity
            };
            _db.Add(ev);

            return CreatedAtAction(nameof(GetById), new { id = ev.Id }, new { data = ev, meta = (object?)null }); // 201
        }

        // PUT: /api/v1/events/{id}
        [HttpPut("{id:guid}")]
        public IActionResult Update(Guid id, [FromBody] UpdateEventDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            if (dto.Capacity <= 0)
                return BadRequest(new { error = "Capacity must be > 0", status = 400 });

            var ev = _db.FirstOrDefault(e => e.Id == id);
            if (ev is null) return NotFound(new { error = "Event not found", status = 404 });

            ev.Title = dto.Title;
            ev.Date = dto.Date;
            ev.Location = dto.Location;
            ev.Theme = dto.Theme;
            ev.Capacity = dto.Capacity;

            return Ok(new { data = ev, meta = (object?)null }); // 200
        }

        // DELETE: /api/v1/events/{id}
        [HttpDelete("{id:guid}")]
        public IActionResult Delete(Guid id)
        {
            var ev = _db.FirstOrDefault(e => e.Id == id);
            if (ev is null) return NotFound(new { error = "Event not found", status = 404 });

            _db.Remove(ev);
            return NoContent(); // 204
        }
    }
}