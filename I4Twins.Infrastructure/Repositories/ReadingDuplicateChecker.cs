using I4Twins.Domain.Entities;
using I4Twins.Domain.Interfaces;
using I4Twins.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace I4Twins.Infrastructure.Repositories;

public class ReadingDuplicateChecker : IReadingDuplicateChecker
{
    private readonly AppDbContext _context;

    public ReadingDuplicateChecker(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsDuplicateAsync(Reading reading)
    {
        return await _context.Readings
            .AnyAsync(r => r.DeviceId == reading.DeviceId
                           && r.Metric == reading.Metric
                           && r.Timestamp == reading.Timestamp
                           && r.Seq == reading.Seq);
    }
}