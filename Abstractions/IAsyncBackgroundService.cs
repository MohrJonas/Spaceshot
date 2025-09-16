namespace Mohr.Jonas.Spaceshot.Abstractions;

public interface IAsyncBackgroundService
{
    public Task SetupAsync();
    public Task TeardownAsync();
}