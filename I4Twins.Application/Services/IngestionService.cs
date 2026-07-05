using I4Twins.Domain.Entities;
using I4Twins.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace I4Twins.Application.Services;

public class IngestionService : IIngestionService
{
    private readonly IReadingRepository _repository;
    private readonly IReadingDuplicateChecker _duplicateChecker;
    private readonly ILogger<IngestionService> _logger;

    public int TotalLines { get; private set; }
    public int StoredCount { get; private set; }
    public int DuplicateCount { get; private set; }
    public int InvalidCount { get; private set; }

    public IngestionService(
        IReadingRepository repository,
        IReadingDuplicateChecker duplicateChecker,
        ILogger<IngestionService> logger)
    {
        _repository = repository;
        _duplicateChecker = duplicateChecker;
        _logger = logger;
    }

    public async Task ProcessFileAsync(string filePath)
    {
        _logger.LogInformation("Starting ingestion from {FilePath}", filePath);

        TotalLines = 0;
        StoredCount = 0;
        DuplicateCount = 0;
        InvalidCount = 0;

        if (!File.Exists(filePath))
        {
            _logger.LogError("File not found: {FilePath}", filePath);
            return;
        }

        var lines = await File.ReadAllLinesAsync(filePath);
        TotalLines = lines.Length;

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                InvalidCount++;
                continue;
            }

            try
            {
                using var jsonDoc = JsonDocument.Parse(line);
                var root = jsonDoc.RootElement;

                var deviceId = root.GetProperty("deviced").GetString() ?? throw new JsonException("deviced is null");
                var metric = root.GetProperty("metric").GetString() ?? throw new JsonException("metric is null");
                var tsString = root.GetProperty("ts").GetString() ?? throw new JsonException("ts is null");
                var value = root.GetProperty("value").GetDouble();
                var seq = root.GetProperty("seq").GetInt64();

                if (!DateTime.TryParse(tsString, null, System.Globalization.DateTimeStyles.RoundtripKind, out var timestamp))
                {
                    throw new FormatException($"Invalid timestamp format: {tsString}");
                }

                var reading = new Reading(deviceId, metric, timestamp.ToUniversalTime(), value, seq);

                if (await _duplicateChecker.IsDuplicateAsync(reading))
                {
                    DuplicateCount++;
                    _logger.LogWarning("Duplicate reading ignored: {IdentityKey}", reading.GetIdentityKey());
                    continue;
                }

                await _repository.AddAsync(reading);
                StoredCount++;
            }
            catch (Exception ex)
            {
                InvalidCount++;
                _logger.LogError(ex, "Invalid record rejected: {Line}", line.Length > 100 ? line[..100] + "..." : line);
            }
        }

        _logger.LogInformation(
            "Ingestion completed. Total: {Total}, Stored: {Stored}, Duplicates: {Duplicates}, Invalid: {Invalid}",
            TotalLines, StoredCount, DuplicateCount, InvalidCount);
    }
}