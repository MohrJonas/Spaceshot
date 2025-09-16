#region

using Mohr.Jonas.Spaceshot.Spaceshot.IOC;

#endregion

namespace Mohr.Jonas.Spaceshot.Spaceshot.Services;

internal sealed record SetupAdapter(Type ProvidingType, bool IsAsync, Func<DiInjector, object> Factory);