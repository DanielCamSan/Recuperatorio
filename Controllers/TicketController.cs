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
            new Ticket { Id = Guid.NewGuid(),GuestId = Guid.NewGuid(), EventId = Guid.NewGuid(), Type = "FESTIVAL", Price=419.99, Status="Ongoing", Notes = "None" },
            new Ticket { Id = Guid.NewGuid(),GuestId = Guid.NewGuid(), EventId = Guid.NewGuid(), Type = "HOUSE", Price=49.99, Status="Ongoing", Notes = "None" },
            new Ticket { Id = Guid.NewGuid(),GuestId = Guid.NewGuid(), EventId = Guid.NewGuid(), Type = "TECHNO", Price=19.99, Status="Concluded", Notes = "None"  },
            new Ticket {Id = Guid.NewGuid(),GuestId = Guid.NewGuid(), EventId = Guid.NewGuid(), Type = "COUNTRY", Price=9.99, Status="Scheduled", Notes = "None"  },
        };

        private static (int page, int limit) NormalizePage(int? page, int? limit)
        {
            var p = page.GetValueOrDefault(1); if (p < 1) p = 1;
            var l = limit.GetValueOrDefault(10); if (l < 1) l = 1; if (l > 100) l = 100;
            return (p, l);
        }
        private static IEnumerable<T> OrderByProp<T>(IEnumerable<T> src, string? sort, string? order)
        {
            if (string.IsNullOrWhiteSpace(sort)) return src; // no-op
            var prop = typeof(T).GetProperty(sort, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (prop is null) return src; // campo inválido => no ordenar

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
    }
}
