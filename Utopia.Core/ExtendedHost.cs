using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Utopia.Core;

/// <summary>
/// Designed for Autofac and other high-level features.
/// </summary>
public abstract class ExtendedHost(IContainer container) : IHost,IHostApplicationLifetime
{
    private async Task BeforeStartServices()
    {
        var services = container.Resolve<IEnumerable<IHostedLifecycleService>>();
        foreach (var service in services)
        {
            await service.StartingAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
    
    private async Task StartServices()
    {
        var services = container.Resolve<IEnumerable<IHostedService>>();
        foreach (var service in services)
        {
            await service.StartAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }

    private async Task AfterStartServices()
    {
        var services = container.Resolve<IEnumerable<IHostedLifecycleService>>();
        foreach (var service in services)
        {
            await service.StartedAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
    
    private async Task BeforeStopServices()
    {
        var services = container.Resolve<IEnumerable<IHostedLifecycleService>>();
        foreach (var service in services)
        {
            await service.StoppingAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
    
    private async Task StopServices()
    {
        var services = container.Resolve<IEnumerable<IHostedService>>();
        foreach (var service in services)
        {
            await service.StopAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }

    private async Task AfterStopServices()
    {
        var services = container.Resolve<IEnumerable<IHostedLifecycleService>>();
        foreach (var service in services)
        {
            await service.StoppedAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
    
    private bool _disposed = false;

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

    protected abstract void Start();
    
    protected abstract void Main();

    protected abstract void Stop();

    protected abstract string ThreadName { get; }

    public Task StartAsync(CancellationToken cancellationToken = new())
    {
        var token = new TaskCompletionSource();
        var thread = new Thread( () =>
        {
            try
            {
                BeforeStartServices().Wait(cancellationToken);
                StartServices().Wait(cancellationToken);
                Start();
                AfterStartServices().Wait(cancellationToken);
                _startedCtx.Cancel();
                Main();
            }
            catch(Exception ex)
            {
                token.SetException(ex);
            }

            token.TrySetResult();

            StopApplication();
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
    
    public void StopApplication()
    {
        try
        {
            _stoppingCts.Cancel();
            BeforeStopServices().Wait(CancellationToken.None);
            StopServices().Wait(CancellationToken.None);
            Stop();
            AfterStopServices().Wait(CancellationToken.None);
        }
        finally
        {
            _stoppedCts.Cancel();
        }
    }
    
    private readonly CancellationTokenSource _startedCtx = new();
    private readonly CancellationTokenSource _stoppingCts = new();
    private readonly CancellationTokenSource _stoppedCts = new();

    public CancellationToken ApplicationStarted => _startedCtx.Token;
    public CancellationToken ApplicationStopping => _stoppingCts.Token;
    public CancellationToken ApplicationStopped => _stoppedCts.Token;
}