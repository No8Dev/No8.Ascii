using Asciis.Terminal.Helpers;

namespace Asciis.Terminal.Views;

/// <summary>
/// The <see cref="CheckBox"/> <see cref="View"/> shows an on/off toggle that the user can set
/// </summary>
public class CheckBox : View
{
    private string text;
    private int hot_pos = -1;
    private Rune hot_key;

    /// <summary>
    ///   Toggled event, raised when the <see cref="CheckBox"/>  is toggled.
    /// </summary>
    /// <remarks>
    ///   Client code can hook up to this event, it is
    ///   raised when the <see cref="CheckBox"/> is activated either with
    ///   the mouse or the keyboard. The passed <c>bool</c> contains the previous state. 
    /// </remarks>
    public event Action<bool> Toggled;

    /// <summary>
    /// Called when the <see cref="Checked"/> property changes. Invokes the <see cref="Toggled"/> event.
    /// </summary>
    public virtual void OnToggled(bool previousChecked) { Toggled?.Invoke(previousChecked); }

    /// <summary>
    /// Initializes a new instance of <see cref="CheckBox"/> based on the given text, using <see cref="LayoutStyle.Computed"/> layout.
    /// </summary>
    public CheckBox()
        : this(string.Empty) { }

    /// <summary>
    /// Initializes a new instance of <see cref="CheckBox"/> based on the given text, using <see cref="LayoutStyle.Computed"/> layout.
    /// </summary>
    /// <param name="s">S.</param>
    /// <param name="is_checked">If set to <c>true</c> is checked.</param>
    public CheckBox(string s, bool is_checked = false)
        : base()
    {
        Checked = is_checked;
        Text = s;
        CanFocus = true;
        Height = 1;
        Width = s.RuneCount() + 4;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="CheckBox"/> using <see cref="LayoutStyle.Absolute"/> layout.
    /// </summary>
    /// <remarks>
    ///   The size of <see cref="CheckBox"/> is computed based on the
    ///   text length. This <see cref="CheckBox"/> is not toggled.
    /// </remarks>
    public CheckBox(int x, int y, string s)
        : this(x, y, s, false) { }

    /// <summary>
    /// Initializes a new instance of <see cref="CheckBox"/> using <see cref="LayoutStyle.Absolute"/> layout.
    /// </summary>
    /// <remarks>
    ///   The size of <see cref="CheckBox"/> is computed based on the
    ///   text length. 
    /// </remarks>
    public CheckBox(int x, int y, string s, bool is_checked)
        : base(new Rectangle(x, y, s.Length + 4, 1))
    {
        Checked = is_checked;
        Text = s;

        CanFocus = true;
    }

    /// <summary>
    ///    The state of the <see cref="CheckBox"/>
    /// </summary>
    public bool Checked { get; set; }

    /// <summary>
    ///   The text displayed by this <see cref="CheckBox"/>
    /// </summary>
    public new string Text
    {
        get => text;

        set
        {
            text = value;

            var i = 0;
            hot_pos = -1;
            hot_key = (Rune)0;
            var runes = text.ToRuneArray();

            foreach (var r in runes)
            {
                if (r.Value == '_')
                {
                    hot_key = runes[i + 1]; // TODO: <<-= Not sure if theis is correct. 
                    HotKey = (Key)Rune.ToUpperInvariant(hot_key).Value;
                    text = text.Replace("_", "");
                    hot_pos = i;
                    break;
                }

                i++;
            }
        }
    }

    ///<inheritdoc/>
    public override void Redraw(Rectangle bounds)
    {
        Driver.SetAttribute(HasFocus ? ColorScheme.Focus : GetNormalColor());
        Move(0, 0);
        Driver.AddRune(Checked ? Driver.Checked : Driver.UnChecked);
        Driver.AddRune(' ');
        Move(2, 0);
        Driver.AddStr(Text);
        if (hot_pos != -1)
        {
            Move(2 + hot_pos, 0);
            Driver.SetAttribute(
                HasFocus ? ColorScheme.HotFocus : Enabled ? ColorScheme.HotNormal : ColorScheme.Disabled);
            Driver.AddRune(hot_key);
        }
    }

    ///<inheritdoc/>
    public override void PositionCursor() { Move(0, 0); }

    ///<inheritdoc/>
    public override bool ProcessKey(KeyEvent kb)
    {
        if (kb.KeyValue == ' ')
        {
            var previousChecked = Checked;
            Checked = !Checked;
            OnToggled(previousChecked);
            SetNeedsDisplay();
            return true;
        }

        return base.ProcessKey(kb);
    }

    ///<inheritdoc/>
    public override bool ProcessHotKey(KeyEvent ke)
    {
        if (ke.Key == (Key.AltMask | HotKey))
        {
            SetFocus();
            var previousChecked = Checked;
            Checked = !Checked;
            OnToggled(previousChecked);
            SetNeedsDisplay();
            return true;
        }

        return false;
    }

    ///<inheritdoc/>
    public override bool MouseEvent(MouseEvent me)
    {
        if (!me.Flags.HasFlag(MouseFlags.Button1Clicked) || !CanFocus)
            return false;

        SetFocus();
        var previousChecked = Checked;
        Checked = !Checked;
        OnToggled(previousChecked);
        SetNeedsDisplay();

        return true;
    }

    ///<inheritdoc/>
    public override bool OnEnter(View view)
    {
        Application.Driver.SetCursorVisibility(CursorVisibility.Invisible);

        return base.OnEnter(view);
    }
}
