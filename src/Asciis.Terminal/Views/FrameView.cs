namespace Asciis.Terminal.Views;

/// <summary>
/// The FrameView is a container frame that draws a frame around the contents. It is similar to
/// a GroupBox in Windows.
/// // NOTE: FrameView is functionally identical to Window with the following exceptions. 
///  - Is not a Toplevel
///  - Does not support mouse dragging
///  - Does not support padding (but should)
///  - Does not support IEnumerable
/// Any udpates done here should probably be done in Window as well; TODO: Merge these classes
/// </summary>
public class FrameView : View
{
    private View contentView;
    private string title;

    /// <summary>
    /// The title to be displayed for this <see cref="FrameView"/>.
    /// </summary>
    /// <value>The title.</value>
    public string Title
    {
        get => title;
        set
        {
            title = value;
            SetNeedsDisplay();
        }
    }

    /// <inheritdoc/>
    public override Border Border
    {
        get => base.Border;
        set
        {
            if (base.Border != null && base.Border.Child != null && value.Child == null)
                value.Child = base.Border.Child;
            base.Border = value;
            if (value == null) 
                return;
            Rectangle frame;
            if (contentView != null && (contentView.Width is Dim || contentView.Height is Dim))
                frame = Rectangle.Empty;
            else
                frame = Frame;
            AdjustContentView(frame);

            Border.BorderChanged += Border_BorderChanged;
        }
    }

    private void Border_BorderChanged(Border border)
    {
        Rectangle frame;
        if (contentView != null && (contentView.Width is Dim || contentView.Height is Dim))
            frame = Rectangle.Empty;
        else
            frame = Frame;
        AdjustContentView(frame);
    }

    protected override void ApplicationChanged()
    {
        base.ApplicationChanged();
        if (contentView != null)
            contentView.Application = Application;
    }

    /// <summary>
    /// ContentView is an internal implementation detail of Window. It is used to host Views added with <see cref="Add(View)"/>. 
    /// Its ONLY reason for being is to provide a simple way for Window to expose to those SubViews that the Window's Bounds 
    /// are actually deflated due to the border. 
    /// </summary>
    private class ContentView : View
    {
        public ContentView(Rectangle frame)
            : base(frame) { }

