using I4Twins.Application.Dtos;
using I4Twins.Application.Services;
using I4Twins.Domain.Entities;
using I4Twins.Domain.Enums;
using I4Twins.Domain.Interfaces;
using Moq;

namespace I4Twins.Tests;

public class AggregationTests
{
    [Fact]
    public async Task GetAggregatedAsync_ShouldCalculateCorrectAggregates()
    {
        // Arrange
        var deviceId = "PUMP-01";
        var metric = MetricType.Temperature;
        var from = new DateTime(2025, 6, 1, 8, 0, 0, DateTimeKind.Utc);
        var to = new DateTime(2025, 6, 1, 9, 0, 0, DateTimeKind.Utc);
        var bucketSize = 60;

        var readings = new List<Reading>
        {
            new Reading(deviceId, metric, new DateTime(2025, 6, 1, 8, 10, 0, DateTimeKind.Utc), 10, 1),
            new Reading(deviceId, metric, new DateTime(2025, 6, 1, 8, 20, 0, DateTimeKind.Utc), 20, 2),
            new Reading(deviceId, metric, new DateTime(2025, 6, 1, 8, 30, 0, DateTimeKind.Utc), 30, 3),
        };

        // 1. ساختن Mock از IReadingRepository
        var mockRepo = new Mock<IReadingRepository>();

        // 2. تنظیم Mock
        mockRepo.Setup(r => r.GetByFilterAsync(deviceId, metric, from, to))
                .ReturnsAsync(readings);

        // 3. تزریق Mock به سازنده AggregationService
        var service = new AggregationService(mockRepo.Object);

        // Act
        var result = await service.GetAggregatedAsync(deviceId, metric, from, to, bucketSize);

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal(10, result[0].Min);
        Assert.Equal(10, result[0].Max);
        Assert.Equal(10, result[0].Avg);
    }
}