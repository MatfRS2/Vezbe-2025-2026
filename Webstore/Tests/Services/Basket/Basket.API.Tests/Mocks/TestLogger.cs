using Basket.API.Services;
using Microsoft.Extensions.Logging;

namespace Basket.API.Tests.Mocks;

public class TestLogger : ILogger<BasketService>
{
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        throw new NotImplementedException();
    }

    public bool IsEnabled(LogLevel logLevel)
        => false;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        throw new NotImplementedException();
    }
}