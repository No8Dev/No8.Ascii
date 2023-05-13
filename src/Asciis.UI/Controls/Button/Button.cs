using System.Drawing;
using System.Text;

namespace Asciis.UI.Controls;
public class Button : Control
{
    static Button()
    {
        Composer.Register(false, typeof(Button), typeof(ButtonComposer));
    }

    //-= Constructors =------------------------------------------

    public Button(string? name = null) : base(name)
    {
        CanFocus = true;
    }

    public Button(out Button button, string? name = null) : this(name)
    {
        button = this;
    }

    private string? _text;
    private Edges? _border;
    private Color? _borderColor;
    private LineSet? _lineSet;

    public string? Text { get => _text; set => ChangeDirtiesLayout(ref _text, value); }
    public Edges? Border { get => _border; set => ChangeDirtiesLayout(ref _border, value); }
    public Color? BorderColor { get => _borderColor; set => ChangeDirtiesPainting(ref _borderColor, value); }
    public LineSet? LineSet { get => _lineSet; set => ChangeDirtiesPainting(ref _lineSet, value); }

    public Action<Button>? Click;

    public virtual void RaiseClick()
    {
        if (IsEnabled)
            Click?.Invoke(this);
    }

    protected override void AppendProperties(StringBuilder sb)
    {
        base.AppendProperties(sb);
        if (_text != null) sb.Append(" Text=" + _text);
        if (_border != null) sb.Append(" Border=" + _border);
        if (_borderColor != null) sb.Append(" BorderColor=" + _borderColor);
        if (_lineSet != null) sb.Append(" LineSet=" + _lineSet);
    }

    public override string ShortString =>
        base.ShortString + (_text ?? "");
}
