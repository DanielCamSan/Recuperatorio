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
        
    }
}
