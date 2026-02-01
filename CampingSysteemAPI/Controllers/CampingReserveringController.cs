using CampingSystem;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CampingSysteemAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CampingReserveringController : ControllerBase
    {
        private readonly DAL dal;

        public CampingReserveringController(DAL dal)
        {
            this.dal = dal;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(dal.GetCampingReserveringen());
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var res = dal.GetCampingReservering(id);
            if (res == null) return NotFound();
            return Ok(res);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CampingReservering r)
        {
            if (r == null) return BadRequest();

            dal.CreateCampingReservering(r);
            return StatusCode(201, r); // r heeft nu Id gekregen
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] CampingReservering r)
        {
            var existing = dal.GetCampingReservering(id);
            if (existing == null) return NotFound();

            existing.Rekening = r.Rekening;
            existing.Naam = r.Naam;
            existing.Emailadres = r.Emailadres;
            existing.Telefoonnummer = r.Telefoonnummer;
            existing.BeginDatum = r.BeginDatum;
            existing.EindDatum = r.EindDatum;

            dal.UpdateCampingReservering(existing);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var existing = dal.GetCampingReservering(id);
            if (existing == null) return NotFound();
            if (!existing.PlaatsReserveringen.IsNullOrEmpty()) return BadRequest("Deze camping reservering is verbonden aan plaats reserveringen!");

            dal.DeleteCampingReservering(id);
            return NoContent();
        }
    }
}

