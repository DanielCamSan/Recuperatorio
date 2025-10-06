using Microsoft.AspNetCore.Mvc;
using Recuperatorio.Models;
using System.Reflection;
namespace Recuperatorio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class TicketController : ControllerBase
    {
       
       
            private static readonly List<Ticket> _Tickets = new()
        {
            new Ticket { Id = Guid.NewGuid(),GuestId = Guid.NewGuid(), EventId = Guid.NewGuid(), Type = "BACKSTAGE", Price=419.99, Notes = "None" },
            new Ticket { Id = Guid.NewGuid(),GuestId = Guid.NewGuid(), EventId = Guid.NewGuid(), Type = "GENERAL", Price=49.99, Notes = "None" },
            new Ticket { Id = Guid.NewGuid(),GuestId = Guid.NewGuid(), EventId = Guid.NewGuid(), Type = "VIP", Price=19.99, Status="USED", Notes = "None"  },
            new Ticket {Id = Guid.NewGuid(),GuestId = Guid.NewGuid(), EventId = Guid.NewGuid(), Type = "GENERAL", Price=9.99, Status="CANCELED", Notes = "None"  },
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
            IEnumerable<Ticket> query = _Tickets;
            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(g => g.Type.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                                     g.Notes.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                                     g.Status.Contains(q, StringComparison.OrdinalIgnoreCase));
            }
            query = OrderByProp(query, sort, order);
            var total = query.Count();
            var data = query.Skip((p - 1) * l).Take(l).ToList();
            return Ok(new { data, meta = new { page = p, limit = l, total } });
        }


        [HttpGet("{id:guid}")]
        public ActionResult<Ticket> GetOne(Guid id)
        {
            var ticket = _Tickets.FirstOrDefault(a => a.Id == id);
            return ticket is null
                ? NotFound(new { error = "ticket not found", status = 404 })
                : Ok(ticket);
        }

        [HttpPost]
        public ActionResult<Ticket> Create([FromBody] CreateTicketDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                GuestId = Guid.NewGuid(),
                EventId = Guid.NewGuid(),
                Type = dto.Type.Trim(),
                Price = dto.Price,
                Notes = dto.Notes
            };

            _Tickets.Add(ticket);
            return CreatedAtAction(nameof(GetOne), new { id = ticket.Id }, ticket);
        }

        [HttpDelete("{id:guid}")]
        public IActionResult Delete(Guid id)
        {
            var byebye = _Tickets.RemoveAll(a => a.Id == id);
            return byebye == 0
                ? NotFound(new { error = "Ticket not found", status = 404 })
                : NoContent();
        }

    }
}
