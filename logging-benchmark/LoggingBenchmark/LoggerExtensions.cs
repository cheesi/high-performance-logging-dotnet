using Microsoft.Extensions.Logging;

namespace LoggingBenchmark;

public static partial class LoggerExtensions
{
    #region Define Information

    private static readonly Action<ILogger, Guid, Exception?> _retailedVehicle =
          LoggerMessage.Define<Guid>(
             LogLevel.Information,
             new EventId(364),
             "Retailed vehicle with id {vehicleId}.");

    public static void LogRetailedVehicle(
        this ILogger logger,
        Guid vehicleId)
        => _retailedVehicle(logger, vehicleId, default);

    #endregion

    #region Define Debug

    private static readonly Action<ILogger, Guid, Exception?> _retailedVehicleDebug =
      LoggerMessage.Define<Guid>(
         LogLevel.Debug,
         new EventId(364),
         "Retailed vehicle with id {vehicleId}.");

    public static void LogRetailedVehicleDebug(
        this ILogger logger,
        Guid vehicleId)
        => _retailedVehicleDebug(logger, vehicleId, default);

    #endregion

    [LoggerMessage(
        EventId = 364,
        Level = LogLevel.Information,
        Message = "Retailed vehicle with id {vehicleId}.")]
    public static partial void LogRetailedVehicleGenerated(this ILogger logger, Guid vehicleId);

    [LoggerMessage(
        EventId = 365,
        Level = LogLevel.Debug,
        Message = "Retailed vehicle with id {vehicleId}.")]
    public static partial void LogRetailedVehicleGeneratedDebug(this ILogger logger, Guid vehicleId);
}
