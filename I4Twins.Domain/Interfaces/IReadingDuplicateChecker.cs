using I4Twins.Domain.Entities;

namespace I4Twins.Domain.Interfaces;

/// <summary>
/// Domain service interface for checking duplicate readings.
/// Business rule: Two readings are identical if they have the same (deviceId, metric, ts, seq).
/// </summary>
public interface IReadingDuplicateChecker
{
    Task<bool> IsDuplicateAsync(Reading reading);
}