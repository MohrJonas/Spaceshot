#region

using Mohr.Jonas.Spaceshot.Abstractions;

#endregion

namespace Mohr.Jonas.Spaceshot.Exceptions;

public class SetupFailedException(ISetup setup, Exception exception)
    : AggregateException($"Setup of type {setup.GetType()} failed", exception);