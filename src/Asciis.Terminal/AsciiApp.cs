using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Asciis.Terminal;

public class AsciiApp : IHost
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AsciiAppBuilder"/> class with preconfigured defaults.
    /// </summary>
    /// <returns>The <see cref="AsciiAppBuilder"/>.</returns>
    public static AsciiAppBuilder CreateBuilder() => new(null);

    /// <summary>
    /// Initializes a new instance of the <see cref="AsciiAppBuilder"/> class with optional defaults.
    /// </summary>
    /// <param name="useDefaults">Whether to create the <see cref="AsciiAppBuilder"/> with common defaults.</param>
    /// <returns>The <see cref="AsciiAppBuilder"/>.</returns>
    public static AsciiAppBuilder CreateBuilder(string[]? args = null) => new(args);

    private readonly IHost _host;


    internal AsciiApp(IHost host)
    {
        _host = host;
        Logger = host.Services.GetRequiredService<ILoggerFactory>().CreateLogger(Environment.ApplicationName);
    }

    /// <summary>
    /// The application's configured services.
    /// </summary>
    public IServiceProvider Services => _host.Services;

    /// <summary>
    /// The application's configured <see cref="IConfiguration"/>.
    /// </summary>
    public IConfiguration Configuration => _host.Services.GetRequiredService<IConfiguration>();

    /// <summary>
    /// The application's configured <see cref="IHostEnvironment"/>.
    /// </summary>
    public IHostEnvironment Environment => _host.Services.GetRequiredService<IHostEnvironment>();

    /// <summary>
    /// Allows consumers to be notified of application lifetime events.
    /// </summary>
    public IHostApplicationLifetime Lifetime => _host.Services.GetRequiredService<IHostApplicationLifetime>();

    /// <summary>
    /// The default logger for the application.
    /// </summary>
    public ILogger Logger { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsciiApp"/> class with preconfigured defaults.
    /// </summary>
    /// <returns>The <see cref="AsciiApp"/>.</returns>
    public static AsciiApp Create() => (AsciiApp)new AsciiAppBuilder(null).BuildHost();

    /// <summary>
    /// Start the application.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>
    /// A <see cref="Task"/> that represents the startup of the <see cref="AsciiApp"/>.
    /// Successful completion indicates the HTTP server is ready to accept new requests.
    /// </returns>
    public Task StartAsync(CancellationToken cancellationToken = default) =>
        _host.StartAsync(cancellationToken);

    /// <summary>
    /// Shuts down the application.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>
    /// A <see cref="Task"/> that represents the shutdown of the <see cref="AsciiApp"/>.
    /// Successful completion indicates that all the HTTP server has stopped.
    /// </returns>
    public Task StopAsync(CancellationToken cancellationToken = default) =>
        _host.StopAsync(cancellationToken);

    /// <summary>
    /// Disposes the application.
    /// </summary>
    void IDisposable.Dispose() => _host.Dispose();
}
