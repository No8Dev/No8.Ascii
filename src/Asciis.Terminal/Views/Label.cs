namespace Asciis.Terminal.Views;

/// <summary>
/// The Label <see cref="View"/> displays a string at a given position and supports multiple lines separated by newline characters.
/// Multi-line Labels support word wrap.
/// </summary>
/// <remarks>
/// The <see cref="Label"/> view is functionality identical to <see cref="View"/> and is included for API backwards compatibility.
/// </remarks>
public class Label : View
{
    /// <inheritdoc/>
    public Label() { Initialize(); }

    /// <inheritdoc/>
    public Label(Rectangle frame)
        : base(frame) { }

    /// <inheritdoc/>
    public Label(string text)
        : base(text)
    {
        Initialize();
    }

    /// <inheritdoc/>
    public Label(Rectangle rect, string text)
        : base(rect, text) { }

    /// <inheritdoc/>
    public Label(int x, int y, string text)
        : base(x, y, text)
    {
        Initialize();
    }

    /// <inheritdoc/>
    public Label(string text, TextDirection direction)
        : base(text, direction)
    {
        Initialize();
    }

    private void Initialize() { AutoSize = true; }

    /// <summary>
    ///   Clicked <see cref="Action"/>, raised when the user clicks the primary mouse button within the Bounds of this <see cref="View"/>
    ///   or if the user presses the action key while this view is focused. (TODO: IsDefault)
    /// </summary>
    /// <remarks>
    ///   Client code can hook up to this event, it is
    ///   raised when the button is activated either with
    ///   the mouse or the keyboard.
    /// </remarks>
    public event Action Clicked;

    ///// <inheritdoc/>
    //public new string Text {
    //	get => base.Text;
    //	set {
    //		base.Text = value;
    //		// This supports Label auto-sizing when Text changes (preserving backwards compat behavior)
    //		if (Frame.Height == 1 && !string.IsNullOrEmpty (value)) {
    //			int w = Text.RuneCount;
    //			Width = w;
    //			Frame = new Rect (Frame.Location, new Size (w, Frame.Height));
    //		}
    //		SetNeedsDisplay ();
    //	}
    //}

    /// <summary>
    /// Method invoked when a mouse event is generated
    /// </summary>
    /// <param name="mouseEvent"></param>
    /// <returns><c>true</c>, if the event was handled, <c>false</c> otherwise.</returns>
    public override bool OnMouseEvent(MouseEvent mouseEvent)
    {
        MouseEventArgs args = new(mouseEvent);
        if (OnMouseClick(args))
            return true;
        if (MouseEvent(mouseEvent))
            return true;

        if (mouseEvent.Flags == MouseFlags.Button1Clicked)
        {
            if (!HasFocus && SuperView != null)
            {
                if (!SuperView.HasFocus) SuperView.SetFocus();
                SetFocus();
                SetNeedsDisplay();
            }

            Clicked?.Invoke();
            return true;
        }

        return false;
    }

    ///<inheritdoc/>
    public override bool OnEnter(View view)
    {
        Application.Driver.SetCursorVisibility(CursorVisibility.Invisible);

        return base.OnEnter(view);
    }
}
