using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Utopia.Core;

/// <summary>
/// Designed for Autofac and other high-level features.
/// </summary>
public abstract class ExtendedHost(IContainer container) : IHost
{
    private bool _disposed = false;

    private readonly IContainer _container = container;

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    ~ExtendedHost()
    {
        Dispose(disposing: false);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
    }

    protected abstract void Main();

    protected abstract string ThreadName { get; }

    public Task StartAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var token = new TaskCompletionSource();
        var thread = new Thread(() =>
        {
            try
            {
                Main();
            }
            catch(Exception ex)
            {
                token.SetException(ex);
            }

            token.TrySetResult();
        })
        {
            Name = ThreadName,
            IsBackground = false,
            Priority = ThreadPriority.Normal,
        };
        thread.Start();

        return token.Task;
    }

    public Task StopAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        return Task.CompletedTask;
    }

    public IServiceProvider Services { get; } = new AutofacServiceProvider(container);
}