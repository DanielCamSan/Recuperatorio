using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Recuperatorio.Models;
using System.Net.Sockets;
using System.Reflection;

namespace Recuperatorio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GuestController:ControllerBase
    {

        private static readonly List<Guest> _guests = new()
        {
            new Guest { Id = Guid.NewGuid(), FullName ="yessica",Email="yes@ucbhhh",Phone="12234444",Confirmed=false},
            new Guest { Id = Guid.NewGuid(), FullName ="jose",Email="yes@dddd",Phone="12234444",Confirmed=true},
            new Guest { Id = Guid.NewGuid(), FullName ="yessica",Email="yes@ucbhhh",Phone="12234444",Confirmed=false}
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
            IEnumerable<Guest> query = _guests;
            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(g => g.FullName.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                                     g.Email.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                                     g.Phone.Contains(q, StringComparison.OrdinalIgnoreCase));
            }
            query = OrderByProp(query, sort, order);
            var total= query.Count();
            var data= query.Skip((p - 1) * l).Take(l).ToList();
            return Ok(new { data, meta = new { page= p, limit = l, total } });
        }
        [HttpGet("{id:guid}")]
        public ActionResult<Guest> GetOne(Guid id)
        {
            var guest = _guests.FirstOrDefault(a => a.Id == id);
            return guest is null
                ? NotFound(new { error = "ticket not found", status = 404 })
                : Ok(guest);
        }

        [HttpPost]
        public ActionResult<Guest> Create([FromBody] CreateGuestDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var guest = new Guest
            {
                Id = Guid.NewGuid(),
                FullName = dto.FullName.Trim(),
                Email = dto.Email.Trim(),
                Phone = dto.Phone.Trim(),
                Confirmed= dto.Confirmed
            };

            _guests.Add(guest);
            return CreatedAtAction(nameof(GetOne), new { id = guest.Id }, guest);
        }

        [HttpDelete("{id:guid}")]
        public ActionResult Delete(Guid id)
        {
            var removed = _guests.RemoveAll(a => a.Id == id);
            return removed == 0
                ? NotFound(new { error = "guest not found", status = 404 })
                : NoContent();
        }

    }
}
