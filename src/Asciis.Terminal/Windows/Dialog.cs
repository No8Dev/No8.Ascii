namespace Asciis.Terminal.Windows;

/// <summary>
/// The <see cref="Dialog"/> <see cref="View"/> is a <see cref="Window"/> that by default is centered and contains one 
/// or more <see cref="Button"/>s. It defaults to the <see cref="Colors.Dialog"/> color scheme and has a 1 cell padding around the edges.
/// </summary>
/// <remarks>
///  To run the <see cref="Dialog"/> modally, create the <see cref="Dialog"/>, and pass it to <see cref="Application.Run(Func{Exception, bool})"/>. 
///  This will execute the dialog until it terminates via the [ESC] or [CTRL-Q] key, or when one of the views
///  or buttons added to the dialog calls <see cref="Application.RequestStop"/>.
/// </remarks>
public class Dialog : Window
{
    private List<Button> buttons = new();
    private const int padding = 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="Dialog"/> class using <see cref="LayoutStyle.Computed"/> positioning 
    /// and an optional set of <see cref="Button"/>s to display
    /// </summary>
    /// <param name="title">Title for the dialog.</param>
    /// <param name="width">Width for the dialog.</param>
    /// <param name="height">Height for the dialog.</param>
    /// <param name="buttons">Optional buttons to lay out at the bottom of the dialog.</param>
    /// <remarks>
    /// if <c>width</c> and <c>height</c> are both 0, the Dialog will be vertically and horizontally centered in the
    /// container and the size will be 85% of the container. 
    /// After initialization use <c>X</c>, <c>Y</c>, <c>Width</c>, and <c>Height</c> to override this with a location or size.
    /// </remarks>
    /// <remarks>
    /// Use the constructor that does not take a <c>width</c> and <c>height</c> instead.
    /// </remarks>
    public Dialog(AsciiApplication application, string title, int width, int height, params Button[] buttons)
        : base(title, padding)
    {
        Application = application;
        X = Pos.Center();
        Y = Pos.Center();

        if ((width == 0) & (height == 0))
        {
            Width = Dim.Percent(85);
            Height = Dim.Percent(85);
        }
        else
        {
            Width = width;
            Height = height;
        }

        ColorScheme = Colors.Dialog;
        Modal = true;
        Border.Effect3D = true;

        if (buttons != null)
            foreach (var b in buttons)
            {
                this.buttons.Add(b);
                Add(b);
            }

        LayoutStarted += (args) => { LayoutStartedHandler(); };
    }

    public Dialog(AsciiApplication application) 
        : this(application, string.Empty, 0, 0)
    {  }

    public Dialog(AsciiApplication application, string title, params Button[] buttons) 
        : this(application, title, 0, 0, buttons)
    {  }


    /// <summary>
    /// Adds a <see cref="Button"/> to the <see cref="Dialog"/>, its layout will be controlled by the <see cref="Dialog"/>
    /// </summary>
    /// <param name="button">Button to add.</param>
    public void AddButton(Button button)
    {
        if (button == null)
            return;

        button.Application = Application;
        buttons.Add(button);
        Add(button);
        SetNeedsDisplay();
        LayoutSubviews();
    }

    internal int GetButtonsWidth()
    {
        if (buttons.Count == 0) return 0;
        return buttons.Select(b => b.Bounds.Width).Sum() + buttons.Count - 1;
    }

    private void LayoutStartedHandler()
    {
        var buttonsWidth = GetButtonsWidth();

        var shiftLeft = Math.Max((Bounds.Width - buttonsWidth) / 2 - 2, 0);
        for (var i = buttons.Count - 1; i >= 0; i--)
        {
            Button button = buttons[i];
            shiftLeft += button.Frame.Width + 1;
            button.X = Pos.AnchorEnd(shiftLeft);
            button.Y = Pos.AnchorEnd(1);
        }
    }

    ///<inheritdoc/>
    public override bool ProcessKey(KeyEvent kb)
    {
        switch (kb.Key)
        {
            case Key.Esc:
                Running = false;
                return true;
        }

        return base.ProcessKey(kb);
    }
}
