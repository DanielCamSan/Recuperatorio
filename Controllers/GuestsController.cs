using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
namespace newCRUD.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class guestsController : ControllerBase
    {
        private static readonly List<Guest> _guests = new()
        {
            new Guest {Id=Guid.NewGuid(),FullName="Armando Barrera", Email="armandobarrera@gmail.com", PhoneNumber="77994086",Confirmed=true },
            new Guest {Id=Guid.NewGuid(),FullName="Tatiana Rodriguez", Email="tatianaroa@gmail.com", PhoneNumber="77786542",Confirmed=false }
        };
        private static (int page, int limit) NormalizePage(int? page, int? limit)
        {
            var p = page.GetValueOrDefault(1); if (p < 1) p = 1;
            var l = limit.GetValueOrDefault(10); if (l < 1) l = 1; if (l > 100) l = 100;
            return (p, l);
        }
        private static IEnumerable<T> OrderByProp<T>(IEnumerable<T> src, string? sort, string? order)
        {
            if (string.IsNullOrEmpty(sort)) return src;
            var prop = typeof(T).GetProperty(sort, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (prop == null) return src;
            return string.Equals(order, "desc", StringComparison.OrdinalIgnoreCase) ? src.OrderByDescending(x => prop.GetValue(x)) : src.OrderBy(x => prop.GetValue(x));
        }
        [HttpGet]
        public IActionResult GetAll([FromQuery] int? page, [FromQuery] int? limit, [FromQuery] string? sort, [FromQuery] string? order, [FromQuery] string? q, [FromQuery] string? species)
        {
            var (p, l) = NormalizePage(page, limit);
            IEnumerable<Guest> query = _guests;
            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(a => a.FullName.Contains(q, StringComparison.OrdinalIgnoreCase) || a.Email.Contains(q, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrWhiteSpace(species))
            {
                query = query.Where(a => a.FullName.Contains(q, StringComparison.OrdinalIgnoreCase));
            }
            query=OrderByProp(query,sort,order);
            var total=query.Count();
            var data = query.Skip((p-1)*l).Take(l).ToList();
            return Ok(new
            {
                data,
                meta =new
                {
                    page = p,
                    limit = l,
                    total
                }
            });
        }
        [HttpGet("{id:guid}")]
        public ActionResult<Guest>GetOne(Guid id)
        {
            var user=_guests.FirstOrDefault(a => a.Id == id);
            return user is null ? NotFound(new { error = "User not found", status = 404 }) : Ok(user);
        }
        [HttpPost]
        public ActionResult<Guest> Create([FromBody] CreateUserDto dto)
        {
            if(!ModelState.IsValid) return ValidationProblem(ModelState);
            var user = new Guest
            {
                Id = Guid.NewGuid(),
                FullName = dto.FullName.Trim(),
                Email = dto.Email.Trim(),
                Confirmed = dto.Confirmed
            };
            _guests.Add(user);
            return CreatedAtAction(nameof(GetOne), new {id=user.Id}, user);
        }
        [HttpPut("{id:guid}")]
        public ActionResult<Guest> Update(Guid id, [FromBody] UpdateUserDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var index = _guests.FindIndex(a => a.Id == id);
            if (index == -1)
                return NotFound(new { error = "User Not Found", status = 404 });
            var updated = new Guest
            {
                Id = id,
                FullName = dto.FullName.Trim(),
                Email = dto.Email.Trim(),
                Confirmed = dto.Confirmed
            };
            _guests[index] = updated;
            return Ok(updated);
        }
        [HttpDelete("{id:guid}")]
        public IActionResult Delete(Guid id)
        {
            var removed =_guests.RemoveAll(a=>a.Id==id);
            return removed == 0
                ? NotFound(new {error="user not found", status=404}):NoContent();
                
        }
    }
}