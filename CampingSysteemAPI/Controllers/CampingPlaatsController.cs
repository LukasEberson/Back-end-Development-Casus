using CampingSystem;
using Microsoft.AspNetCore.Mvc;

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
    }
}
