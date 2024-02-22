using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CitiesManager.Core.Entities;
using CitiesManager.Infrastructure.DatabaseContext;

namespace CitiesManager.WebAPI.Controllers.v1
{
    /// <summary>
    /// 
    /// </summary>
    [ApiVersion("1.0")]
    public class CitiesController : CustomControllerBase
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public CitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Cities
        /// <summary>
        /// To get list of cities (including city ID and city name)
        /// from 'cities' table
        /// </summary>
        /// <returns>Returns all of the Cities present in the database</returns>
        [HttpGet]
        // [Produces("application/xml")]
        public async Task<ActionResult<IEnumerable<City>>> GetCities()
        {
            return await _context.Cities
                .OrderByDescending(city => city.CityName)
                .ToListAsync();
        }

        // GET: api/Cities/5
        /// <summary>
        /// Get's a particular city (including city ID and city name)
        /// from the 'cities' table based on the provided cityID
        /// </summary>
        /// <param name="cityID"></param>
        /// <returns></returns>
        [HttpGet("{cityID}")]
        public async Task<ActionResult<City>> GetCity(Guid cityID)
        {
            var city = await _context.Cities.FirstOrDefaultAsync(c => c.CityID == cityID);

            if (city == null)
            {
                return Problem(detail: "Invalid city ID", statusCode: 404, title: "City Not Found");
            }

            return city; // Ok(city) - OkObjectResult(city)
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        [HttpPost("queryString")]
        public async Task<ActionResult<City>> AddCityWithQueryString([FromQuery] City city)
        {
            bool exists = await _context.Cities.AnyAsync(c => c.CityID == city.CityID);

            if (exists)
            {
                return Conflict(string.Format("The City with the ID {0} already exists in the Database", city.CityID));
            }

            _context.Cities.Add(city);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCity", new { cityID = city.CityID }, city);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        [HttpPost("formData")]
        public async Task<ActionResult<City>> AddCityWithFormData([FromForm] City city)
        {
            bool exists = await _context.Cities.AnyAsync(c => c.CityID == city.CityID);

            if (exists)
            {
                return Conflict(string.Format("The City with the ID {0} already exists in the Database", city.CityID));
            }

            _context.Cities.Add(city);

            await _context.SaveChangesAsync();


            return CreatedAtAction("GetCity", new { cityID = city.CityID }, city);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cityID"></param>
        /// <param name="city"></param>
        /// <returns></returns>
        // PUT: api/Cities/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{cityID}")]
        public async Task<IActionResult> PutCity(Guid cityID, [Bind(nameof(City.CityID),nameof(City.CityName))]
        City city)
        {
            if (cityID != city.CityID)
            {
                return BadRequest();
            }

            //_context.Entry(city).State = EntityState.Modified;

            var get_city = await _context.Cities.FindAsync(cityID);

            if (get_city == null)
            {
                return NotFound(get_city);
            }

            get_city.CityName = city.CityName;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CityExists(cityID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        // POST: api/Cities
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<City>> PostCity([Bind(nameof(City.CityID),nameof(City.CityName))]
        City city)
        {
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCity", new { cityID = city.CityID }, city);
            // api/Cities/{cityID}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cityID"></param>
        /// <returns></returns>
        // DELETE: api/Cities/5
        [HttpDelete("{cityID}")]
        public async Task<IActionResult> DeleteCity(Guid cityID)
        {
            var city = await _context.Cities.FindAsync(cityID);
            if (city == null)
            {
                return NotFound();
            }

            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CityExists(Guid id)
        {
            return _context.Cities.Any(e => e.CityID == id);
        }
    }
}
