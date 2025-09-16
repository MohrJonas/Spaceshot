#region

using Mohr.Jonas.Spaceshot.Abstractions;

#endregion

namespace Mohr.Jonas.Spaceshot.Exceptions;

public class AsyncTeardownFailedException(IAsyncTeardown asyncTeardown, Exception exception)
    : AggregateException($"Async teardown of type {asyncTeardown.GetType()} failed", exception);