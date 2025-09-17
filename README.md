# 🚀 Spaceshot - An alternative apphost

## About

Spaceshot in an apphost, similar to packages like [Microsoft.Extensions.Hosting](https://www.nuget.org/packages/microsoft.extensions.hosting)

## Features

- Dependency injection for services
- Dynamic service creating / starting / stopping at runtime
- Minimal, easily maintainable codebase

## Dependency injection

Dependency injection is configured via `Spaceshot.ConfigurePayload()`.

### Injection strategies

Dependency injection support three strategies:

- `Singleton`: The value will be created once during the lifetime of the application
- `Scoped`: The value is created once required and will persist until disposed at which point it will be recreated the next time it is required
- `OneShot` The value is created every time it is required

### Registering values

Values can be injected at startup via `Spaceshot.ConfigurePayload()` or during runtime via the magic `PayloadConfigurator` object.

```csharp
configurator.RegisterSingleton(static (_, _) => LoggerFactory.Create(builder =>
{
    builder.SetMinimumLevel(LogLevel.Trace);
    builder.AddSimpleConsole(conf =>
    {
        conf.IncludeScopes = true;
        conf.SingleLine = false;
    });
}));
configurator.RegisterOneShot(static (type, injector) => injector.Inject<ILoggerFactory>().CreateLogger(type));
configurator.RegisterSingleton(static (_, _) => new ProgramSettings());
```

### The injector

The injector is used to retrieve values from the dependency injection container via its methods:

```csharp
public TInjectable Inject<TInjectable>() { ... }
public TInjectable InjectKeyed<TInjectable>(string key) { ... }
```

### Magic objects

Magic object are objects that can be retrieved via dependency injection and are always automatically available.

#### The Spaceship object

The spaceship object is a special object that can be retrieved at runtime.  
It functions as a controller for the running apphost, allowing the user to stop it.

#### The PayloadEvictor

The payload evictor can be used to evict values from the dependency injection container, making this tool essential for using `Scoped` values.

#### The PayloadConfigurator

The payload configurator is used to add dependency injection values at runtime.

## Services

Services are configured via `Spaceshot.ConfigureControls()`.
The available services are:
- `ISetup`: Runs once at apphost start, used to set up dependencies, create required files, parsing arguments, ...
- `IAsyncSetup`: Same as `ISetup`, but runs asynchronously
- `ITeardown`: Runs once at apphost stop, used to save files, dispose objects, ...
- `IAsyncTeardown`: Same as `ITeardown`, but runs asynchronously
- `IBackgroundService`: Combination of `ISetup` and `ITeardown`
- `IAsyncBackground`: Same as `IBackgroundService`, but runs asynchronously

```csharp
spaceshot.ConfigureControls(static configurator =>
{
    configurator.RegisterSetup<ParseCliSetup>(static _ => new ParseCliSetup());
    configurator.RegisterAsyncTeardown<DBDisposer>(static injector => new DBDisposer(
        injector.Inject<ILogger>(),
        injector.Inject<SqliteConnection>()
    ));
});
```

### Configuring startup / teardown sequence

The startup and teardown sequence can be configured via the `PriorityAttribute`.  
The attribute supports the following priorities, execute in the following order:

- `Highest`
- `High`
- `Medium`
- `Low`
- `Lowest`

Assuming you want to configure the `ParseCliSetup` to run first, the `PriorityAttribute` can be used as follows:

```csharp
[Priority(Priority.Highest)]
internal sealed class ParseCliSetup : ISetup
{
    public void Setup() { ... }
}
```

## Full example

```csharp
internal static class Program
{
    private static void Main()
    {
        using var factory = LoggerFactory.Create(builder =>
            builder.AddSimpleConsole(conf =>
            {
                conf.IncludeScopes = true;
                conf.SingleLine = false;
            }));
        var logger = factory.CreateLogger(typeof(Spaceshot));
        var spaceship = new Spaceshot(logger);
        spaceship.ConfigurePayload(static configurator =>
        {
            configurator.RegisterSingleton(static (_, _) => LoggerFactory.Create(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Trace);
                builder.AddSimpleConsole(conf =>
                {
                    conf.IncludeScopes = true;
                    conf.SingleLine = false;
                });
            }));
            configurator.RegisterOneShot(static (type, injector) => injector.Inject<ILoggerFactory>().CreateLogger(type));
            configurator.RegisterSingleton(static (_, _) => new ProgramSettings());
        });
        spaceship.ConfigureControls(static configurator =>
        {
            configurator.RegisterSetup(static injector => new SigtermCatcher(
                    injector.Inject<ILogger>(),
                    injector.Inject<Spaceship>()
                ));
            configurator.RegisterAsyncSetup(static injector => new DbMigrator(
                injector.Inject<ILogger>(),
                injector.Inject<PayloadConfigurator>(),
                injector.Inject<ProgramSettings>()
            ));
            configurator.RegisterAsyncTeardown(static injector => new DBDisposer(
                injector.Inject<ILogger>(),
                injector.Inject<SqliteConnection>()
            ));
        });
        spaceship.LiftOff();
    }
}
```