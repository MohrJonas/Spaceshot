#region

using Mohr.Jonas.Spaceshot.Abstractions;

#endregion

namespace Mohr.Jonas.Spaceshot.Exceptions;

public class TeardownFailedException(ITeardown teardown, Exception exception)
    : AggregateException($"Teardown of type {teardown.GetType()} failed", exception);