namespace Asciis.App.Controls.Animation;

internal class Tweener
{

	long _lastMilliseconds;

	int _timer;
	long _frames;

	public Tweener(uint length)
	{
		Value = 0.0f;
		Length = length;
		Rate = 1;
		Loop = false;
	}

	public Tweener(uint length, uint rate)
	{
		Value = 0.0f;
		Length = length;
		Rate = rate;
		Loop = false;
	}

	public AnimatableKey? Handle { get; set; }
	public event EventHandler? Finished;
	public event EventHandler? ValueUpdated;

	public uint Length { get; }
	public uint Rate { get; }
	public bool Loop { get; set; }
	public float Value { get; private set; }


	public void Pause()
	{
		if (_timer != 0)
		{
			Ticker.Default.Remove(_timer);
			_timer = 0;
		}
	}

	public void Start()
	{
		Pause();

		_lastMilliseconds = 0;
		_frames = 0;

		if (!Ticker.Default.SystemEnabled)
		{
			FinishImmediately();
			return;
		}

		_timer = Ticker.Default.Insert(step =>
		{
			if (step == long.MaxValue)
			{
				// We're being forced to finish
				Value = 1.0f;
			}
			else
			{
				long ms = step + _lastMilliseconds;

				Value = MathF.Min(1.0f, ms / (float)Length);

				_lastMilliseconds = ms;
			}

			long wantedFrames = (_lastMilliseconds / Rate) + 1;
			if (wantedFrames > _frames || Value >= 1.0f)
			{
				ValueUpdated?.Invoke(this, EventArgs.Empty);
			}
			_frames = wantedFrames;

			if (Value >= 1.0f)
			{
				if (Loop)
				{
					_lastMilliseconds = 0;
					Value = 0.0f;
					return true;
				}

				Finished?.Invoke(this, EventArgs.Empty);
				Value = 0.0f;
				_timer = 0;
				return false;
			}
			return true;
		});
	}

	void FinishImmediately()
	{
		Value = 1.0f;
		ValueUpdated?.Invoke(this, EventArgs.Empty);
		Finished?.Invoke(this, EventArgs.Empty);
		Value = 0.0f;
		_timer = 0;
	}

	public void Stop()
	{
		Pause();
		Value = 1.0f;
		Finished?.Invoke(this, EventArgs.Empty);
		Value = 0.0f;
	}


	~Tweener()
	{
		if (_timer != 0)
		{
			try
			{
				Ticker.Default.Remove(_timer);
			}
			catch (InvalidOperationException)
			{
			}
		}
		_timer = 0;
	}
}
