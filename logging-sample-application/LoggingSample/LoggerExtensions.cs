namespace Microsoft.Extensions.Logging;

public static partial class LoggerExtensions
{
    [LoggerMessage(
        EventId = 364,
        Level = LogLevel.Information,
        Message = "Retailed vehicle with id {VehicleId}.")]
    public static partial void LogRetailedVehicle(this ILogger logger, Guid vehicleId);

    private static Func<ILogger, Guid, IDisposable?> _orderScope = LoggerMessage.DefineScope<Guid>("Order {OrderId}");

    public static IDisposable? BeginOrderScope(this ILogger logger, Guid orderId)
        => _orderScope(logger, orderId);
}
