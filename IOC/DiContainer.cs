#region

using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Mohr.Jonas.Spaceshot.Abstractions;
using Mohr.Jonas.Spaceshot.Exceptions;

#endregion

namespace Mohr.Jonas.Spaceshot.IOC;

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
        var injectables = _injectables.Where(injectable => injectable.ProvidedType.IsAssignableTo(type)).ToArray();
        return injectables.Length switch
        {
            0 => throw new NoInjectableFoundException(type),
            1 => GetValue(injectedClassType, injectables[0]),
            _ => throw new AmbiguousInjectionException(type)
        };
    }

    public object GetValueForKey(Type injectedClassType, string key)
    {
        var injectables = _injectables.Where(injectable => injectable.Key == key).ToArray();
        return injectables.Length switch
        {
            0 => throw new NoInjectableFoundException(key),
            1 => GetValue(injectedClassType, injectables[0]),
            _ => throw new AmbiguousInjectionException(key)
        };
    }

    public bool EvictValueForType(Type type)
    {
        var injectable = _injectables.FirstOrDefault(injectable => injectable.ProvidedType.IsAssignableTo(type));
        if (injectable?.Instance == null)
            return false;
        injectable.Instance = null;
        return true;
    }

    public bool EvictValueForKey(string key)
    {
        var injectable = _injectables.FirstOrDefault(injectable => injectable.Key == key);
        if (injectable?.Instance == null)
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