        public ContentView()
            : base() { }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FrameView"/> class using <see cref="LayoutStyle.Absolute"/> layout.
    /// </summary>
    /// <param name="frame">Frame.</param>
    /// <param name="title">Title.</param>
    /// <param name="views">Views.</param>
    /// <param name="border">The <see cref="Border"/>.</param>
    public FrameView(Rectangle frame, string title = null, View[] views = null, Border border = null)
        : base(frame)
    {
        //var cFrame = new Rect (1, 1, Math.Max (frame.Width - 2, 0), Math.Max (frame.Height - 2, 0));
        Initialize(frame, title, views, border);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FrameView"/> class using <see cref="LayoutStyle.Computed"/> layout.
    /// </summary>
    /// <param name="title">Title.</param>
    /// <param name="border">The <see cref="Border"/>.</param>
    public FrameView(string title, Border border = null) { Initialize(Rectangle.Empty, title, null, border); }

    /// <summary>
    /// Initializes a new instance of the <see cref="FrameView"/> class using <see cref="LayoutStyle.Computed"/> layout.
    /// </summary>
    public FrameView()
        : this(string.Empty) { }

    private void Initialize(Rectangle frame, string title, View[] views = null, Border border = null)
    {
        this.title = title;
        if (border == null)
        {
            Border = new Border()
            {
                Application = Application,
                BorderStyle = BorderStyle.Single,
            };
        }
        else
        {
            Border = border;
            Border.Application = Application;
        }
        AdjustContentView(frame, views);
    }

    private void AdjustContentView(Rectangle frame, View[] views = null)
    {
        var borderLength = Border.DrawMarginFrame ? 1 : 0;
        var sumPadding = Border.GetSumThickness();
        var wb = new Size();
        if (frame == Rectangle.Empty)
        {
            wb.Width = borderLength + sumPadding.Right;
            wb.Height = borderLength + sumPadding.Bottom;
            if (contentView == null)
            {
                contentView = new ContentView()
                {
                    X = borderLength + sumPadding.Left,
                    Y = borderLength + sumPadding.Top,
                    Width = Dim.Fill(wb.Width),
                    Height = Dim.Fill(wb.Height),
                };
            }
            else
            {
                contentView.X = borderLength + sumPadding.Left;
                contentView.Y = borderLength + sumPadding.Top;
                contentView.Width = Dim.Fill(wb.Width);
                contentView.Height = Dim.Fill(wb.Height);
            }
        }
        else
        {
            wb.Width = 2 * borderLength + sumPadding.Right + sumPadding.Left;
            wb.Height = 2 * borderLength + sumPadding.Bottom + sumPadding.Top;
            var cFrame = new Rectangle(
                borderLength + sumPadding.Left,
                borderLength + sumPadding.Top,
                frame.Width - wb.Width,
                frame.Height - wb.Height);
            if (contentView == null)
                contentView = new ContentView(cFrame);
            else
                contentView.Frame = cFrame;
        }
        contentView.Application = Application;

        if (views != null)
        {
            foreach (var view in views)
                contentView.Add(view);
        }
        if (Subviews?.Count == 0)
        {
            base.Add(contentView);
            contentView.Text = base.Text;
        }

        Border.Child = contentView;
    }

    private void DrawFrame() { DrawFrame(new Rectangle(0, 0, base.Frame.Width, base.Frame.Height), 0, true); }

    /// <summary>
    /// Add the specified <see cref="View"/> to this container.
    /// </summary>
    /// <param name="view"><see cref="View"/> to add to this container</param>
    public override void Add(View view)
    {
        contentView.Add(view);
        if (view.CanFocus)
            CanFocus = true;
    }


    /// <summary>
    ///   Removes a <see cref="View"/> from this container.
    /// </summary>
    /// <remarks>
    /// </remarks>
    public override void Remove(View view)
    {
        if (view == null)
            return;

        SetNeedsDisplay();
        var touched = view.Frame;
        contentView.Remove(view);

        if (contentView.InternalSubviews.Count < 1)
            CanFocus = false;
    }

    /// <summary>
    ///   Removes all <see cref="View"/>s from this container.
    /// </summary>
    /// <remarks>
    /// </remarks>
    public override void RemoveAll() { contentView.RemoveAll(); }

    ///<inheritdoc/>
    public override void Redraw(Rectangle bounds)
    {
        var padding = Border.GetSumThickness();
        var scrRect = ViewToScreen(new Rectangle(0, 0, base.Frame.Width, base.Frame.Height));

        if (!NeedDisplay.IsEmpty)
        {
            Driver.SetAttribute(GetNormalColor());
            //Driver.DrawWindowFrame (scrRect, padding + 1, padding + 1, padding + 1, padding + 1, border: true, fill: true);
            Border.DrawContent();
        }

        var savedClip = contentView.ClipToBounds();
        contentView.Redraw(contentView.Bounds);
        Driver.Clip = savedClip;

        ClearNeedsDisplay();
        if (Border.BorderStyle != BorderStyle.None)
        {
            Driver.SetAttribute(GetNormalColor());
            //Driver.DrawWindowFrame (scrRect, padding + 1, padding + 1, padding + 1, padding + 1, border: true, fill: false);
            if (HasFocus)
                Driver.SetAttribute(ColorScheme.HotNormal);
            //Driver.DrawWindowTitle (scrRect, Title, padding, padding, padding, padding);
            Driver.DrawWindowTitle(scrRect, Title, padding.Left, padding.Top, padding.Right, padding.Bottom);
        }

        Driver.SetAttribute(GetNormalColor());
    }

    /// <summary>
    ///   The text displayed by the <see cref="Label"/>.
    /// </summary>
    public override string Text
    {
        get => contentView.Text;
        set
        {
            base.Text = value;
            if (contentView != null) contentView.Text = value;
        }
    }

    /// <summary>
    /// Controls the text-alignment property of the label, changing it will redisplay the <see cref="Label"/>.
    /// </summary>
    /// <value>The text alignment.</value>
    public override TextAlignment TextAlignment
    {
        get => contentView.TextAlignment;
        set => base.TextAlignment = contentView.TextAlignment = value;
    }

    ///<inheritdoc/>
    public override bool OnEnter(View view)
    {
        if (Subviews.Count == 0 || !Subviews.Any(subview => subview.CanFocus))
            Application.Driver?.SetCursorVisibility(CursorVisibility.Invisible);

        return base.OnEnter(view);
    }
}
