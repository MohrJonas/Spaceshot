#region

using Mohr.Jonas.Spaceshot.Spaceshot.IOC;

#endregion

namespace Mohr.Jonas.Spaceshot.Spaceshot.Services;

internal sealed record BackgroundServiceAdapter(Type ProvidingType, bool IsAsync, Func<DiInjector, object> Factory)
{
    public object? Instance { get; set; }
}