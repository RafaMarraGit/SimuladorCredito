using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Collections.Generic;

namespace SimuladorCredito.Services
{
    public class TelemetryService
    {
        private long _requestCount = 0;
        private readonly DateTime _startTime;

        // Registro de cada requisição
        public class RequestTelemetry
        {
            public DateTime Timestamp { get; set; }
            public double DurationMs { get; set; }
            public int StatusCode { get; set; }
            public bool Success => StatusCode >= 200 && StatusCode < 400;
            public string Path { get; set; }
            public string Method { get; set; } // Adicione este campo
        }

        private readonly ConcurrentBag<RequestTelemetry> _requests = new();

        public TelemetryService()
        {
            _startTime = DateTime.UtcNow;
        }

        public void IncrementRequestCount()
        {
            Interlocked.Increment(ref _requestCount);
        }

        public long GetRequestCount() => Interlocked.Read(ref _requestCount);

        public DateTime GetStartTime() => _startTime;

        public TimeSpan GetUptime() => DateTime.UtcNow - _startTime;

        public void RegisterRequest(DateTime timestamp, double durationMs, int statusCode, string path, string method)
        {
            _requests.Add(new RequestTelemetry
            {
                Timestamp = timestamp,
                DurationMs = durationMs,
                StatusCode = statusCode,
                Path = path,
                Method = method
            });
        }

        public IEnumerable<RequestTelemetry> GetRequests() => _requests;
    }
}
