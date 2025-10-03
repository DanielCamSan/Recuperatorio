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

      
    }  
}
