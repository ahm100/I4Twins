using I4Twins.Domain.Entities;
using I4Twins.Domain.Interfaces;
using I4Twins.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace I4Twins.Infrastructure.Repositories;

public class ReadingRepository : IReadingRepository
{
    private readonly AppDbContext _context;

    public ReadingRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Reading reading)
    {
        try
        {
            await _context.Readings.AddAsync(reading);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("IX_Reading_Unique") == true)
        {
            throw new InvalidOperationException("Duplicate reading detected.", ex);
        }
    }

    public async Task<List<Reading>> GetByFilterAsync(string deviceId, string metric, DateTime from, DateTime to)
    {
        return await _context.Readings
            .Where(r => r.DeviceId == deviceId
                        && r.Metric == metric
                        && r.Timestamp >= from
                        && r.Timestamp <= to)
            .OrderBy(r => r.Timestamp)
            .ToListAsync();
    }

    public async Task<int> GetTotalStoredCountAsync()
    {
        return await _context.Readings.CountAsync();
    }
}