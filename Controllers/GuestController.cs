using Microsoft.AspNetCore.Mvc;

namespace Recuperatorio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GuestController:ControllerBase
    {

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
            return Ok(new { data, meta = new { page.= p, limit = l, total } });
        }


    }
}
