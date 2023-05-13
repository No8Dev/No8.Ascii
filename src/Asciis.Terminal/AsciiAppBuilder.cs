using Asciis.Terminal.ConsoleDrivers;
using Asciis.Terminal.ConsoleDrivers.CursesDriver;
using Asciis.Terminal.ConsoleDrivers.FakeDriver;
using Asciis.Terminal.ConsoleDrivers.NetDriver;
using Asciis.Terminal.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;
using Microsoft.Extensions.Primitives;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Asciis.Terminal;

public class AsciiAppBuilder
{
    private readonly AsciiApplicationServiceCollection _services = new();
    private readonly ILoggingBuilder _logging;
    private AsciiApp? _builtAsciiApp;
	private IHostBuilder _hostBuilder;

    public AsciiAppBuilder(string[]? args = null)
    {
        Services = _services;
		Services.AddSingleton<IConfiguration>(_ => Configuration);

        _logging = new LoggingBuilder(Services);

		_hostBuilder = new HostBuilder();
		_hostBuilder.ConfigureAsciiDefaults(args);
    }

    /// <summary>
    /// A collection of services for the application to compose. This is useful for adding user provided or framework provided services.
    /// </summary>
    public IServiceCollection Services { get; }

    /// <summary>
    /// A collection of configuration providers for the application to compose. This is useful for adding new configuration sources and providers.
    /// </summary>
    public ConfigurationManager Configuration { get; } = new();

    /// <summary>
    /// A collection of logging providers for the application to compose. This is useful for adding new logging providers.
    /// </summary>
    public ILoggingBuilder Logging => _logging;

    public IDictionary<object, object> Properties { get; } = new Dictionary<object, object>();

    public IHost BuildHost()
    {
		var tests = UnitTestDetector.IsRunningFromNUnit;
		if (tests)
        {
			if (!_services.Any(s => s.ServiceType == typeof(ConsoleDriver)))
            {
				_services.AddSingleton<ConsoleDriver, FakeConsoleDriver>();
				_services.AddSingleton<IMainLoopDriver, FakeMainLoopDriver>();
			}
		}

		if (!_services.Any(s => s.ServiceType.IsAssignableTo(typeof(AsciiApplication))))
		{
			this.UseAsciiApp<AsciiApplication>();
		}

		_hostBuilder.ConfigureHostConfiguration(builder =>
		{
			var applicationName = Configuration[HostDefaults.ApplicationKey] ?? GetDefaultApplicationName();
			var environmentName = Configuration[HostDefaults.EnvironmentKey] ?? Environments.Production;
			var contentRootPath = Configuration[HostDefaults.ContentRootKey]
					?? AppContext.BaseDirectory
					?? Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

			builder.AddInMemoryCollection(
				new Dictionary<string, string> {
						{ HostDefaults.ApplicationKey, applicationName },
						{ HostDefaults.EnvironmentKey, environmentName },
						{ HostDefaults.ContentRootKey, contentRootPath },
				});
		});

		// Chain the configuration sources into the final IConfigurationBuilder
		var chainedConfigSource = new TrackingChainedConfigurationSource(Configuration);

		_hostBuilder.ConfigureAppConfiguration(builder =>
		{
			builder.Add(chainedConfigSource);

			foreach (var kvp in ((IConfigurationBuilder)Configuration).Properties)
				builder.Properties[kvp.Key] = kvp.Value;
		});

		// This needs to go here to avoid adding the IHostedService that boots the server twice (the GenericWebHostService).
		// Copy the services that were added via AsciiAppBuilder.Services into the final IServiceCollection
		_hostBuilder.ConfigureServices((context, services) =>
		{
			// We've only added services configured by the GenericWebHostBuilder and WebHost.ConfigureWebDefaults
			// at this point. HostBuilder news up a new ServiceCollection in HostBuilder.Build() we haven't seen
			// until now, so we cannot clear these services even though some are redundant because
			// we called ConfigureWebHostDefaults on both the _deferredHostBuilder and _hostBuilder.
			foreach (var s in _services)
				services.Add(s);

			// Drop the reference to the existing collection and set the inner collection
			// to the new one. This allows code that has references to the service collection to still function.
			_services.InnerCollection = services;

			if (!_services.Any(s => s.ServiceType == typeof(ConsoleDriver)))
			{
				if (OperatingSystem.IsWindows())
				{
					_services.AddSingleton<ConsoleDriver, WindowsDriver>();
					_services.AddSingleton<IMainLoopDriver, WindowsMainLoop>();
				}
				else if (OperatingSystem.IsLinux())
				{
					_services.AddSingleton<ConsoleDriver, CursesDriver>();
					_services.AddSingleton<IMainLoopDriver, UnixMainLoop>();
				}
				else
				{
					_services.AddSingleton<ConsoleDriver, SystemConsoleDriver>();
					_services.AddSingleton<IMainLoopDriver, SystemConsoleMainLoop>();
				}
			}


			var hostBuilderProviders = ((IConfigurationRoot)context.Configuration).Providers;

			if (!hostBuilderProviders.Contains(chainedConfigSource.BuiltProvider))
			{
				// Something removed the _hostBuilder's TrackingChainedConfigurationSource pointing back to the ConfigurationManager.
				// Replicate the effect by clearing the ConfingurationManager sources.
				((IConfigurationBuilder)Configuration).Sources.Clear();
			}

			// Make builder.Configuration match the final configuration. To do that, we add the additional
			// providers in the inner _hostBuilders's Configuration to the ConfigurationManager.
			foreach (var provider in hostBuilderProviders)
			{
				if (!ReferenceEquals(provider, chainedConfigSource.BuiltProvider))
				{
					((IConfigurationBuilder)Configuration).Add(new ConfigurationProviderSource(provider));
				}
			}
		});

		_builtAsciiApp = new AsciiApp(_hostBuilder.Build());
		_services.IsReadOnly = true;

		// Resolve both the _hostBuilder's Configuration and builder.Configuration to mark both as resolved within the
		// service provider ensuring both will be properly disposed with the provider.
		_ = _builtAsciiApp.Services.GetService<IEnumerable<IConfiguration>>();

		return _builtAsciiApp;
	}

