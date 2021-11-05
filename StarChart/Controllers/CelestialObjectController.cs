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

            foreach (var obj in celestialObjects)
            {
                obj.Satellites = GetRelatedCelestialObjects(obj.Id);
            }

            return Ok(celestialObjects);
        }

        private List<CelestialObject> GetRelatedCelestialObjects(int id) => _context.CelestialObjects.Where(x => x.Id == id).ToList();

        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new { id = celestialObject.Id },celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] CelestialObject celestialObject)
        {
            var obj = _context.CelestialObjects.Where(x => x.Id == id).FirstOrDefault();

            if (obj is null)
            {
                return NotFound();
            }

            obj.Name = celestialObject.Name;
            obj.OrbitalPeriod = celestialObject.OrbitalPeriod;
            obj.OrbitedObjectId = celestialObject.OrbitedObjectId;

            _context.CelestialObjects.Update(obj);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var obj = _context.CelestialObjects.Where(x => x.Id == id).FirstOrDefault();

            if (obj is null)
            {
                return NotFound();
            }

            obj.Name = name;

            _context.CelestialObjects.Update(obj);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var objs = _context.CelestialObjects.Where(x => x.Id == id).ToList();
            var objsOrb = _context.CelestialObjects.Where(x => x.OrbitedObjectId == id).ToList();

            if (!objs.Any() && !objsOrb.Any())
            {
                return NotFound();
            }

            foreach(var celObj in objsOrb)
            {
                celObj.OrbitedObjectId = default;
            }

            _context.CelestialObjects.RemoveRange(objs);
            _context.CelestialObjects.UpdateRange(objsOrb);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
