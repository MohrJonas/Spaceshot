#region

using Mohr.Jonas.Spaceshot.Abstractions;
using Mohr.Jonas.Spaceshot.IOC;
using Mohr.Jonas.Spaceshot.Services;

#endregion

namespace Mohr.Jonas.Spaceshot.Configuration;

public sealed class ControlsConfigurator
{
    private readonly ServicesContainer _services;

    internal ControlsConfigurator(ServicesContainer services)
    {
        _services = services;
    }

    public void RegisterSetup<TSetup>(Func<DiInjector, TSetup> factory) where TSetup : ISetup
    {
        _services.Setups.Add(new SetupAdapter(typeof(TSetup), false, injector => factory(injector)));
    }

    public void RegisterAsyncSetup<TAsyncSetup>(Func<DiInjector, TAsyncSetup> factory) where TAsyncSetup : IAsyncSetup
    {
        _services.Setups.Add(new SetupAdapter(typeof(TAsyncSetup), true, injector => factory(injector)));
    }

    public void RegisterTeardown<TTeardown>(Func<DiInjector, TTeardown> factory) where TTeardown : ITeardown
    {
        _services.Teardowns.Add(new TeardownAdapter(typeof(TTeardown), false, injector => factory(injector)));
    }

    public void RegisterAsyncTeardown<TAsyncTeardown>(Func<DiInjector, TAsyncTeardown> factory)
        where TAsyncTeardown : IAsyncTeardown
    {
        _services.Teardowns.Add(new TeardownAdapter(typeof(TAsyncTeardown), true,
            injector => factory(injector)));
    }

    public void RegisterBackgroundService<TBackgroundService>(Func<DiInjector, TBackgroundService> factory)
        where TBackgroundService : IBackgroundService
    {
        _services.BackgroundServices.Add(new BackgroundServiceAdapter(typeof(TBackgroundService), false, injector => factory(injector)));
    }

    public void RegisterAsyncBackgroundService<TAsyncBackgroundService>(
        Func<DiInjector, TAsyncBackgroundService> factory)
        where TAsyncBackgroundService : IAsyncBackgroundService
    {
        _services.BackgroundServices.Add(new BackgroundServiceAdapter(typeof(TAsyncBackgroundService), true, injector => factory(injector)));
    }
}