namespace Asciis.Terminal.Views;

/// <summary>
/// Specifies the style that a <see cref="ProgressBar"/> uses to indicate the progress of an operation.
/// </summary>
public enum ProgressBarStyle
{
    /// <summary>
    /// Indicates progress by increasing the number of segmented blocks in a <see cref="ProgressBar"/>.
    /// </summary>
    Blocks,

    /// <summary>
    /// Indicates progress by increasing the size of a smooth, continuous bar in a <see cref="ProgressBar"/>.
    /// </summary>
    Continuous,

    /// <summary>
    /// Indicates progress by continuously scrolling a block across a <see cref="ProgressBar"/> in a marquee fashion.
    /// </summary>
    MarqueeBlocks,

    /// <summary>
    /// Indicates progress by continuously scrolling a block across a <see cref="ProgressBar"/> in a marquee fashion.
    /// </summary>
    MarqueeContinuous
}

/// <summary>
///Specifies the format that a <see cref="ProgressBar"/> uses to indicate the visual presentation.
/// </summary>
public enum ProgressBarFormat
{
    /// <summary>
    /// A simple visual presentation showing only the progress bar.
    /// </summary>
    Simple,

    /// <summary>
    /// A simple visual presentation showing the progress bar and the percentage.
    /// </summary>
    SimplePlusPercentage,

    /// <summary>
    /// A framed visual presentation showing only the progress bar.
    /// </summary>
    Framed,

    /// <summary>
    /// A framed visual presentation showing the progress bar and the percentage.
    /// </summary>
    FramedPlusPercentage,

    /// <summary>
    /// A framed visual presentation showing all with the progress bar padded.
    /// </summary>
    FramedProgressPadded
}

