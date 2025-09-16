#region

using Mohr.Jonas.Spaceshot.Abstractions;

#endregion

namespace Mohr.Jonas.Spaceshot.Exceptions;

public class AsyncBackgroundServiceSetupFailedException(IAsyncBackgroundService service, Exception exception)
    : AggregateException($"Async background service setup of type {service.GetType()} failed", exception);