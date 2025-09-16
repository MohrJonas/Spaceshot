#region

using System.Globalization;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Mohr.Jonas.Spaceshot.Spaceshot.Abstractions;
using Mohr.Jonas.Spaceshot.Spaceshot.Configuration;
using Mohr.Jonas.Spaceshot.Spaceshot.IOC;
using Mohr.Jonas.Spaceshot.Spaceshot.Services;

#endregion

namespace Mohr.Jonas.Spaceshot.Spaceshot;

public class Spaceshot
{
    private readonly ILogger _logger;
    private readonly DiContainer _diContainer;
    private readonly PayloadEvictor _payloadEvictor;
    private readonly ServicesContainer _servicesContainer = new();
    private readonly CancellationTokenSource _tokenSource = new();
    private readonly Spaceship _spaceship;
    private readonly ManualResetEventSlim _shutdownCompleteEvent = new(false);
    
    public Spaceshot(ILogger logger)
    {
        _logger = logger;
        _diContainer = new DiContainer(logger);
        _payloadEvictor = new PayloadEvictor(logger, _diContainer);
        _spaceship = new Spaceship(this);
        
        var configurator = new PayloadConfigurator(_diContainer);
        configurator.RegisterSingleton((_, _) => _payloadEvictor);
        configurator.RegisterSingleton((_, _) => _spaceship);
        configurator.RegisterSingleton((_, _) => configurator);
    }
    
    private async Task RunSetups()
    {
        using (_logger.BeginScope("Running setups"))
        {
            var groupedSetups = _servicesContainer.Setups
                .GroupBy(static func =>
                    func.ProvidingType.GetCustomAttribute<PriorityAttribute>()?.Priority ?? Priority.Medium)
                .OrderByDescending(grouping => grouping.Key)
                .ToArray();
            foreach (var group in groupedSetups)
                using (_logger.BeginScope("Running setups with priority {}", group.Key))
                {
                    _logger.LogInformation("Running setups with priority {}", group.Key);
                    await Task.WhenAll(group.Select(async setupAdapter =>
                    {
                        _logger.LogInformation("Running setup {}", setupAdapter.ProvidingType.Name);
                        var instance = setupAdapter.Factory(new DiInjector(setupAdapter.ProvidingType, _diContainer));
                        if (setupAdapter.IsAsync)
                            await ((IAsyncSetup)instance).SetupAsync();
                        else
                            await Task.Run(() => ((ISetup)instance).Setup());
                    }));
                }
        }
    }

    private async Task RunTeardowns()
    {
        using (_logger.BeginScope("Running teardowns"))
        {
            var groupedTeardowns = _servicesContainer.Teardowns
                .GroupBy(static func =>
                    func.ProvidingType.GetCustomAttribute<PriorityAttribute>()?.Priority ?? Priority.Medium)
                .OrderByDescending(grouping => grouping.Key)
                .ToArray();
            foreach (var group in groupedTeardowns)
                using (_logger.BeginScope("Running teardowns with priority {}", group.Key))
                {
                    await Task.WhenAll(group.Select(async teardownAdapter =>
                    {
                        _logger.LogInformation("Running teardown {}", teardownAdapter.ProvidingType.Name);
                        var instance =
                            teardownAdapter.Factory(new DiInjector(teardownAdapter.ProvidingType, _diContainer));
                        if (teardownAdapter.IsAsync)
                            await ((IAsyncTeardown)instance).TeardownAsync();
                        else
                            await Task.Run(() => ((ITeardown)instance).Teardown());
                    }));
                }
        }
    }

    private async Task RunBackgroundServiceSetup()
    {
        using (_logger.BeginScope("Running background service setup"))
        {
            foreach (var backgroundServiceAdapter in _servicesContainer.BackgroundServices)
            {
                _logger.LogInformation("Running background setup {}", backgroundServiceAdapter.ProvidingType.Name);
                var instance =
                    backgroundServiceAdapter.Factory(new DiInjector(backgroundServiceAdapter.ProvidingType,
                        _diContainer));
                backgroundServiceAdapter.Instance = instance;
                if (backgroundServiceAdapter.IsAsync)
                    await ((IAsyncBackgroundService)instance).SetupAsync();
                else
                    await Task.Run(() => ((IBackgroundService)instance).Setup());
            }
        }
    }

    private async Task RunBackgroundServiceTeardown()
    {
        using (_logger.BeginScope("Running background service teardown"))
        {
            foreach (var backgroundServiceAdapter in _servicesContainer.BackgroundServices)
            {
                _logger.LogInformation("Running background taerdown {}", backgroundServiceAdapter.ProvidingType.Name);
                if (backgroundServiceAdapter.IsAsync)
                    await ((IAsyncBackgroundService)backgroundServiceAdapter.Instance!).TeardownAsync();
                else
                    await Task.Run(() => ((IBackgroundService)backgroundServiceAdapter.Instance!).Teardown());
            }
        }
    }

    public void LiftOff()
    {
        RunSetups().Wait();
        RunBackgroundServiceSetup().Wait();
        _logger.LogInformation("🚀 We have liftoff at {}", DateTime.Now.ToString(CultureInfo.CurrentCulture));
        _tokenSource.Token.WaitHandle.WaitOne();
        RunTeardowns().Wait();
        RunBackgroundServiceTeardown().Wait();
        _shutdownCompleteEvent.Set();
    }

    public async Task LiftOffAsync()
    {
        await RunSetups();
        await RunBackgroundServiceSetup();
        _logger.LogInformation("🚀 We have liftoff at {}", DateTime.Now.ToString(CultureInfo.CurrentCulture));
        _tokenSource.Token.WaitHandle.WaitOne();
        await RunTeardowns();
        await RunBackgroundServiceTeardown();
        _shutdownCompleteEvent.Set();
    }

    public void AbortMission(bool waitForCompletion = false)
    {
        _logger.LogInformation("Aborting mission");
        _tokenSource.Cancel();
        if (waitForCompletion)
            _shutdownCompleteEvent.Wait();
    }

    public async Task AbortMissionAsync(bool waitForCompletion = false)
    {
        _logger.LogInformation("Aborting mission");
        await _tokenSource.CancelAsync();
        if (waitForCompletion)
            _shutdownCompleteEvent.Wait();
    }

    public void ConfigurePayload(Action<PayloadConfigurator> configurator)
    {
        configurator(new PayloadConfigurator(_diContainer));
    }

    public void ConfigureControls(Action<ControlsConfigurator> configurator)
    {
        configurator(new ControlsConfigurator(_servicesContainer));
    }
}