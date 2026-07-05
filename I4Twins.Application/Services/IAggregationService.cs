using I4Twins.Application.Dtos;

namespace I4Twins.Application.Services;

public interface IAggregationService
{
    Task<List<AggregationBucketDto>> GetAggregatedAsync(
        string deviceId, string metric, DateTime from, DateTime to, int bucketSizeSeconds);
}