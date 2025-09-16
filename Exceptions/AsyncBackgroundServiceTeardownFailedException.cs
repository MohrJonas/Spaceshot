#region

using Mohr.Jonas.Spaceshot.Abstractions;

#endregion

namespace Mohr.Jonas.Spaceshot.Exceptions;

public class AsyncBackgroundServiceTeardownFailedException(IAsyncBackgroundService service, Exception exception)
    : AggregateException($"Async background service teardown of type {service.GetType()} failed", exception);