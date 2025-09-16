#region

using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Mohr.Jonas.Spaceshot.Spaceshot.Abstractions;

#endregion

namespace Mohr.Jonas.Spaceshot.Spaceshot.IOC;

public sealed class DiContainer : IDiContainer
{
    private const long AllowedInjectionDuration = 20;
    private readonly ConcurrentBag<Injectable> _injectables = [];
    private readonly ILogger _logger;
    private readonly Stopwatch _stopwatch = new();

    internal DiContainer(ILogger logger)
    {
        _logger = logger;
    }

    public object GetValueForType(Type injectedClassType, Type type)
    {
        var injectable = _injectables.Single(injectable => injectable.ProvidedType == type);
        return GetValue(injectedClassType, injectable);
    }

    public object GetValueForKey(Type injectedClassType, string key)
    {
        var injectable = _injectables.Single(injectable => injectable.Key == key);
        return GetValue(injectedClassType, injectable);
    }

    public bool EvictValueForType(Type type)
    {
        var injectable = _injectables.FirstOrDefault(injectable => injectable.ProvidedType == type);
        if(injectable?.Instance == null)
            return false;
        injectable.Instance = null;
        return true;
    }

    public bool EvictValueForKey(string key)
    {
        var injectable = _injectables.FirstOrDefault(injectable => injectable.Key == key);
        if(injectable?.Instance == null)
            return false;
        injectable.Instance = null;
        return true;
    }

    public void RegisterInjectable<TType>(string? key, InjectionStrategy injectionStrategy,
        Func<Type, DiInjector, TType> factory) where TType : class
    {
        _injectables.Add(new Injectable(key, typeof(TType), injectionStrategy, factory));
    }

    private object InstantiateIfNecessary(Type injectedClassType, Injectable injectable)
    {
        return injectable.Instance ?? (injectable.Instance =
            injectable.Factory(injectedClassType, new DiInjector(injectedClassType, this)));
    }

    private object GetValue(Type injectedClassType, Injectable injectable)
    {
        _stopwatch.Restart();
        var value = injectable.InjectionStrategy switch
        {
            InjectionStrategy.Singleton or InjectionStrategy.Scoped => InstantiateIfNecessary(injectedClassType,
                injectable),
            InjectionStrategy.OneShot => injectable.Factory(injectedClassType, new DiInjector(injectedClassType, this)),
            _ => throw new ArgumentOutOfRangeException(nameof(injectedClassType), injectedClassType, null)
        };
        _stopwatch.Stop();
        if (_stopwatch.ElapsedMilliseconds >= AllowedInjectionDuration)
            _logger.LogWarning("Injectable factory took over {}ms ({}ms)", AllowedInjectionDuration,
                _stopwatch.ElapsedMilliseconds);
        return value;
    }
}