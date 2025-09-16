#region

using Mohr.Jonas.Spaceshot.Abstractions;

#endregion

namespace Mohr.Jonas.Spaceshot.Exceptions;

public class BackgroundServiceSetupFailedException(IBackgroundService service, Exception exception)
    : AggregateException($"Background service setup of type {service.GetType()} failed", exception);