using Microsoft.AspNetCore.Mvc;
using Team_12.Repositories;
using Team_12.Models;
using Team_12.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Claims;

namespace Team_12.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacilityController : ControllerBase
    {
        private readonly IFacilityRepository _facilityRepository;

        public FacilityController(IFacilityRepository facilityRepository)
        {
            _facilityRepository = facilityRepository;
        }

        // GET: api/Facility
        [HttpGet]
        public async Task<IActionResult> GetFacilities()
        {
            var facilities = await _facilityRepository.GetAllFacilitiesAsync();
            return Ok(facilities);
        }

        // GET: api/Facility/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFacility(int id)
        {
            var facility = await _facilityRepository.GetFacilityByIdAsync(id);
            if (facility == null)
            {
                return NotFound();
            }
            return Ok(facility);
        }

        // POST: api/Facility
        [HttpPost]
        public async Task<IActionResult> CreateFacility([FromBody] FacilityDto facilityDto)
        {
            var facility = new Facility
            {
                Name = facilityDto.Name,
                Description = facilityDto.Description,
                Type = facilityDto.Type,
                PricePerHour = facilityDto.PricePerHour,
                Capacity = facilityDto.Capacity,
                Address = facilityDto.Address,
                ImageURL = facilityDto.ImageURL
            };

            await _facilityRepository.AddFacilityAsync(facility);
            return CreatedAtAction(nameof(GetFacility), new { id = facility.FacilityId }, facility);
        }

        // PUT: api/Facility/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFacility(int id, [FromBody] FacilityDto facilityDto)
        {
            var facility = await _facilityRepository.GetFacilityByIdAsync(id);
            if (facility == null)
            {
                return NotFound();
            }

            facility.Name = facilityDto.Name;
            facility.Description = facilityDto.Description;
            facility.Type = facilityDto.Type;
            facility.PricePerHour = facilityDto.PricePerHour;
            facility.Capacity = facilityDto.Capacity;
            facility.Address = facilityDto.Address;
            facility.ImageURL = facilityDto.ImageURL;

            await _facilityRepository.UpdateFacilityAsync(facility);
            return NoContent();
        }

        // DELETE: api/Facility/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFacility(int id)
        {
            var facility = await _facilityRepository.GetFacilityByIdAsync(id);
            if (facility == null)
            {
                return NotFound();
            }

            await _facilityRepository.DeleteFacilityAsync(facility);
            return NoContent();
        }

        // POST: api/Facility/{id}/rate
        [HttpPost("{id}/rate")]
        [Authorize] // Ensure that only logged-in users can rate
        public async Task<IActionResult> RateFacility(int id, [FromBody] RatingDto ratingDto)
        {
            var facility = await _facilityRepository.GetFacilityByIdAsync(id);
            if (facility == null)
            {
                return NotFound();
            }

            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value; // Get User ID from JWT token

            var rating = new Rating
            {
                FacilityId = id,
                UserId = userId,
                Stars = ratingDto.Stars,
                Comments = ratingDto.Comments,
                Date = DateTime.Now
            };

            await _facilityRepository.AddRatingAsync(rating);
            return Ok();
        }
    }
}

