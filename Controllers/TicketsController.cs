using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Recuperatorio.Controllers
{

    [ApiController]
    [Route("api/v1/{controller}")]
    public class TicketsController:ControllerBase
    {
        private static readonly List<Ticket> _tickets = new()
        {
            new Ticket{Id=Guid.NewGuid(),GuestId=Guid.NewGuid(),EventId=Guid.NewGuid(),Type="general",Price= 10,Status="valid",Notes="sfnskjgn"},
            new Ticket{Id=Guid.NewGuid(),GuestId=Guid.NewGuid(),EventId=Guid.NewGuid(),Type="general",Price= 10,Status="valid",Notes="sfnskjgn"},
            new Ticket{Id=Guid.NewGuid(),GuestId=Guid.NewGuid(),EventId=Guid.NewGuid(),Type="vip",Price= 100,Status="valid",Notes="sfnskjgn"},
            new Ticket{Id=Guid.NewGuid(),GuestId=Guid.NewGuid(),EventId=Guid.NewGuid(),Type="backstage",Price= 50,Status="valid",Notes="sfnskjgn"},
        };

        [HttpGet("{id:guid}")]
        public IActionResult GetOne(Guid id)
        {
            var ticket = _tickets.FirstOrDefault(b => b.Id == id);
            return ticket is null
                ? NotFound(new { error = "Ticket not found", status = 404 })
                : Ok(ticket);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreatedTicketDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var ticket = new Ticket
            { 
                Id= Guid.NewGuid(),
                GuestId= Guid.NewGuid(),
                EventId= Guid.NewGuid(),
                Type=dto.Type.Trim(),
                Price=dto.Price,
                Status=dto.Status.Trim(),
                Notes=dto.Notes.Trim()
         
            
            };
            _tickets.Add(ticket);
            return CreatedAtAction(nameof(GetOne), new { id = ticket.Id }, ticket);


        }

        [HttpPut("{id:guid}")]
        public IActionResult Update(Guid id, [FromBody] UpdateTicketDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var index = _tickets.FindIndex(b => b.Id == id);
            if (index == -1)
                return NotFound(new { error = "Ticket not found", status = 404 });
            var updated = new Ticket
            {
                Id = id,
                GuestId = Guid.NewGuid(),
                EventId = Guid.NewGuid(),
                Type = dto.Type.Trim(),
                Price = dto.Price,
                Status = dto.Status.Trim(),
                Notes = dto.Notes.Trim()


            };

            _tickets[index]=updated;
            return Ok(updated);

        }

        [HttpDelete("{id:guid}")]
        public IActionResult Delete(Guid id)
        {
            var removed = -_tickets.RemoveAll(b => b.Id == id);
            return removed == 0
                ? NotFound(new { error = "Ticket not Found", status = 404 })
                : NoContent();

        }




    }
}
