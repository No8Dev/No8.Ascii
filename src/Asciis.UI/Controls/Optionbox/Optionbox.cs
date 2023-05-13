using System.Diagnostics;
using System.Drawing;
using System.Text;

namespace Asciis.UI.Controls;

public class Optionbox : Control, IGroupNavigation
{
    static Optionbox()
    {
        Composer.Register(false, typeof(Optionbox), typeof(OptionboxComposer));
    }

    //-= Constructors =------------------------------------------

    public Optionbox(string? name = null) : base(name)
    {
        CanFocus = true;
        OnPropertyChanging(nameof(HasFocus),
                                       () =>
                                       {
                                           Debug.WriteLine("Changing Focus");
                                       });
        OnPropertyChanged<bool>(
                nameof(HasFocus),
                (old, value) =>
                {
                    if (IsEnabled && HasFocus)
                        Checked = true;
                });
    }

    public Optionbox(out Optionbox checkbox, string? name = null) : this(name)
    {
        checkbox = this;
    }

    private string? _text;
    private string? _groupName;
    private Edges? _border;
    private Color? _borderColor;
    private LineSet? _lineSet;
    private bool? _checked;
    private string? _checkedString;
    private string? _uncheckedString;


    public string? Text { get => _text; set => ChangeDirtiesLayout(ref _text, value); }
    public string? GroupName { get => _groupName; set => ChangeDirtiesLayout(ref _groupName, value); }
    public Edges? Border { get => _border; set => ChangeDirtiesLayout(ref _border, value); }
    public Color? BorderColor { get => _borderColor; set => ChangeDirtiesPainting(ref _borderColor, value); }
    public LineSet? LineSet { get => _lineSet; set => ChangeDirtiesPainting(ref _lineSet, value); }
    public bool? Checked
    {
        get => _checked;
        set
        {
            if (value == true && _checked != value)
                Host?.AllComposersForGroupName(GroupName,
                                     composer =>
                                     {
                                         if (composer.Control is Optionbox optionbox)
                                             optionbox.Checked = false;
                                     });

            ChangeDirtiesPainting(ref _checked, value);
        }
    }

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

    public Action<Optionbox>? Click;

    public virtual void RaiseClick()
    {
        if (IsEnabled)
        {
            Checked = true;
            Click?.Invoke(this);
        }
    }

    protected override void AppendProperties(StringBuilder sb)
    {
        base.AppendProperties(sb);
        if (_checked != null) sb.Append(" Checked=" + _checked);
        if (_groupName != null) sb.Append(" Group=" + _groupName);
        if (_text != null) sb.Append(" Text=" + _text);
        if (_border != null) sb.Append(" Border=" + _border);
        if (_borderColor != null) sb.Append(" BorderColor=" + _borderColor);
        if (_lineSet != null) sb.Append(" LineSet=" + _lineSet);
        if (_checkedString != null) sb.Append(" CheckedStr=" + _checkedString);
        if (_uncheckedString != null) sb.Append(" UncheckedStr=" + _uncheckedString);
    }

    public override string ShortString =>
        base.ShortString + (_text ?? "");

}
