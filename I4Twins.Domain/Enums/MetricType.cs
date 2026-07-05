namespace I4Twins.Domain.Enums;

/// <summary>
/// انواع متریک‌های پشتیبانی‌شده
/// </summary>
public enum MetricType
{
    /// <summary>
    /// دما (درجه سانتی‌گراد یا فارنهایت)
    /// </summary>
    Temperature = 1,

    /// <summary>
    /// فشار (بار یا PSI)
    /// </summary>
    Pressure = 2,

    /// <summary>
    /// لرزش (هرتز یا میلی‌متر بر ثانیه)
    /// </summary>
    Vibration = 3
}