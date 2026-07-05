namespace I4Twins.Domain.Entities;

public class Reading
{
    public int Id { get; private set; }
    public string DeviceId { get; private set; }
    public string Metric { get; private set; }
    public DateTime Timestamp { get; private set; }
    public double Value { get; private set; }
    public long Seq { get; private set; }

    public Reading(string deviceId, string metric, DateTime timestamp, double value, long seq)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
            throw new ArgumentException("DeviceId is required.", nameof(deviceId));
        if (string.IsNullOrWhiteSpace(metric))
            throw new ArgumentException("Metric is required.", nameof(metric));
        if (timestamp.Kind != DateTimeKind.Utc)
            throw new ArgumentException("Timestamp must be UTC.", nameof(timestamp));
        if (double.IsNaN(value) || double.IsInfinity(value))
            throw new ArgumentException("Value is invalid.", nameof(value));
        if (seq < 0)
            throw new ArgumentException("Seq must be positive.", nameof(seq));

        DeviceId = deviceId;
        Metric = metric;
        Timestamp = timestamp;
        Value = value;
        Seq = seq;
    }

  
    public string GetIdentityKey() => $"{DeviceId}_{Metric}_{Timestamp:O}_{Seq}";
}