	public AsciiApplication Build()
	{
		_builtAsciiApp ??= (AsciiApp)BuildHost();

		if (_builtAsciiApp == null)
			throw new Exception("You must build host first");

		return _builtAsciiApp.Services.GetService<AsciiApplication>() ?? throw new ApplicationException("Failed to generate AsciiApplication");
	}

	internal static string GetDefaultApplicationName()
	{
		var startupAssemblyName = Assembly.GetEntryAssembly()?.GetName()?.Name;
		return startupAssemblyName ?? "AsciiApp";
	}

	private class LoggingBuilder : ILoggingBuilder
	{
        public LoggingBuilder(IServiceCollection services) => Services = services;
        public IServiceCollection Services { get; }
    }

	internal sealed class TrackingChainedConfigurationSource : IConfigurationSource
	{
		private readonly ChainedConfigurationSource _chainedConfigurationSource = new();

		public TrackingChainedConfigurationSource(ConfigurationManager configManager)
		{
			_chainedConfigurationSource.Configuration = configManager;
		}

		public IConfigurationProvider? BuiltProvider { get; set; }

		public IConfigurationProvider Build(IConfigurationBuilder builder)
		{
			BuiltProvider = _chainedConfigurationSource.Build(builder);
			return BuiltProvider;
		}
	}

	internal sealed class ConfigurationProviderSource : IConfigurationSource
	{
		private readonly IConfigurationProvider _configurationProvider;

		public ConfigurationProviderSource(IConfigurationProvider configurationProvider)
		{
			_configurationProvider = configurationProvider;
		}

		public IConfigurationProvider Build(IConfigurationBuilder builder)
		{
			return new IgnoreFirstLoadConfigurationProvider(_configurationProvider);
		}

		// These providers have already been loaded, so no need to reload initially.
		// Otherwise, providers that cannot be reloaded like StreamConfigurationProviders will fail.
		private sealed class IgnoreFirstLoadConfigurationProvider : IConfigurationProvider, IEnumerable<IConfigurationProvider>, IDisposable
		{
			private readonly IConfigurationProvider _provider;
			private bool _hasIgnoredFirstLoad;

			public IgnoreFirstLoadConfigurationProvider(IConfigurationProvider provider)
			{
				_provider = provider;
			}

            public IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string parentPath) => _provider.GetChildKeys(earlierKeys, parentPath);
            public IChangeToken GetReloadToken() => _provider.GetReloadToken();

            public void Load()
			{
				if (!_hasIgnoredFirstLoad)
				{
					_hasIgnoredFirstLoad = true;
					return;
				}

				_provider.Load();
			}

            public void Set(string key, string value) => _provider.Set(key, value);

            public bool TryGet(string key, out string value) => _provider.TryGet(key, out value);

            // Provide access to the original IConfigurationProvider via a single-element IEnumerable to code that goes out of its way to look for it.
            public IEnumerator<IConfigurationProvider> GetEnumerator() => GetUnwrappedEnumerable().GetEnumerator();
			IEnumerator IEnumerable.GetEnumerator() => GetUnwrappedEnumerable().GetEnumerator();
            public override bool Equals(object? obj) => _provider.Equals(obj);
            public override int GetHashCode() => _provider.GetHashCode();
            public override string? ToString() => _provider.ToString();
            public void Dispose() => (_provider as IDisposable)?.Dispose();

            private IEnumerable<IConfigurationProvider> GetUnwrappedEnumerable()
			{
				yield return _provider;
			}
		}
	}
}
