using CampingSystem;
using Microsoft.AspNetCore.Mvc;

namespace CampingSysteemAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CampingPlaatsTarievenController : ControllerBase
    {
        private readonly DAL dal;

        public CampingPlaatsTarievenController(DAL dal)
        {
            this.dal = dal;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(dal.GetCampingPlaatsTarieven());
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var tarief = dal.GetCampingPlaatsTarieven(id);
            if (tarief == null) return NotFound();
            return Ok(tarief);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CampingPlaatsTarieven t)
        {
            if (t == null) return BadRequest();

            dal.CreateCampingPlaatsTarieven(t);
            return StatusCode(201, t); // t heeft nu Id gekregen
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] CampingPlaatsTarieven t)
        {
            var existing = dal.GetCampingPlaatsTarieven(id);
            if (existing == null) return NotFound();

            // kopieer velden (zelfde stijl als jouw andere controllers)
            existing.Type = t.Type;
            existing.GeldigVan = t.GeldigVan;
            existing.GeldigTot = t.GeldigTot;
            existing.TariefVolwassenen = t.TariefVolwassenen;
            existing.TariefKinderenOnder7 = t.TariefKinderenOnder7;
            existing.TariefKinderenOnder12 = t.TariefKinderenOnder12;
            existing.TariefHonden = t.TariefHonden;
            existing.TariefElectriciteit = t.TariefElectriciteit;

            dal.UpdateCampingPlaatsTarieven(existing);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var existing = dal.GetCampingPlaatsTarieven(id);
            if (existing == null) return NotFound();

            dal.DeleteCampingPlaatsTarieven(id);
            return NoContent();
        }
    }
}


