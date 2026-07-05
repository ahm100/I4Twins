using I4Twins.Application.Services;
using I4Twins.Domain.Interfaces;
using I4Twins.Infrastructure.Persistence;
using I4Twins.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=readings.db"));

builder.Services.AddScoped<IReadingRepository, ReadingRepository>();
builder.Services.AddScoped<IReadingDuplicateChecker, ReadingDuplicateChecker>();
builder.Services.AddScoped<IAggregationService, AggregationService>();
builder.Services.AddScoped<IIngestionService, IngestionService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.EnsureCreatedAsync();

    var ingestionService = scope.ServiceProvider.GetRequiredService<IIngestionService>();
    var filePath = Path.Combine(AppContext.BaseDirectory, "Data", "readings.jsonl");

    await ingestionService.ProcessFileAsync(filePath);

    Console.WriteLine();
    Console.WriteLine("===== INGESTION REPORT =====");
    Console.WriteLine($"Total Lines Read:   {ingestionService.TotalLines}");
    Console.WriteLine($"Stored Successfully: {ingestionService.StoredCount}");
    Console.WriteLine($"Duplicates Removed:  {ingestionService.DuplicateCount}");
    Console.WriteLine($"Invalid Rejected:    {ingestionService.InvalidCount}");
    Console.WriteLine("=============================");
    Console.WriteLine();
}

app.Run();
