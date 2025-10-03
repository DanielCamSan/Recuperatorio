using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
namespace Recuperatorio.Controllers
{
    [ApiController]
    [Route("api/v1/{controller}")]
    public class GuestsController : ControllerBase
    {
        private static readonly List<Guest> _guests = new()
        {
            new Guest {Id = Guid.NewGuid(),FullName="LuisAguilar",Email="a@gmail.com",Phone="11111111",Confirmed=true},
            new Guest {Id = Guid.NewGuid(),FullName="Leo",Email="b@gmail.com",Phone="22222222",Confirmed=true},
            new Guest {Id = Guid.NewGuid(),FullName="Nicole",Email="c@gmail.com",Phone="33333333",Confirmed=false},
            new Guest {Id = Guid.NewGuid(),FullName="Carol",Email="d@gmail.com",Phone="44444444",Confirmed=true},
            new Guest {Id = Guid.NewGuid(),FullName="Adrian",Email="e@gmail.com",Phone="55555555",Confirmed=false},
        };
        [HttpGet("{id:guid}")]
        public IActionResult GetOne(Guid id)
        {
            var guest = _guests.FirstOrDefault(g => g.Id == id);
            return guest is null ? NotFound(new { error = "Guest not found", status = 404 })
                : Ok(guest);
        }
        [HttpPost]
        public IActionResult Create([FromBody] CreateGuestDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var guest = new Guest
            {
                Id=Guid.NewGuid(),
                FullName=dto.FullName.Trim(),
                Email=dto.Email.Trim(),
                Phone=dto.Phone.Trim(),
                Confirmed=dto.Confirmed
            };
            _guests.Add(guest);
            return CreatedAtAction(nameof(GetOne), new { id = guest.Id }, guest);
        }
        [HttpPut("{id:guid}")]
        public IActionResult Update(Guid id, [FromBody] UpdateGuestDto dto)
        {
            if(!ModelState.IsValid) return ValidationProblem(ModelState);
            var index = _guests.FindIndex(g => g.Id == id);
            if (index == -1) return NotFound(new { error = "Guest not found", status = 404 });
            var updated = new Guest
            {
                Id=id,
                FullName=dto.FullName.Trim(),
                Email=dto.Email.Trim(),
                Phone=dto.Phone.Trim(),
                Confirmed=dto.Confirmed
            };
            _guests[index] = updated;
            return Ok(updated);
        }
        [HttpDelete("{id:guid}")]
        public IActionResult Delete(Guid id)
        {
            var removed = _guests.RemoveAll(g => g.Id == id);
            return removed == 0
                ? NotFound(new { error = "Guest not fount", status = 404 })
                : NoContent();
        }

        private static (int page,int limit) NormalizePage(int? page, int? limit)
        {
            var p = page.GetValueOrDefault(1); if (p < 1) p = 1;
            var l = limit.GetValueOrDefault(10); if (l < 1) l = 1; if (l > 100) l = 100;
            return (p, l);
        }
        private static IEnumerable<T> OrderByProp<T>(IEnumerable<T> src,string? sort,string? order)
        {
            if(string.IsNullOrWhiteSpace(sort)) return src;
            var prop = typeof(T).GetProperty(sort,BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if(prop is null) return src;
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
            if(!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(g=>
                g.FullName.Contains(q,StringComparison.OrdinalIgnoreCase) ||
                g.Email.Contains(q,StringComparison.OrdinalIgnoreCase) );
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
