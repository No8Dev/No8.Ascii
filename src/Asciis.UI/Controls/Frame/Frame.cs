using System.Drawing;
using System.Text;

namespace Asciis.UI.Controls;

public class Frame : Control
{
    static Frame()
    {
        Composer.Register(false, typeof(Frame), typeof(FrameComposer));
    }

    private string? _text;
    private Edges? _border;
    private Color? _borderColor;
    private LineSet? _lineSet;

    public string? Text { get => _text; set => ChangeDirtiesLayout(ref _text, value); }
    public Edges? Border { get => _border; set => ChangeDirtiesLayout(ref _border, value); }
    public Color? BorderColor { get => _borderColor; set => ChangeDirtiesPainting(ref _borderColor, value); }
    public LineSet? LineSet { get => _lineSet; set => ChangeDirtiesPainting(ref _lineSet, value); }

    public Frame(Frame other)
        : base(other)
    {
        _text        = other._text;
        _border      = other._border;
        _borderColor = other._borderColor;
        _lineSet     = other._lineSet;
    }
    public Frame(string?    name = null) : base(name) { }
    public Frame(LayoutPlan plan) : base(plan) { }
    public Frame(string? name, LayoutPlan? plan = null) : base(name, plan) { }

    public Frame(
        out Frame   node,
        LayoutPlan? plan,
        string?     name = null)
        : base(name, plan)
    {
        node = this;
    }

    public Frame(out Frame frame, string? name = null) : this(name)
    {
        frame = this;
    }
    
    protected override void AppendProperties(StringBuilder sb)
    {
        base.AppendProperties(sb);
        if (_text != null) sb.Append(" Text=" + _text);
        if (_border != null) sb.Append(" Border=" + _border);
        if (_borderColor != null) sb.Append(" BorderColor=" + _borderColor);
        if (_lineSet != null) sb.Append(" LineSet=" + _lineSet);
    }
}
