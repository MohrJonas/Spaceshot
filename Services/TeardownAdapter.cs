#region

using Mohr.Jonas.Spaceshot.IOC;

#endregion

namespace Mohr.Jonas.Spaceshot.Services;

internal sealed record TeardownAdapter(Type ProvidingType, bool IsAsync, Func<DiInjector, object> Factory);