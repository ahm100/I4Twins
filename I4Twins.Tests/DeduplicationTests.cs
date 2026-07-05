using I4Twins.Domain.Entities;
using I4Twins.Domain.Enums;

namespace I4Twins.Tests;

public class DeduplicationTests
{
    [Fact]
    public void TwoReadingsWithSameIdentity_ShouldHaveSameKey()
    {
        // Arrange
        var timestamp = DateTime.UtcNow;
        var reading1 = new Reading("PUMP-01", MetricType.Temperature, timestamp, 67.21, 1199);
        var reading2 = new Reading("PUMP-01", MetricType.Temperature, timestamp, 68.50, 1199); // مقدار متفاوت ولی کلید یکسان

        // Act & Assert
        Assert.Equal(reading1.GetIdentityKey(), reading2.GetIdentityKey());
    }

    [Fact]
    public void TwoReadingsWithDifferentSeq_ShouldHaveDifferentKey()
    {
        // Arrange
        var timestamp = DateTime.UtcNow;
        var reading1 = new Reading("PUMP-01", MetricType.Temperature, timestamp, 67.21, 1199);
        var reading2 = new Reading("PUMP-01", MetricType.Temperature, timestamp, 67.21, 1200);

        // Act & Assert
        Assert.NotEqual(reading1.GetIdentityKey(), reading2.GetIdentityKey());
    }

    [Fact]
    public void TwoReadingsWithDifferentDevice_ShouldHaveDifferentKey()
    {
        // Arrange
        var timestamp = DateTime.UtcNow;
        var reading1 = new Reading("PUMP-01", MetricType.Temperature, timestamp, 67.21, 1199);
        var reading2 = new Reading("PUMP-02", MetricType.Temperature, timestamp, 67.21, 1199);

        // Act & Assert
        Assert.NotEqual(reading1.GetIdentityKey(), reading2.GetIdentityKey());
    }
}