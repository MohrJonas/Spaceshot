#region

using Mohr.Jonas.Spaceshot.IOC;

#endregion

namespace Mohr.Jonas.Spaceshot.Services;

internal sealed record BackgroundServiceAdapter(Type ProvidingType, bool IsAsync, Func<DiInjector, object> Factory)
{
    public object? Instance { get; set; }
}