using I4Twins.Application.Dtos;
using I4Twins.Domain.Enums;
using I4Twins.Domain.Interfaces;

namespace I4Twins.Application.Services;

public class AggregationService : IAggregationService
{
    private readonly IReadingRepository _repository;

    public AggregationService(IReadingRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// دریافت داده‌های aggregated برای یک دستگاه و متریک خاص در بازه زمانی مشخص
    /// </summary>
    /// <param name="deviceId">شناسه دستگاه (مثلاً PUMP-01)</param>
    /// <param name="metric">نوع متریک (مثلاً temperature, pressure, vibration)</param>
    /// <param name="from">شروع بازه زمانی (UTC)</param>
    /// <param name="to">پایان بازه زمانی (UTC)</param>
    /// <param name="bucketSizeSeconds">اندازه هر باکت زمانی بر حسب ثانیه (مثلاً 60 برای ۱ دقیقه، 3600 برای ۱ ساعت)</param>
    /// <returns>
    /// لیستی از باکت‌های زمانی با اطلاعات aggregated شامل:
    /// - BucketStart: زمان شروع باکت
    /// - Count: تعداد رکوردها در باکت
    /// - Avg: میانگین مقادیر
    /// - Min: کمترین مقدار
    /// - Max: بیشترین مقدار
    /// </returns>    
    public async Task<List<AggregationBucketDto>> GetAggregatedAsync(
        string deviceId, MetricType metric, DateTime from, DateTime to, int bucketSizeSeconds)
    {
        var readings = await _repository.GetByFilterAsync(deviceId, metric, from, to);

        if (!readings.Any())
            return new List<AggregationBucketDto>();

        var result = new List<AggregationBucketDto>();

        // پیدا کردن اولین و آخرین بازه
        var firstReading = readings.First();
        var start = GetBucketStart(firstReading.Timestamp, bucketSizeSeconds);
        var end = GetBucketStart(to, bucketSizeSeconds); // شروع آخرین باکت

        for (var bucketStart = start; bucketStart <= end; bucketStart = bucketStart.AddSeconds(bucketSizeSeconds))
        {
            var bucketEnd = bucketStart.AddSeconds(bucketSizeSeconds);

            var itemsInBucket = readings
                .Where(r => r.Timestamp >= bucketStart && r.Timestamp < bucketEnd)
                .ToList();

            // تصمیم: حذف باکت‌های خالی
            if (!itemsInBucket.Any())
                continue;

            result.Add(new AggregationBucketDto
            {
                BucketStart = bucketStart,
                Count = itemsInBucket.Count,
                Min = itemsInBucket.Min(r => r.Value),
                Max = itemsInBucket.Max(r => r.Value),
                Avg = itemsInBucket.Average(r => r.Value)
            });
        }

        return result;
    }


    /// <summary>
    /// محاسبه شروع باکت برای یک زمان مشخص
    /// </summary>
    /// <param name="timestamp">زمان ورودی (مثلاً 08:33:45)</param>
    /// <param name="bucketSizeSeconds">اندازه هر باکت به ثانیه (مثلاً 60 برای ۱ دقیقه)</param>
    /// <returns>شروع باکت (مثلاً 08:33:00)</returns>
    /// <example>
    /// اگر timestamp = 08:33:45 و bucketSizeSeconds = 60
    /// خروجی: 08:33:00
    /// </example>
    private DateTime GetBucketStart(DateTime timestamp, int bucketSizeSeconds)
    {
        var secondsSinceMidnight = (int)(timestamp - timestamp.Date).TotalSeconds;
        var bucketOffset = (secondsSinceMidnight / bucketSizeSeconds) * bucketSizeSeconds;
        // or bad way by converting to double
        //(int)Math.Floor(seconds / (double)bucketSize) * bucketSize
        return timestamp.Date.AddSeconds(bucketOffset);
    }
}