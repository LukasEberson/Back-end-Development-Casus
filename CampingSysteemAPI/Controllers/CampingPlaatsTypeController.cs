using CampingSystem;
using Microsoft.AspNetCore.Mvc;

namespace CampingSysteemAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CampingPlaatsTypeController : ControllerBase
    {
        private readonly DAL dal;

        public CampingPlaatsTypeController(DAL dal)
        {
            this.dal = dal;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(dal.GetCampingPlaatsTypen());
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var type = dal.GetCampingPlaatsType(id);
            if (type == null) return NotFound();
            return Ok(type);
        }

        [HttpPost]
        public IActionResult Create()
        {
            dal.CreateCampingPlaatsType(new CampingPlaatsType());
            return StatusCode(201);
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var existing = dal.GetCampingPlaatsType(id);
            if (existing == null) return NotFound();

            dal.DeleteCampingPlaatsType(existing);
            return NoContent();
        }

    }
}
