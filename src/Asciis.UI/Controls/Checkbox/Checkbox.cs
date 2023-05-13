using System.Drawing;
using System.Text;

namespace Asciis.UI.Controls;

public class Checkbox : Control
{
    static Checkbox()
    {
        Composer.Register(false, typeof(Checkbox), typeof(CheckboxComposer));
    }

    //-= Constructors =------------------------------------------

    public Checkbox(string? name = null) : base(name)
    {
        CanFocus = true;
    }

    public Checkbox(out Checkbox checkbox, string? name = null) : this(name)
    {
        checkbox = this;
    }

    private string? _text;
    private Edges? _border;
    private Color? _borderColor;
    private LineSet? _lineSet;
    private bool? _checked;
    private string? _checkedString;
    private string? _uncheckedString;
    private string? _unknownString;


    public string? Text { get => _text; set => ChangeDirtiesLayout(ref _text, value); }
    public Edges? Border { get => _border; set => ChangeDirtiesLayout(ref _border, value); }
    public Color? BorderColor { get => _borderColor; set => ChangeDirtiesPainting(ref _borderColor, value); }
    public LineSet? LineSet { get => _lineSet; set => ChangeDirtiesPainting(ref _lineSet, value); }
    public bool? Checked { get => _checked; set => ChangeDirtiesPainting(ref _checked, value); }
    public string? CheckedString
    {
        get => _checkedString;
        set => ChangeDirtiesLayout(ref _checkedString, value);
    }
    public string? UncheckedString
    {
        get => _uncheckedString;
        set => ChangeDirtiesLayout(ref _uncheckedString, value);
    }
    public string? UnknownString
    {
        get => _unknownString;
        set => ChangeDirtiesLayout(ref _unknownString, value);
    }

    public Action<Checkbox>? Click;

    public virtual void RaiseClick()
    {
        if (IsEnabled)
        {
            Checked = Checked == null ? true : !Checked;
            Click?.Invoke(this);
        }
    }

    protected override void AppendProperties(StringBuilder sb)
    {
        base.AppendProperties(sb);
        if (_checked != null) sb.Append(" Checked=" + _checked);
        if (_text != null) sb.Append(" Text=" + _text);
        if (_border != null) sb.Append(" Border=" + _border);
        if (_borderColor != null) sb.Append(" BorderColor=" + _borderColor);
        if (_lineSet != null) sb.Append(" LineSet=" + _lineSet);
        if (_checkedString != null) sb.Append(" CheckedStr=" + _checkedString);
        if (_uncheckedString != null) sb.Append(" UncheckedStr=" + _uncheckedString);
        if (_unknownString != null) sb.Append(" UnknownString=" + _unknownString);
    }

    public override string ShortString =>
        base.ShortString + (_text ?? "");
}
