using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.Where(x => x.Id == id).FirstOrDefault();

            if (celestialObject is null)
                return NotFound();

            celestialObject.Satellites = GetRelatedCelestialObjects(celestialObject.Id);

            return Ok(celestialObject);
        }

        [HttpGet("{name}", Name = "GetByName")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects.Where(x => x.Name == name).ToList();

            if (celestialObjects is null || !celestialObjects.Any())
                return NotFound();

            foreach (var obj in celestialObjects) 
            {
                obj.Satellites = GetRelatedCelestialObjects(obj.Id);
            }
            return Ok(celestialObjects);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects.ToList();

            foreach(var obj in celestialObjects)
            {
                obj.Satellites = GetRelatedCelestialObjects(obj.Id);
            }

            return Ok(celestialObjects);
        }

        private List<CelestialObject> GetRelatedCelestialObjects(int id) => _context.CelestialObjects.Where(x => x.Id == id).ToList();
    }
}
