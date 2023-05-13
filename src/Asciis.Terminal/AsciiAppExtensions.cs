using Asciis.Terminal.ConsoleDrivers;
using Asciis.Terminal.ConsoleDrivers.CursesDriver;
using Asciis.Terminal.ConsoleDrivers.FakeDriver;
using Asciis.Terminal.Core;
using Asciis.Terminal.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Asciis.Terminal;

public static class AsciiAppExtensions
{
	public static AsciiAppBuilder UseAsciiApp<
		[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TApp>(
		this AsciiAppBuilder builder)
	where TApp : AsciiApplication
	{
		builder.Services.TryAddSingleton<AsciiApplication, TApp>();
		return builder;
	}

	public static AsciiAppBuilder UseAsciiApp<
		[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TApp>(
		this AsciiAppBuilder builder, Func<IServiceProvider, TApp> implementationFactory)
			where TApp : AsciiApplication
	{
		builder.Services.TryAddSingleton<AsciiApplication>(implementationFactory);

		if (UnitTestDetector.IsRunningFromNUnit)
		{
			builder.Services.TryAddSingleton<ConsoleDriver, CursesDriver>();
			builder.Services.TryAddSingleton<IMainLoopDriver, UnixMainLoop>();
		}
		else if (OperatingSystem.IsWindows())
		{
			builder.Services.TryAddSingleton<ConsoleDriver, WindowsDriver>();
			builder.Services.TryAddSingleton<IMainLoopDriver, WindowsMainLoop>();
		}
		else if (OperatingSystem.IsLinux())
		{
			builder.Services.TryAddSingleton<ConsoleDriver, CursesDriver>();
			builder.Services.TryAddSingleton<IMainLoopDriver, UnixMainLoop>();
		}

		return builder;
	}

	public static AsciiAppBuilder UseCursesConsole(this AsciiAppBuilder builder)
	{
		return builder
			.UseConsoleDriver<CursesDriver>()
			.UseMainLoopDriver<UnixMainLoop>();
	}
	public static AsciiAppBuilder UseFakeConsole(this AsciiAppBuilder builder)
	{
		return builder
			.UseConsoleDriver<FakeConsoleDriver>()
			.UseMainLoopDriver<FakeMainLoopDriver>();
	}
	public static AsciiAppBuilder UseWindowsConsole(this AsciiAppBuilder builder)
	{
		return builder
			.UseConsoleDriver<WindowsDriver>()
			.UseMainLoopDriver<WindowsMainLoop>();
	}
	private static AsciiAppBuilder UseConsoleDriver<T>(
		this AsciiAppBuilder builder)
		where T : ConsoleDriver
	{
		builder.Services.TryAddSingleton<ConsoleDriver, T>();
		return builder;
	}
	private static AsciiAppBuilder UseMainLoopDriver<T>(
		this AsciiAppBuilder builder)
		where T : class, IMainLoopDriver
	{
		builder.Services.TryAddScoped<IMainLoopDriver, T>();
		return builder;
	}

	internal static IHostBuilder ConfigureAsciiDefaults(this IHostBuilder builder, string[]? args)
	{
		builder.UseContentRoot(Directory.GetCurrentDirectory());

		builder.ConfigureHostConfiguration(config =>
		{
			config.AddEnvironmentVariables(prefix: "DOTNET_");
			if (args?.Length > 0)
				config.AddCommandLine(args);
		});

		builder.ConfigureAppConfiguration((hostingContext, config) =>
		{
			IHostEnvironment env = hostingContext.HostingEnvironment;
			bool reloadOnChange = GetReloadConfigOnChangeValue(hostingContext);

			config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: reloadOnChange)
				  .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: reloadOnChange);

			if (env.IsDevelopment() && env.ApplicationName is { Length: > 0 })
			{
				var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
				if (appAssembly is not null)
					config.AddUserSecrets(appAssembly, optional: true, reloadOnChange: reloadOnChange);
			}

			config.AddEnvironmentVariables();

			if (args?.Length > 0)
				config.AddCommandLine(args);
		});

		builder.ConfigureLogging((hostingContext, logging) =>
		{
			bool isWindows =
#if NET6_0_OR_GREATER
					OperatingSystem.IsWindows();
#else
                    RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
#endif

			// IMPORTANT: This needs to be added *before* configuration is loaded, this lets
			// the defaults be overridden by the configuration.
			if (isWindows) // Default the EventLogLoggerProvider to warning or above
				logging.AddFilter<EventLogLoggerProvider>(level => level >= LogLevel.Warning);

			logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
			logging.AddDebug();
			logging.AddEventSourceLogger();

			if (isWindows) // Add the EventLogLoggerProvider on windows machines
				logging.AddEventLog();

			logging.Configure(options =>
			{
				options.ActivityTrackingOptions =
					ActivityTrackingOptions.SpanId |
					ActivityTrackingOptions.TraceId |
					ActivityTrackingOptions.ParentId;
			});

		});

		builder.UseDefaultServiceProvider((context, options) =>
		{
			bool isDevelopment = context.HostingEnvironment.IsDevelopment();
			options.ValidateScopes = isDevelopment;
			options.ValidateOnBuild = isDevelopment;
		});

		return builder;

		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "Calling IConfiguration.GetValue is safe when the T is bool.")]
		static bool GetReloadConfigOnChangeValue(HostBuilderContext hostingContext)
		{
			return hostingContext.Configuration.GetValue("hostBuilder:reloadConfigOnChange", defaultValue: true);
		}
	}
}
