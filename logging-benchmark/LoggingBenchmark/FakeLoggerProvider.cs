namespace Microsoft.Extensions.Logging;

#pragma warning disable S3881 // "IDisposable" should be implemented correctly
#pragma warning disable S1186 // Methods should not be empty

public class FakeLoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName) => new FakeLogger();

    public void Dispose()
    {
    }

    private sealed class FakeLogger : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
            => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Information;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
        }
    }

    /// <summary>
    /// An empty scope without any logic
    /// </summary>
    /// <see cref="https://github.com/dotnet/runtime/blob/66556c467d0b2419b5c2a261fdfa2703e6b3f6bc/src/libraries/Common/src/Extensions/Logging/NullScope.cs"/>
    internal sealed class NullScope : IDisposable
    {
        public static NullScope Instance { get; } = new NullScope();

        private NullScope()
        {
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }
    }
}

#pragma warning restore S1186 // Methods should not be empty
#pragma warning restore S3881 // "IDisposable" should be implemented correctly
