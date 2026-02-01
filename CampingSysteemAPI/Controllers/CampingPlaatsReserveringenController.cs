using CampingSystem;
using Microsoft.AspNetCore.Mvc;

namespace CampingSysteemAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CampingPlaatsReserveringenController : ControllerBase
    {
        private readonly DAL dal;

        public CampingPlaatsReserveringenController(DAL dal)
        {
            this.dal = dal;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(dal.GetCampingPlaatsReserveringen());
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var reservering = dal.GetCampingPlaatsReservering(id);
            if (reservering == null) return NotFound();
            return Ok(reservering);
        }

        
        [HttpPost]
        public IActionResult Create([FromBody] CampingPlaatsReservering reservering)
        {
            if (reservering == null || reservering.Plaats == null || reservering.Plaats.Id <= 0)
                return BadRequest("Camping plaats reservering met geldig Id is verplicht.");

            dal.CreateCampingPlaatsReservering(reservering);
            return StatusCode(201);
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] CampingPlaatsReservering reservering)
        {
            var existing = dal.GetCampingPlaatsReservering(id);
            if (existing == null) return NotFound();

            existing.Reservering = reservering.Reservering;
            existing.Plaats = reservering.Plaats;
            existing.Tarieven = reservering.Tarieven;
            existing.AantalVolwassenen = reservering.AantalVolwassenen;
            existing.AantalKinderenOnder7 = reservering.AantalKinderenOnder7;
            existing.AantalKinderenOnder12 = reservering.AantalKinderenOnder12;
            existing.AantalHonden = reservering.AantalHonden;

            dal.UpdateCampingPlaatsReservering(existing);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var existing = dal.GetCampingPlaatsReservering(id);
            if (existing == null) return NotFound();

            dal.DeleteCampingPlaatsReservering(existing);
            return NoContent();
        }
    }
}
