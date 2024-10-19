using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Team_12.DBContext;
using Team_12.Models;

namespace Team_12.Repositories
{
    public interface IEventRepository
    {
        Task<Event> CreateEvent(Event eventModel);
        Task<Event> GetEventById(int eventId);
        Task<IEnumerable<Event>> GetAllEvents();
        Task<Event> UpdateEvent(Event eventModel);
        Task<bool> DeleteEvent(int eventId);
        Task ExpireEvents();
    }

    public class EventRepository : IEventRepository
    {
        private readonly Team12DbContext _context;

        public EventRepository(Team12DbContext context)
        {
            _context = context;
        }

        public async Task<Event> CreateEvent(Event eventModel)
        {
            _context.Events.Add(eventModel);
            await _context.SaveChangesAsync();
            return eventModel;
        }

        public async Task<Event> GetEventById(int eventId)
        {
            return await _context.Events
                .Include(e => e.Facility)
                .FirstOrDefaultAsync(e => e.EventId == eventId);
        }

        public async Task<IEnumerable<Event>> GetAllEvents()
        {
            return await _context.Events
                .Include(e => e.Facility)
                .ToListAsync();
        }

        public async Task<Event> UpdateEvent(Event eventModel)
        {
            _context.Entry(eventModel).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return eventModel;
        }

        public async Task<bool> DeleteEvent(int eventId)
        {
            var eventModel = await _context.Events.FindAsync(eventId);
            if (eventModel == null)
                return false;

            _context.Events.Remove(eventModel);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task ExpireEvents()
        {
            var expiredEvents = await _context.Events
                .Where(e => e.EndDate < DateTime.Now && e.IsActive)
                .ToListAsync();

            foreach (var eventModel in expiredEvents)
            {
                eventModel.IsActive = false;
            }

            await _context.SaveChangesAsync();
        }
    }
}