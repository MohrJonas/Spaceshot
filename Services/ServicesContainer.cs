#region

using System.Collections.Concurrent;

#endregion

namespace Mohr.Jonas.Spaceshot.Services;

internal sealed class ServicesContainer
{
    public ConcurrentBag<SetupAdapter> Setups { get; } = [];
    public ConcurrentBag<TeardownAdapter> Teardowns { get; } = [];
    public ConcurrentBag<BackgroundServiceAdapter> BackgroundServices { get; } = [];
}