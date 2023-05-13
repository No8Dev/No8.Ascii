using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

namespace No8.Ascii.Controls.Internals;

[EditorBrowsable(EditorBrowsableState.Never)]
public class Ticker
{
	static Ticker? s_ticker = null;

	private Timer? _timer;
	readonly Stopwatch _stopwatch;
	readonly List<(int tick, Func<long, bool> func)> _timeouts;

	int _count;
	bool _enabled;

	protected Ticker()
	{
		_count = 0;
		_timeouts = new();
		_stopwatch = new Stopwatch();
	}

	// Some devices may suspend the services we use for the ticker (e.g., in power save mode)
	// The native implementations can override this value as needed
	public virtual bool SystemEnabled => true;

	// Native ticker implementations can let us know that the ticker has been enabled/disabled by the system 
	protected void OnSystemEnabledChanged()
	{
		if (!SystemEnabled)
		{
			// Something (possibly power save mode) has disabled the ticker; tell all the current in-progress
			// timeouts to finish
			SendFinish();
		}
	}

	public static void SetDefault(Ticker ticker) => Default = ticker;
	public static Ticker Default
	{
		internal set
		{
			if (value == null && s_ticker != null)
				(s_ticker as IDisposable)?.Dispose();
			s_ticker = value;
		}
		get
		{
			s_ticker ??= new();
			return s_ticker.GetTickerInstance();
		}
	}

	protected virtual Ticker GetTickerInstance()
	{
		// This method is provided so platforms can override it and return something other than
		// the normal Ticker singleton
		return s_ticker!;
	}

	public virtual int Insert(Func<long, bool> timeout)
	{
		_count++;
		_timeouts.Add(new (_count, timeout));

		if (!_enabled)
		{
			_enabled = true;
			Enable();
		}

		return _count;
	}

	public virtual void Remove(int handle)
	{
		// TODO: On Main Thread?
		RemoveTimeout(handle);
	}

	void RemoveTimeout(int handle)
	{
		_timeouts.RemoveAll(t => t.Item1 == handle);

		if (_timeouts.Count == 0)
		{
			_enabled = false;
			Disable();
		}
	}

	protected virtual void DisableTimer()
    {
		_timer?.Dispose();
	}

	protected virtual void EnableTimer()
    {
		_timer = new(
			_ => SendSignals(),
			null,
			TimeSpan.FromMilliseconds(15), TimeSpan.FromMilliseconds(15));
	}

	protected void SendFinish()
	{
		SendSignals(long.MaxValue);
	}

	protected void SendSignals(int timestep = -1)
	{
		long step = timestep >= 0
			? timestep
			: _stopwatch.ElapsedMilliseconds;

		SendSignals(step);
	}

	protected void SendSignals(long step)
	{
		_stopwatch.Reset();
		_stopwatch.Start();

		foreach ((int tick, Func<long, bool> func) in _timeouts.ToArray())
		{
			bool remove = !func(step);
			if (remove)
				_timeouts.RemoveAll(t => t.tick == tick);
		}

		if (_timeouts.Count == 0)
		{
			_enabled = false;
			Disable();
		}
	}

	void Disable()
	{
		_stopwatch.Reset();
		DisableTimer();
	}

	void Enable()
	{
		_stopwatch.Start();
		EnableTimer();
	}
}
