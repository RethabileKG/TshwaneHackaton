using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Team_12.Models;
using Team_12.Repositories;
using Team_12.DTOs;

namespace Team_12.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventController : ControllerBase
    {
        private readonly IEventRepository _eventRepository;

        public EventController(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateEvent([FromBody] EventDTO eventDTO)
        {
            var eventModel = new Event
            {
                Name = eventDTO.Name,
                Description = eventDTO.Description,
                FacilityId = eventDTO.FacilityId,
                StartDate = eventDTO.StartDate,
                EndDate = eventDTO.EndDate,
                EventPrice = eventDTO.EventPrice,
                Capacity = eventDTO.Capacity,
                IsActive = true
            };

            var createdEvent = await _eventRepository.CreateEvent(eventModel);
            return CreatedAtAction(nameof(GetEvent), new { id = createdEvent.EventId }, createdEvent);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetEvent(int id)
        {
            var eventModel = await _eventRepository.GetEventById(id);
            if (eventModel == null)
            {
                return NotFound();
            }
            return Ok(eventModel);
        }

        [HttpPut]
        [Route("update/{id}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] EventDTO eventDTO)
        {
            var existingEvent = await _eventRepository.GetEventById(id);
            if (existingEvent == null)
            {
                return NotFound();
            }

            existingEvent.Name = eventDTO.Name;
            existingEvent.Description = eventDTO.Description;
            existingEvent.FacilityId = eventDTO.FacilityId;
            existingEvent.StartDate = eventDTO.StartDate;
            existingEvent.EndDate = eventDTO.EndDate;
            existingEvent.EventPrice = eventDTO.EventPrice;
            existingEvent.Capacity = eventDTO.Capacity;

            var updatedEvent = await _eventRepository.UpdateEvent(existingEvent);
            return Ok(updatedEvent);
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var result = await _eventRepository.DeleteEvent(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetAllEvents()
        {
            var events = await _eventRepository.GetAllEvents();
            return Ok(events);
        }

        [HttpPost]
        [Route("expire")]
        public async Task<IActionResult> ExpireEvents()
        {
            await _eventRepository.ExpireEvents();
            return Ok("Events expired successfully");
        }
    }
}