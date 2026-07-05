namespace I4Twins.Application.Services;

public interface IIngestionService
{
    Task ProcessFileAsync(string filePath);
    int TotalLines { get; }
    int StoredCount { get; }
    int DuplicateCount { get; }
    int InvalidCount { get; }
}