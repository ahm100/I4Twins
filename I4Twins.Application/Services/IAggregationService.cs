using I4Twins.Application.Dtos;
using I4Twins.Domain.Enums;

namespace I4Twins.Application.Services;

public interface IAggregationService
{
    Task<List<AggregationBucketDto>> GetAggregatedAsync(
        string deviceId, MetricType metric, DateTime from, DateTime to, int bucketSizeSeconds);
}