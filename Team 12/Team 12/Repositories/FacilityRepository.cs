using Team_12.DBContext;
using Team_12.Models;
using Team_12.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Team_12.Repositories
{
    public interface IFacilityRepository
    {
        Task<IEnumerable<Facility>> GetAllFacilitiesAsync();
        Task<Facility> GetFacilityByIdAsync(int facilityId);
        Task AddFacilityAsync(Facility facility);
        Task UpdateFacilityAsync(Facility facility);
        Task DeleteFacilityAsync(Facility facility);
        Task AddRatingAsync(Rating rating);
    }
}

public class FacilityRepository : IFacilityRepository
{
    private readonly Team12DbContext _context;

    public FacilityRepository(Team12DbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Facility>> GetAllFacilitiesAsync()
    {
        return await _context.Facilities.Include(f => f.Ratings).ToListAsync();
    }

    public async Task<Facility> GetFacilityByIdAsync(int facilityId)
    {
        return await _context.Facilities.Include(f => f.Ratings)
                                        .FirstOrDefaultAsync(f => f.FacilityId == facilityId);
    }

    public async Task AddFacilityAsync(Facility facility)
    {
        await _context.Facilities.AddAsync(facility);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateFacilityAsync(Facility facility)
    {
        _context.Facilities.Update(facility);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteFacilityAsync(Facility facility)
    {
        _context.Facilities.Remove(facility);
        await _context.SaveChangesAsync();
    }

    public async Task AddRatingAsync(Rating rating)
    {
        await _context.Ratings.AddAsync(rating);
        await _context.SaveChangesAsync();
    }
}
