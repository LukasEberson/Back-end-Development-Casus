using CampingSystem;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CampingSysteemAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CampingRekeningController : ControllerBase
    {
        private readonly DAL dal;

        public CampingRekeningController(DAL dal)
        {
            this.dal = dal;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(dal.GetCampingRekeningen());
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var r = dal.GetCampingRekening(id);
            if (r == null) return NotFound();
            return Ok(r);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CampingRekening r)
        {
            if (r == null) return BadRequest();

            dal.CreateCampingRekening(r);
            return StatusCode(201, r);
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] CampingRekening r)
        {
            var existing = dal.GetCampingRekening(id);
            if (existing == null) return NotFound();

            existing.ToeristenBelasting = r.ToeristenBelasting;
            existing.Korting = r.Korting;
            existing.Betaald = r.Betaald;

            dal.UpdateCampingRekening(existing);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var existing = dal.GetCampingRekening(id);
            if (existing == null) return NotFound();
            if (!existing.Reserveringen.IsNullOrEmpty()) return BadRequest("Deze camping rekening is verbonden aan reserveringen!");

            dal.DeleteCampingRekening(id);
            return NoContent();
        }
    }
}

