using System.Collections;

namespace Asciis.App.Controls.Animation;

public interface IAnimatable 
{
	public RectF Bounds { get;  }
	public RectF ContentBounds { get; }

	//public LayoutActual Layout { get; }

	public float TranslationX { get; set; }
	public float TranslationY { get; set; }
	public float Scale { get; set; }
	public float ScaleX { get; set; }
	public float ScaleY { get; set; }
	public float Opacity { get; set; }
}

public class Animation : IEnumerable
{
	readonly List<Animation> _children;
	readonly Easing _easing;
	readonly Action? _finished;
	readonly Action<float> _step;
	float _beginAt;
	float _finishAt;
	bool _finishedTriggered;

	public Animation()
	{
		_children = new List<Animation>();
		_easing = Easings.Linear;
		_step = f => { };
	}

	public Animation(
		Action<float> callback, 
		float start = 0.0f, 
		float end = 1.0f, 
		Easing? easing = null, 
		Action? finished = null)
	{
		_children = new List<Animation>();
		_easing = easing ?? Easings.Linear;
		_finished = finished;

		Func<float, float> transform = AnimationExtensions.Interpolate(start, end);
		_step = f => callback(transform(f));
	}

	public IEnumerator GetEnumerator()
	{
		return _children.GetEnumerator();
	}

	public void Add(float beginAt, float finishAt, Animation animation)
	{
		if (beginAt < 0 || beginAt > 1)
			throw new ArgumentOutOfRangeException("beginAt");

		if (finishAt < 0 || finishAt > 1)
			throw new ArgumentOutOfRangeException("finishAt");

		if (finishAt <= beginAt)
			throw new ArgumentException("finishAt must be greater than beginAt");

		animation._beginAt = beginAt;
		animation._finishAt = finishAt;
		_children.Add(animation);
	}

	public void Commit(
		IAnimatable owner, 
		string name, 
		uint rate = 16, 
		uint length = 250, 
		Easing? easing = null, 
		Action<float, bool>? finished = null, 
		Func<bool>? repeat = null)
	{
		owner.Animate(name, this, rate, length, easing, finished, repeat);
	}

	public Action<float> GetCallback()
	{
		Action<float> result = f =>
		{
			_step(_easing.Ease(f));
			foreach (Animation animation in _children)
			{
				if (animation._finishedTriggered)
					continue;

				float val = MathF.Max(0.0f, MathF.Min(1.0f, (f - animation._beginAt) / (animation._finishAt - animation._beginAt)));

				if (val <= 0.0f) // not ready to process yet
					continue;

				Action<float> callback = animation.GetCallback();
				callback(val);

				if (val >= 1.0f)
				{
					animation._finishedTriggered = true;
					if (animation._finished != null)
						animation._finished();
				}
			}
		};
		return result;
	}

	internal void ResetChildren()
	{
		foreach (var anim in _children)
			anim._finishedTriggered = false;
	}

	public Animation Insert(float beginAt, float finishAt, Animation animation)
	{
		Add(beginAt, finishAt, animation);
		return this;
	}

	public Animation WithConcurrent(
		Animation animation, 
		float beginAt = 0.0f, 
		float finishAt = 1.0f)
	{
		animation._beginAt = beginAt;
		animation._finishAt = finishAt;
		_children.Add(animation);
		return this;
	}

	public Animation WithConcurrent(
		Action<float> callback, 
		float start = 0.0f, 
		float end = 1.0f, 
		Easing? easing = null, 
		float beginAt = 0.0f, 
		float finishAt = 1.0f)
	{
		var child = new Animation(callback, start, end, easing);
		child._beginAt = beginAt;
		child._finishAt = finishAt;
		_children.Add(child);
		return this;
	}
}
