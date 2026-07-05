# I4Twins Backend Task

Service for ingesting sensor readings and exposing time-based aggregation API.

## How to Run

dotnet run --project I4Twins.Api

Place readings.jsonl in I4Twins.Api/Data/ folder before running.

The app will:

Create SQLite database (readings.db)

Ingest all readings and show count report in console

Start API on https://localhost:5001

How to Test
dotnet test I4Twins.Tests

API
GET /api/readings/aggregate

Parameter	Type	Example
deviceId	string	PUMP-01
metric	string	temperature
from	ISO-8601 UTC	2025-06-01T08:00:00Z
to	ISO-8601 UTC	2025-06-01T09:00:00Z
bucketSizeSeconds	int (default: 60)	60
Response:

[
  {
    "bucketStart": "2025-06-01T08:00:00Z",
    "count": 5,
    "avg": 67.5,
    "min": 65.0,
    "max": 70.0
  }
]

Key Decisions

Decision	Reason
SQLite	No installation, lightweight
UTC timestamps	Correct deduplication & aggregation
Scoped DI	DbContext thread-safety
Empty buckets omitted	Cleaner response
Valid metrics	temperature, pressure, vibration

Architecture
Domain → Entities + Business Rules
Application → Use Cases (Ingestion, Aggregation)
Infrastructure → SQLite + Repositories
API → Controllers

Requirements
Ingestion from JSONL file

Deduplication (deviceId, metric, ts, seq)

Out-of-order support

Invalid record handling

Aggregation API (count, avg, min, max)

Unit tests

Count report

Logging

CORS enabled