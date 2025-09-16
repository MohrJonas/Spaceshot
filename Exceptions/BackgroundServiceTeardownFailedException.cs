#region

using Mohr.Jonas.Spaceshot.Abstractions;

#endregion

namespace Mohr.Jonas.Spaceshot.Exceptions;

public class BackgroundServiceTeardownFailedException(IBackgroundService service, Exception exception)
    : AggregateException($"Background service teardown of type {service.GetType()} failed", exception);