namespace I4Twins.Application.Dtos;

public class AggregationBucketDto
{
    public DateTime BucketStart { get; set; }
    public int Count { get; set; }
    public double Avg { get; set; }
    public double Min { get; set; }
    public double Max { get; set; }
}