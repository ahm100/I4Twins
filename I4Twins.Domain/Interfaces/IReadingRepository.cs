using I4Twins.Domain.Entities;
using I4Twins.Domain.Enums;

namespace I4Twins.Domain.Interfaces;

public interface IReadingRepository
{
    Task AddAsync(Reading reading);
    Task<List<Reading>> GetByFilterAsync(string deviceId, MetricType metric, DateTime from, DateTime to);
    Task<int> GetTotalStoredCountAsync();
}