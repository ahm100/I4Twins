using I4Twins.Application.Services;
using I4Twins.Domain.Enums;
using I4Twins.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace I4Twins.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReadingsController : ControllerBase
{
    private readonly IAggregationService _aggregationService;

    public ReadingsController(IAggregationService aggregationService)
    {
        _aggregationService = aggregationService;
    }

    /// <summary>
    /// دریافت داده‌های aggregated برای یک دستگاه و متریک خاص
    /// </summary>
    /// <param name="deviceId">شناسه دستگاه (مثلاً PUMP-01)</param>
    /// <param name="metric">نوع متریک (Temperature, Pressure, Vibration)</param>
    /// <param name="from">زمان شروع بازه به صورت ISO-8601 UTC (مثلاً 2025-06-01T08:00:00Z)</param>
    /// <param name="to">زمان پایان بازه به صورت ISO-8601 UTC (مثلاً 2025-06-01T09:00:00Z)</param>
    /// <param name="bucketSizeSeconds">اندازه هر باکت بر حسب ثانیه (پیش‌فرض: ۶۰ ثانیه = ۱ دقیقه، حداکثر: ۸۶۴۰۰ ثانیه = ۲۴ ساعت)</param>
    /// <returns>
    /// لیستی از باکت‌های زمانی با اطلاعات aggregated شامل:
    /// - bucketStart: زمان شروع باکت (UTC)
    /// - count: تعداد رکوردها در باکت
    /// - avg: میانگین مقادیر
    /// - min: کمترین مقدار
    /// - max: بیشترین مقدار
    /// 
    /// باکت‌های خالی از پاسخ حذف می‌شوند.
    /// </returns>
    /// <response code="200">درخواست موفق و داده‌ها برگردانده شدند</response>
    /// <response code="400">پارامترهای ورودی نامعتبر هستند</response>
    /// <example>
    /// درخواست:
    /// GET /api/readings/aggregate?deviceId=PUMP-01&amp;metric=temperature&amp;from=2025-06-01T08:00:00Z&amp;to=2025-06-01T09:00:00Z&amp;bucketSizeSeconds=60
    /// 
    /// پاسخ:
    /// [
    ///   {
    ///     "bucketStart": "2025-06-01T08:00:00Z",
    ///     "count": 5,
    ///     "avg": 67.5,
    ///     "min": 65.0,
    ///     "max": 70.0
    ///   },
    ///   {
    ///     "bucketStart": "2025-06-01T08:01:00Z",
    ///     "count": 3,
    ///     "avg": 68.2,
    ///     "min": 66.0,
    ///     "max": 69.5
    ///   }
    /// ]
    /// </example>
    [HttpGet("aggregate")]
    public async Task<IActionResult> GetAggregate(
        [FromQuery] string deviceId,
        [FromQuery] MetricType metric,
        [FromQuery] string from,
        [FromQuery] string to,
        [FromQuery] int bucketSizeSeconds = 60)
    {
        // اعتبارسنجی پارامترها
        if (string.IsNullOrWhiteSpace(deviceId))
            return BadRequest("deviceId is required.");

        if (!Enum.IsDefined(typeof(MetricType), metric))
            return BadRequest($"Invalid metric value. Allowed values: {string.Join(", ", Enum.GetNames<MetricType>())}");

        if (!DateTime.TryParse(from, null, System.Globalization.DateTimeStyles.RoundtripKind, out var fromDate))
            return BadRequest("Invalid 'from' date format. Use ISO-8601 UTC (e.g., 2025-06-01T08:33:00Z).");

        if (!DateTime.TryParse(to, null, System.Globalization.DateTimeStyles.RoundtripKind, out var toDate))
            return BadRequest("Invalid 'to' date format. Use ISO-8601 UTC (e.g., 2025-06-01T08:33:00Z).");

        if (fromDate >= toDate)
            return BadRequest("'from' must be less than 'to'.");

        if (bucketSizeSeconds <= 0 || bucketSizeSeconds > 86400)
            return BadRequest("bucketSizeSeconds must be between 1 and 86400.");

        var result = await _aggregationService.GetAggregatedAsync(
            deviceId, metric, fromDate.ToUniversalTime(), toDate.ToUniversalTime(), bucketSizeSeconds);

        return Ok(result);
    }

    [HttpGet("debug")]
    public async Task<IActionResult> Debug()
    {
        using var scope = HttpContext.RequestServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var all = await dbContext.Readings.ToListAsync();
        var count = await dbContext.Readings.CountAsync();

        return Ok(new
        {
            total = count,
            readings = all.Take(10)  // فقط ۱۰ تا اول
        });
    }

}