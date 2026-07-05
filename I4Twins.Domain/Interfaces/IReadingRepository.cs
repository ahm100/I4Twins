using I4Twins.Domain.Entities;

namespace I4Twins.Domain.Interfaces;

public interface IReadingRepository
{
    Task AddAsync(Reading reading);
    Task<List<Reading>> GetByFilterAsync(string deviceId, string metric, DateTime from, DateTime to);
    Task<int> GetTotalStoredCountAsync();
}