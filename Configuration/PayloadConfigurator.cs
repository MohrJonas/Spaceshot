#region

using Mohr.Jonas.Spaceshot.Spaceshot.IOC;

#endregion

namespace Mohr.Jonas.Spaceshot.Spaceshot.Configuration;

public sealed class PayloadConfigurator
{
    private readonly DiContainer _container;

    internal PayloadConfigurator(DiContainer container)
    {
        _container = container;
    }

    public void RegisterSingleton<TSingleton>(Func<Type, DiInjector, TSingleton> factory) where TSingleton : class
    {
        _container.RegisterInjectable(null, InjectionStrategy.Singleton, factory);
    }

    public void RegisterKeyedSingleton<TSingleton>(string key, Func<Type, DiInjector, TSingleton> factory)
        where TSingleton : class
    {
        _container.RegisterInjectable(key, InjectionStrategy.Singleton, factory);
    }

    public void RegisterScoped<TScoped>(Func<Type, DiInjector, TScoped> factory) where TScoped : class
    {
        _container.RegisterInjectable(null, InjectionStrategy.Scoped, factory);
    }

    public void RegisterKeyedScoped<TScoped>(string key, Func<Type, DiInjector, TScoped> factory) where TScoped : class
    {
        _container.RegisterInjectable(key, InjectionStrategy.Scoped, factory);
    }

    public void RegisterOneShot<TOneShot>(Func<Type, DiInjector, TOneShot> factory) where TOneShot : class
    {
        _container.RegisterInjectable(null, InjectionStrategy.OneShot, factory);
    }

    public void RegisterKeyedOneShot<TOneShot>(string key, Func<Type, DiInjector, TOneShot> factory)
        where TOneShot : class
    {
        _container.RegisterInjectable(key, InjectionStrategy.OneShot, factory);
    }
}