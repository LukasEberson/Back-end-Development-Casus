using CampingSystem;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CampingSysteemAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CampingPlaatsController : ControllerBase
    {
        private readonly DAL dal;

        public CampingPlaatsController(DAL dal)
        {
            this.dal = dal;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var plaatsen = dal.GetCampingPlaatsen();
            return Ok(plaatsen);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var plaats = dal.GetCampingPlaats(id);
            if (plaats == null) return NotFound();
            return Ok(plaats);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CampingPlaats plaats)
        {
            if (plaats == null || plaats.Type == null || plaats.Type.Id <= 0)
                return BadRequest("Camping plaats met geldig Id is verplicht.");

            dal.CreateCampingPlaats(plaats);
            return StatusCode(201);
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] CampingPlaats plaats)
        {
            var existing = dal.GetCampingPlaats(id);
            if (existing == null) return NotFound();

            existing.Type = plaats.Type;
            existing.Nummer = plaats.Nummer;
            existing.Reserveringen = plaats.Reserveringen;

            dal.UpdateCampingPlaats(existing);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var existing = dal.GetCampingPlaats(id);
            if (existing == null) return NotFound();
            if (!existing.Reserveringen.IsNullOrEmpty()) return BadRequest("Deze camping plaats is verbonden aan reserveringen!");

            dal.DeleteCampingPlaats(existing);
            return NoContent();
        }
    }
}