/// <summary>
/// A Progress Bar view that can indicate progress of an activity visually.
/// </summary>
/// <remarks>
///   <para>
///     <see cref="ProgressBar"/> can operate in two modes, percentage mode, or
///     activity mode.  The progress bar starts in percentage mode and
///     setting the Fraction property will reflect on the UI the progress 
///     made so far.   Activity mode is used when the application has no 
///     way of knowing how much time is left, and is started when the <see cref="Pulse"/> method is called.  
///     Call <see cref="Pulse"/> repeatedly as progress is made.
///   </para>
/// </remarks>
public class ProgressBar : View
{
    private bool isActivity;
    private int[] activityPos;
    private int delta, padding;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProgressBar"/> class, starts in percentage mode with an absolute position and size.
    /// </summary>
    /// <param name="rect">Rect.</param>
    public ProgressBar(Rectangle rect)
        : base(rect)
    {
        Initialize(rect);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProgressBar"/> class, starts in percentage mode and uses relative layout.
    /// </summary>
    public ProgressBar()
        : base()
    {
        Initialize(Rectangle.Empty);
    }

    private void Initialize(Rectangle rect)
    {
        CanFocus = false;
        fraction = 0;
        ColorScheme = new ColorScheme()
        {
            Normal = new (ConsoleColor.Green, ConsoleColor.Gray),
            HotNormal = Colors.Base.Normal
        };
        if (rect.IsEmpty) Height = 1;
    }

    private float fraction;

    /// <summary>
    /// Gets or sets the <see cref="ProgressBar"/> fraction to display, must be a value between 0 and 1.
    /// </summary>
    /// <value>The fraction representing the progress.</value>
    public float Fraction
    {
        get => fraction;
        set
        {
            fraction = Math.Min(value, 1);
            isActivity = false;
            SetNeedsDisplay();
        }
    }

    private ProgressBarStyle progressBarStyle;

    /// <summary>
    /// Gets/Sets the progress bar style based on the <see cref="Views.ProgressBarStyle"/>
    /// </summary>
    public ProgressBarStyle ProgressBarStyle
    {
        get => progressBarStyle;
        set
        {
            progressBarStyle = value;
            switch (value)
            {
                case ProgressBarStyle.Blocks:
                    SegmentCharacter = Driver.BlocksMeterSegment;
                    break;
                case ProgressBarStyle.Continuous:
                    SegmentCharacter = Driver.ContinuousMeterSegment;
                    break;
                case ProgressBarStyle.MarqueeBlocks:
                    SegmentCharacter = Driver.BlocksMeterSegment;
                    break;
                case ProgressBarStyle.MarqueeContinuous:
                    SegmentCharacter = Driver.ContinuousMeterSegment;
                    break;
            }

            SetNeedsDisplay();
        }
    }

    private ProgressBarFormat progressBarFormat;

    /// <summary>
    /// Specifies the format that a <see cref="ProgressBar"/> uses to indicate the visual presentation.
    /// </summary>
    public ProgressBarFormat ProgressBarFormat
    {
        get => progressBarFormat;
        set
        {
            progressBarFormat = value;
            switch (progressBarFormat)
            {
                case ProgressBarFormat.Simple:
                    Height = 1;
                    break;
                case ProgressBarFormat.SimplePlusPercentage:
                    Height = 2;
                    break;
                case ProgressBarFormat.Framed:
                    Height = 3;
                    break;
                case ProgressBarFormat.FramedPlusPercentage:
                    Height = 4;
                    break;
                case ProgressBarFormat.FramedProgressPadded:
                    Height = 6;
                    break;
            }

            SetNeedsDisplay();
        }
    }

    private Rune? segmentCharacter;

    /// <summary>
    /// Segment indicator for meter views.
    /// </summary>
    public Rune SegmentCharacter
    {
        get => segmentCharacter ??= Application.Driver.BlocksMeterSegment;
        set
        {
            segmentCharacter = value;
            SetNeedsDisplay();
        }
    }

    ///<inheritdoc/>
    public override string Text
    {
        get => GetPercentageText();
        set => base.Text = SetPercentageText(value);
    }

    private bool bidirectionalMarquee = true;

    /// <summary>
    /// Specifies if the <see cref="ProgressBarStyle.MarqueeBlocks"/> or the
    ///  <see cref="ProgressBarStyle.MarqueeContinuous"/> styles is unidirectional
    ///  or bidirectional.
    /// </summary>
    public bool BidirectionalMarquee
    {
        get => bidirectionalMarquee;
        set
        {
            bidirectionalMarquee = value;
            SetNeedsDisplay();
        }
    }

    private string GetPercentageText()
    {
        switch (progressBarStyle)
        {
            case ProgressBarStyle.Blocks:
            case ProgressBarStyle.Continuous:
                return $"{fraction * 100:F0}%";
            case ProgressBarStyle.MarqueeBlocks:
            case ProgressBarStyle.MarqueeContinuous:
                break;
        }

        return base.Text;
    }

    private string SetPercentageText(string value)
    {
        switch (progressBarStyle)
        {
            case ProgressBarStyle.Blocks:
            case ProgressBarStyle.Continuous:
                return $"{fraction * 100:F0}%";
            case ProgressBarStyle.MarqueeBlocks:
            case ProgressBarStyle.MarqueeContinuous:
                break;
        }

        return value;
    }

    /// <summary>
    /// Notifies the <see cref="ProgressBar"/> that some progress has taken place.
    /// </summary>
    /// <remarks>
    /// If the <see cref="ProgressBar"/> is percentage mode, it switches to activity
    /// mode. If is in activity mode, the marker is moved.
    /// </remarks>
    public void Pulse()
    {
        if (activityPos == null) PopulateActivityPos();
        if (!isActivity)
        {
            isActivity = true;
            delta = 1;
        }
        else
        {
            for (var i = 0; i < activityPos.Length; i++) activityPos[i] += delta;
            var fWidth = GetFrameWidth();
            if (activityPos[activityPos.Length - 1] < 0)
            {
                for (var i = 0; i < activityPos.Length; i++) activityPos[i] = i - activityPos.Length + 2;
                delta = 1;
            }
            else if (activityPos[0] >= fWidth)
            {
                if (bidirectionalMarquee)
                {
                    for (var i = 0; i < activityPos.Length; i++) activityPos[i] = fWidth + i - 2;
                    delta = -1;
                }
                else
                {
                    PopulateActivityPos();
                }
            }
        }

        SetNeedsDisplay();
    }

    ///<inheritdoc/>
    public override void Redraw(Rectangle region)
    {
        DrawFrame();

        Driver.SetAttribute(GetNormalColor());

        var fWidth = GetFrameWidth();
        if (isActivity)
        {
            Move(padding, padding);
            for (var i = 0; i < fWidth; i++)
                if (Array.IndexOf(activityPos, i) != -1)
                    Driver.AddRune(SegmentCharacter);
                else
                    Driver.AddRune(' ');
        }
        else
        {
            Move(padding, padding);
            var mid = (int)(fraction * fWidth);
            int i;
            for (i = 0; (i < mid) & (i < fWidth); i++)
                Driver.AddRune(SegmentCharacter);
            for (; i < fWidth; i++)
                Driver.AddRune(' ');
        }

        DrawText(fWidth);
    }

    private int GetFrameWidth()
    {
        switch (progressBarFormat)
        {
            case ProgressBarFormat.Simple:
            case ProgressBarFormat.SimplePlusPercentage:
                break;
            case ProgressBarFormat.Framed:
            case ProgressBarFormat.FramedPlusPercentage:
                return Frame.Width - 2;
            case ProgressBarFormat.FramedProgressPadded:
                return Frame.Width - 2 - padding;
        }

        return Frame.Width;
    }

    private void DrawText(int fWidth)
    {
        switch (progressBarFormat)
        {
            case ProgressBarFormat.Simple:
            case ProgressBarFormat.Framed:
                break;
            case ProgressBarFormat.SimplePlusPercentage:
            case ProgressBarFormat.FramedPlusPercentage:
            case ProgressBarFormat.FramedProgressPadded:
                var tf = new TextFormatter()
                {
                    Alignment = TextAlignment.Centered,
                    Text = Text
                };
                var row = padding + (progressBarFormat == ProgressBarFormat.FramedProgressPadded
                    ? 2
                    : 1);
                Move(padding, row);
                var rect = new Rectangle(padding, row, fWidth, base.Frame.Height);
                tf?.Draw(Application.Driver, ViewToScreen(rect), ColorScheme.HotNormal, ColorScheme.HotNormal);
                break;
        }
    }

    private void DrawFrame()
    {
        switch (progressBarFormat)
        {
            case ProgressBarFormat.Simple:
            case ProgressBarFormat.SimplePlusPercentage:
                padding = 0;
                break;
            case ProgressBarFormat.Framed:
            case ProgressBarFormat.FramedPlusPercentage:
                padding = 1;
                Application.Driver.DrawWindowFrame(ViewToScreen(Bounds), padding, padding, padding, padding, true);
                break;
            case ProgressBarFormat.FramedProgressPadded:
                padding = 2;
                Application.Driver.DrawWindowFrame(ViewToScreen(Bounds), padding, padding, padding, padding + 1, true);
                Application.Driver.DrawWindowFrame(
                    ViewToScreen(Bounds),
                    padding - 1,
                    padding - 1,
                    padding - 1,
                    padding - 1,
                    true);
                break;
        }
    }

    private void PopulateActivityPos()
    {
        activityPos = new int[Math.Min(Frame.Width / 3, 5)];
        for (var i = 0; i < activityPos.Length; i++) activityPos[i] = i - activityPos.Length + 1;
    }

    ///<inheritdoc/>
    public override bool OnEnter(View view)
    {
        Application.Driver.SetCursorVisibility(CursorVisibility.Invisible);

        return base.OnEnter(view);
    }
}
