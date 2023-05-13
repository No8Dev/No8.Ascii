namespace No8.Ascii.Controls.Animation;

internal class AnimatableKey
{
	public AnimatableKey(IAnimatable animatable, string handle)
	{
		if (animatable == null)
			throw new ArgumentNullException(nameof(animatable));

		if (string.IsNullOrEmpty(handle))
			throw new ArgumentException("Argument is null or empty", nameof(handle));

		Animatable = new WeakReference<IAnimatable>(animatable);
		Handle = handle;
	}

	public WeakReference<IAnimatable> Animatable { get; }

	public string Handle { get; }

	public override bool Equals(object? obj)
	{
		if (ReferenceEquals(null, obj))
		{
			return false;
		}
		if (ReferenceEquals(this, obj))
		{
			return true;
		}
		if (obj.GetType() != GetType())
		{
			return false;
		}
		return Equals((AnimatableKey)obj);
	}

	public override int GetHashCode()
	{
		unchecked
		{
			if (!Animatable.TryGetTarget(out var target))
				return Handle?.GetHashCode() ?? 0;

			return ((target?.GetHashCode() ?? 0) * 397) ^ (Handle?.GetHashCode() ?? 0);
		}
	}

	protected bool Equals(AnimatableKey other)
	{
		if (!string.Equals(Handle, other.Handle))
			return false;

		if (!Animatable.TryGetTarget(out var thisAnimatable))
			return false;

		if (!other.Animatable.TryGetTarget(out var thatAnimatable))
			return false;

		return Equals(thisAnimatable, thatAnimatable);
	}
}
