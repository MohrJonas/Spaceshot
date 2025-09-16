namespace Mohr.Jonas.Spaceshot.Spaceshot.IOC;

internal sealed record Injectable(
    string? Key,
    Type ProvidedType,
    InjectionStrategy InjectionStrategy,
    Func<Type, DiInjector, object> Factory)
{
    public object? Instance { get; set; }
}