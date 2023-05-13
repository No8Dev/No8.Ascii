using Asciis.Terminal.Views.TextValidateProviders;

namespace Asciis.Terminal.Views;

/// <summary>
/// Text field that validates input through a  <see cref="ITextValidateProvider"/>
/// </summary>
public class TextValidateField : View
{
    private ITextValidateProvider provider;
    private int cursorPosition = 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextValidateField"/> class using <see cref="LayoutStyle.Computed"/> positioning.
    /// </summary>
    public TextValidateField()
        : this(null) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="TextValidateField"/> class using <see cref="LayoutStyle.Computed"/> positioning.
    /// </summary>
    public TextValidateField(ITextValidateProvider provider)
    {
        if (provider != null) Provider = provider;

        Initialize();
    }

    private void Initialize()
    {
        Height = 1;
        CanFocus = true;
    }

    /// <summary>
    /// Provider
    /// </summary>
    public ITextValidateProvider Provider
    {
        get => provider;
        set
        {
            provider = value;
            if (provider.Fixed == true) Width = provider.DisplayText == string.Empty ? 10 : Text.Length;
            HomeKeyHandler();
            SetNeedsDisplay();
        }
    }

    ///<inheritdoc/>
    public override bool MouseEvent(MouseEvent mouseEvent)
    {
        if (mouseEvent.Flags.HasFlag(MouseFlags.Button1Pressed))
        {
            var c = provider.Cursor(mouseEvent.X - GetMargins(Frame.Width).left);
            if (provider.Fixed == false && TextAlignment == TextAlignment.Right && Text.Length > 0) c += 1;
            cursorPosition = c;
            SetFocus();
            SetNeedsDisplay();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Text
    /// </summary>
    public new string Text
    {
        get
        {
            if (provider == null) return string.Empty;

            return provider.Text;
        }
        set
        {
            if (provider == null) return;
            provider.Text = value;

            SetNeedsDisplay();
        }
    }

    ///inheritdoc/>
    public override void PositionCursor()
    {
        var (left, _) = GetMargins(Frame.Width);

        // Fixed = true, is for inputs thar have fixed width, like masked ones.
        // Fixed = false, is for normal input.
        // When it's right-aligned and it's a normal input, the cursor behaves differently.
        if (provider.Fixed == false && TextAlignment == TextAlignment.Right)
            Move(cursorPosition + left - 1, 0);
        else
            Move(cursorPosition + left, 0);
    }

    /// <summary>
    /// Margins for text alignment.
    /// </summary>
    /// <param name="width">Total width</param>
    /// <returns>Left and right margins</returns>
    private (int left, int right) GetMargins(int width)
    {
        var count = Text.Length;
        var total = width - count;
        switch (TextAlignment)
        {
            case TextAlignment.Left:
                return (0, total);
            case TextAlignment.Centered:
                return (total / 2, total / 2 + total % 2);
            case TextAlignment.Right:
                return (total, 0);
            default:
                return (0, total);
        }
    }

    ///<inheritdoc/>
    public override void Redraw(Rectangle bounds)
    {
        if (provider == null)
        {
            Move(0, 0);
            Driver.AddStr("Error: ITextValidateProvider not set!");
            return;
        }

        var bgcolor = !IsValid ? ConsoleColor.Red : ColorScheme.Focus.Background;
        var textColor = new CellAttributes(ColorScheme.Focus.Foreground, bgcolor);

        var (margin_left, margin_right) = GetMargins(bounds.Width);

        Move(0, 0);

        // Left Margin
        Driver.SetAttribute(textColor);
        for (var i = 0; i < margin_left; i++) Driver.AddRune(' ');

        // Content
        Driver.SetAttribute(textColor);
        // Content
        for (var i = 0; i < provider.DisplayText.Length; i++) Driver.AddRune(provider.DisplayText[i]);

        // Right Margin
        Driver.SetAttribute(textColor);
        for (var i = 0; i < margin_right; i++) Driver.AddRune(' ');
    }

    /// <summary>
    /// Try to move the cursor to the left.
    /// </summary>
    /// <returns>True if moved.</returns>
    private bool CursorLeft()
    {
        var current = cursorPosition;
        cursorPosition = provider.CursorLeft(cursorPosition);
        return current != cursorPosition;
    }

    /// <summary>
    /// Try to move the cursor to the right.
    /// </summary>
    /// <returns>True if moved.</returns>
    private bool CursorRight()
    {
        var current = cursorPosition;
        cursorPosition = provider.CursorRight(cursorPosition);
        return current != cursorPosition;
    }

    /// <summary>
    /// Delete char at cursor position - 1, moving the cursor.
    /// </summary>
    /// <returns></returns>
    private bool BackspaceKeyHandler()
    {
        if (provider.Fixed == false && TextAlignment == TextAlignment.Right && cursorPosition <= 1) return false;
        cursorPosition = provider.CursorLeft(cursorPosition);
        provider.Delete(cursorPosition);
        return true;
    }

    /// <summary>
    /// Deletes char at current position.
    /// </summary>
    /// <returns></returns>
    private bool DeleteKeyHandler()
    {
        if (provider.Fixed == false && TextAlignment == TextAlignment.Right)
            cursorPosition = provider.CursorLeft(cursorPosition);
        provider.Delete(cursorPosition);
        return true;
    }

    /// <summary>
    /// Moves the cursor to first char.
    /// </summary>
    /// <returns></returns>
    private bool HomeKeyHandler()
    {
        cursorPosition = provider.CursorStart();
        return true;
    }

    /// <summary>
    /// Moves the cursor to the last char.
    /// </summary>
    /// <returns></returns>
    private bool EndKeyHandler()
    {
        cursorPosition = provider.CursorEnd();
        return true;
    }

    ///<inheritdoc/>
    public override bool ProcessKey(KeyEvent kb)
    {
        if (provider == null) return true;

        switch (kb.Key)
        {
            case Key.Home:
                HomeKeyHandler();
                break;
            case Key.End:
                EndKeyHandler();
                break;
            case Key.Delete:
            case Key.DeleteChar:
                DeleteKeyHandler();
                break;
            case Key.Backspace:
                BackspaceKeyHandler();
                break;
            case Key.CursorLeft:
                CursorLeft();
                break;
            case Key.CursorRight:
                CursorRight();
                break;
            default:
                if (kb.Key < Key.Space || kb.Key > Key.CharMask)
                    return false;

                var key = new Rune((uint)kb.KeyValue);

                var inserted = provider.InsertAt((char)key.Value, cursorPosition);

                if (inserted) CursorRight();

                break;
        }

        SetNeedsDisplay();
        return true;
    }

    /// <summary>
    /// This property returns true if the input is valid.
    /// </summary>
    public virtual bool IsValid
    {
        get
        {
            if (provider == null) return false;

            return provider.IsValid;
        }
    }
}
