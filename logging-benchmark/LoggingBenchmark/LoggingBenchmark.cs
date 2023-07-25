using BenchmarkDotNet.Attributes;
using Isle.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LoggingBenchmark;

[MemoryDiagnoser]
public class LoggingBenchmark
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<LoggingBenchmark> _logger;
    private readonly Guid _vehicleId = Guid.NewGuid();

    public LoggingBenchmark()
    {
        _loggerFactory = LoggerFactory.Create(builder =>
            builder.Services.AddSingleton<ILoggerProvider, FakeLoggerProvider>());
        _logger = _loggerFactory.CreateLogger<LoggingBenchmark>();
        IsleConfiguration.Configure(builder => { });
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _loggerFactory.Dispose();
    }

    [Benchmark]
    public void InterpolatedString()
    {
        Microsoft.Extensions.Logging.LoggerExtensions.LogInformation(_logger, $"Retailed vehicle with id {_vehicleId}.");
    }

    [Benchmark]
    public void InterpolatedString_NotLogged()
    {
        Microsoft.Extensions.Logging.LoggerExtensions.LogDebug(_logger, $"Retailed vehicle with id {_vehicleId}.");
    }

    [Benchmark]
    public void StructuredLogging()
    {
        _logger.LogInformation("Retailed vehicle with id {vehicleId}.", _vehicleId);
    }

    [Benchmark]
    public void StructuredLogging_NotLogged()
    {
        _logger.LogDebug("Retailed vehicle with id {vehicleId}.", _vehicleId);
    }

    [Benchmark]
    public void LoggerMessageDefine()
    {
        _logger.LogRetailedVehicle(_vehicleId);
    }

    [Benchmark]
    public void LoggerMessageDefine_NotLogged()
    {
        _logger.LogRetailedVehicleDebug(_vehicleId);
    }

    [Benchmark(Baseline = true)]
    public void LoggerMessageGenerated()
    {
        _logger.LogRetailedVehicleGenerated(_vehicleId);
    }

    [Benchmark]
    public void LoggerMessageGenerated_NotLogged()
    {
        _logger.LogRetailedVehicleGeneratedDebug(_vehicleId);
    }

    [Benchmark]
    public void ISLE()
    {
        Isle.Extensions.Logging.LoggerExtensions.LogInformation(_logger, $"Retailed vehicle with id {_vehicleId}.");
    }

    [Benchmark]
    public void ISLE_NotLogged()
    {
        Isle.Extensions.Logging.LoggerExtensions.LogDebug(_logger, $"Retailed vehicle with id {_vehicleId}.");
    }
}
