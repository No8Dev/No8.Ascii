using System.Diagnostics;

namespace Asciis.App.Controls.Animation;

[DebuggerDisplay("Parallax Value = {Value}")]
public class Parallax : IAnimatable
{
    private class Layer
    {
        public IAnimatable Element { get; set; }
        public float Factor { get; set; }
        public Action<IAnimatable, float>? UpdateAction { get; set; }
        public Action<IAnimatable>? FinishedAction { get; set; }

        public Layer(IAnimatable control)
        {
            Element = control;
        }
    }

    private readonly List<Layer> _layers = new List<Layer>();

    private float _value = 0f;

    public float Value
    {
        get { return _value; }
        set
        {
            if (_value != value)
            {
                _value = value;
                RaiseValueChanged();
            }
        }
    }

    public RectF Bounds => throw new NotImplementedException();

    public RectF ContentBounds => throw new NotImplementedException();

    public float TranslationX { get => 0; set { } }
    public float TranslationY { get => 0; set { } }
    public float Scale { get => 1; set { } }
    public float ScaleX { get => 1; set { } }
    public float ScaleY { get => 1; set { } }
    public float Opacity { get => 1; set { } }

    public event EventHandler? ValueChanged;

    protected virtual void RaiseValueChanged() => ValueChanged?.Invoke(this, EventArgs.Empty);

    public void AddLayer(
        Control element,
        float factor,
        Action<IAnimatable, float> updateAction,
        Action<IAnimatable>? finishedAction = null)
    {
        _layers.Add(new Layer(element)
        {
            Factor = factor,
            UpdateAction = updateAction,
            FinishedAction = finishedAction
        });
    }

    private void UpdateLayers(float value)
    {
        try
        {
            Value = value;
            foreach (var layer in _layers)
                layer.UpdateAction?.Invoke(layer.Element, value * layer.Factor);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            throw;
        }
    }

    private void RaiseFinished()
    {
        try
        {
            foreach (var layer in _layers)
            {
                layer.FinishedAction?.Invoke(layer.Element);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            throw;
        }
    }

    public Task<bool> MoveBy(float value, uint ms, Easing? easing = null)
    {
        Stop();

        var tcs = new TaskCompletionSource<bool>();

        var animation = new Animation();
        animation.WithConcurrent(UpdateLayers, Value, Value + value, easing);
        animation.Commit( this, "", 16U, ms, easing,
            (d, cancelled) =>
            {
                RaiseFinished();
                tcs.SetResult(!cancelled);
            });

        return tcs.Task;
    }

    public void Auto(float value, uint ms)
    {
        Stop();

        var animation = new Animation();
        animation.WithConcurrent(UpdateLayers, Value, Value + value);
        animation.Commit(this, "", 16U, ms, null,
            (d, cancelled) =>
            {
                RaiseFinished();
                if (!cancelled)
                    Auto(value, ms);
            });
    }

    public void Stop()
    {
        this.AbortAnimation("");
    }
}
