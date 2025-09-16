#region

using Mohr.Jonas.Spaceshot.Abstractions;

#endregion

namespace Mohr.Jonas.Spaceshot.Exceptions;

public class AsyncSetupFailedException(IAsyncSetup asyncSetup, Exception exception)
    : AggregateException($"Async setup of type {asyncSetup.GetType()} failed", exception);