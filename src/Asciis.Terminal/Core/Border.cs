﻿namespace Asciis.Terminal.Core;



/// <summary>
/// Draws a border, background, or both around another element.
/// </summary>
public class Border
{
    private int marginFrame => DrawMarginFrame ? 1 : 0;

    public Border() { }

    /// <summary>
    /// A sealed <see cref="Window"/> derived class to implement <see cref="Border"/> feature.
    /// This is only a wrapper to get borders on a toplevel and is recommended using another
    /// derived, like <see cref="Window"/> where is possible to have borders with or without
    /// border line or spacing around.
    /// </summary>
    public sealed class ToplevelContainer : Window
    {
        /// <inheritdoc/>
        public override Border Border
        {
            get => base.Border;
            set
            {
                if (base.Border != null && base.Border.Child != null && value.Child == null)
                    value.Child = base.Border.Child;
                base.Border = value;
                if (value == null) return;
                Rectangle frame;
                if (Border.Child != null && (Border.Child.Width is Dim || Border.Child.Height is Dim))
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
            if (Border.Child != null && (Border.Child.Width is Dim || Border.Child.Height is Dim))
                frame = Rectangle.Empty;
            else
                frame = Frame;
            AdjustContentView(frame);
        }

        /// <summary>
        /// Initializes with default null values.
        /// </summary>
        public ToplevelContainer()
            : this(null, null) { }

        /// <summary>
        /// Initializes a <see cref="ToplevelContainer"/> with a <see cref="LayoutStyle.Computed"/>
        /// </summary>
        /// <param name="border">The border.</param>
        /// <param name="title">The title.</param>
        public ToplevelContainer(Border border, string title = null) { Initialize(Rectangle.Empty, border, title); }

        /// <summary>
        /// Initializes a <see cref="ToplevelContainer"/> with a <see cref="LayoutStyle.Absolute"/>
        /// </summary>
        /// <param name="frame">The frame.</param>
        /// <param name="border">The border.</param>
        /// <param name="title">The title.</param>
        public ToplevelContainer(Rectangle frame, Border border, string title = null)
            : base(frame)
        {
            Initialize(frame, border, title);
        }

        private void Initialize(Rectangle frame, Border border, string title = null)
        {
            ColorScheme = Colors.TopLevel;
            Text = title ?? "";
            if (border == null)
            {
                Border = new Border()
                {
                    Application = Application,
                    BorderStyle = BorderStyle.Single,
                    BorderBrush = ColorScheme.Normal.Background
                };
            }
            else
                Border = border;
        }

        private void AdjustContentView(Rectangle frame)
        {
            var borderLength = Border.DrawMarginFrame ? 1 : 0;
            var sumPadding = Border.GetSumThickness();
            var wb = new Size();
            if (frame == Rectangle.Empty)
            {
                wb.Width = borderLength + sumPadding.Right;
                wb.Height = borderLength + sumPadding.Bottom;
                if (Border.Child == null)
                {
                    Border.Child = new ChildContentView(this)
                    {
                        X = borderLength + sumPadding.Left,
                        Y = borderLength + sumPadding.Top,
                        Width = Dim.Fill(wb.Width),
                        Height = Dim.Fill(wb.Height)
                    };
                }
                else
                {
                    Border.Child.X = borderLength + sumPadding.Left;
                    Border.Child.Y = borderLength + sumPadding.Top;
                    Border.Child.Width = Dim.Fill(wb.Width);
                    Border.Child.Height = Dim.Fill(wb.Height);
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
                if (Border.Child == null)
                    Border.Child = new ChildContentView(cFrame, this);
                else
                    Border.Child.Frame = cFrame;
            }

            base.Add(Border.Child);
            Border.ChildContainer = this;
        }

        /// <inheritdoc/>
        public override void Add(View view)
        {
            view.Application = Application;
            Border.Child.Add(view);
            if (view.CanFocus) CanFocus = true;
            AddMenuStatusBar(view);
        }

        /// <inheritdoc/>
        public override void Remove(View view)
        {
            if (view == null) return;

            SetNeedsDisplay();
            var touched = view.Frame;
            Border.Child.Remove(view);

            if (Border.Child.InternalSubviews.Count < 1) CanFocus = false;
            RemoveMenuStatusBar(view);
        }

        /// <inheritdoc/>
        public override void RemoveAll() { Border.Child.RemoveAll(); }

        /// <inheritdoc/>
        public override void Redraw(Rectangle bounds)
        {
            if (!NeedDisplay.IsEmpty)
            {
                Driver.SetAttribute(GetNormalColor());
                Border.DrawContent();
            }

            var savedClip = Border.Child.ClipToBounds();
            Border.Child.Redraw(Border.Child.Bounds);
            Driver.Clip = savedClip;

            ClearLayoutNeeded();
            ClearNeedsDisplay();

            if (Border.BorderStyle != BorderStyle.None)
            {
                Driver.SetAttribute(GetNormalColor());
                Border.DrawTitle(this, Frame);
            }

            // Checks if there are any SuperView view which intersect with this window.
            if (SuperView != null)
            {
                SuperView.SetNeedsLayout();
                SuperView.SetNeedsDisplay();
            }
        }
    }

    private class ChildContentView : View
    {
        private View instance;

        public ChildContentView(Rectangle frame, View instance)
            : base(frame)
        {
            this.instance = instance;
        }

        public ChildContentView(View instance) { this.instance = instance; }

        public override bool MouseEvent(MouseEvent mouseEvent) { return instance.MouseEvent(mouseEvent); }
    }

    /// <summary>
    /// Event to be invoked when any border property change.
    /// </summary>
    public event Action<Border> BorderChanged;

    private BorderStyle borderStyle;
    private bool drawMarginFrame;
    private Thickness borderThickness;
    private Thickness padding;

    /// <summary>
    /// Specifies the <see cref="Core.BorderStyle"/> for a view.
    /// </summary>
    public BorderStyle BorderStyle
    {
        get => borderStyle;
        set
        {
            if (value != BorderStyle.None && !drawMarginFrame) // Ensures drawn the border lines.
                drawMarginFrame = true;
            borderStyle = value;
            OnBorderChanged();
        }
    }

    /// <summary>
    /// Gets or sets if a margin frame is drawn around the <see cref="Child"/> regardless the <see cref="BorderStyle"/>
    /// </summary>
    public bool DrawMarginFrame
    {
        get => drawMarginFrame;
        set
        {
            if (borderStyle != BorderStyle.None
                && (!value || !drawMarginFrame)) // Ensures drawn the border lines.
                drawMarginFrame = true;
            else
                drawMarginFrame = value;
            OnBorderChanged();
        }
    }

    /// <summary>
    /// Gets or sets the relative <see cref="Thickness"/> of a <see cref="Border"/>.
    /// </summary>
    public Thickness BorderThickness
    {
        get => borderThickness;
        set
        {
            borderThickness = value;
            OnBorderChanged();
        }
    }

    /// <summary>
    /// Gets or sets the <see cref="ConsoleColor"/> that draws the outer border color.
    /// </summary>
    public ConsoleColor BorderBrush { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="ConsoleColor"/> that fills the area between the bounds of a <see cref="Border"/>.
    /// </summary>
    public ConsoleColor Background { get; set; }

    /// <summary>
    /// Gets or sets a <see cref="Thickness"/> value that describes the amount of space between a
    ///  <see cref="Border"/> and its child element.
    /// </summary>
    public Thickness Padding
    {
        get => padding;
        set
        {
            padding = value;
            OnBorderChanged();
        }
    }

    /// <summary>
    /// Gets the rendered width of this element.
    /// </summary>
    public int ActualWidth
    {
        get
        {
            var driver = Application.Driver;
            if (Parent?.Border == null)
                return Math.Min(
                    Child?.Frame.Width + 2 * marginFrame + Padding.Right
                    + BorderThickness.Right + Padding.Left + BorderThickness.Left ?? 0,
                    driver.Cols);
            return Math.Min(Parent.Frame.Width, driver.Cols);
        }
    }

    /// <summary>
    /// Gets the rendered height of this element.
    /// </summary>
    public int ActualHeight
    {
        get
        {
            var driver = Application.Driver;
            if (Parent?.Border == null)
                return Math.Min(
                    Child?.Frame.Height + 2 * marginFrame + Padding.Bottom
                    + BorderThickness.Bottom + Padding.Top + BorderThickness.Top ?? 0,
                    driver.Rows);
            return Math.Min(Parent.Frame.Height, driver.Rows);
        }
    }

    /// <summary>
    /// Gets or sets the single child element of a <see cref="View"/>.
    /// </summary>
    public View Child { get; set; }

    /// <summary>
    /// Gets the parent <see cref="Child"/> parent if any.
    /// </summary>
    public View Parent => Child?.SuperView;

    /// <summary>
    /// Gets or private sets by the <see cref="ToplevelContainer"/>
    /// </summary>
    public ToplevelContainer ChildContainer { get; private set; }

    /// <summary>
    /// Gets or sets the 3D effect around the <see cref="Border"/>.
    /// </summary>
    public bool Effect3D { get; set; }

    /// <summary>
    /// Get or sets the offset start position for the <see cref="Effect3D"/>
    /// </summary>
    public Point Effect3DOffset { get; set; } = new(1, 1);

    /// <summary>
    /// Gets or sets the color for the <see cref="Border"/>
    /// </summary>
    public CellAttributes? Effect3DBrush { get; set; }
    public AsciiApplication Application { get; set; }

    /// <summary>
    /// Calculate the sum of the <see cref="Padding"/> and the <see cref="BorderThickness"/>
    /// </summary>
    /// <returns>The total of the <see cref="Border"/> <see cref="Thickness"/></returns>
    public Thickness GetSumThickness()
    {
        return new Thickness()
        {
            Left = Padding.Left + BorderThickness.Left,
            Top = Padding.Top + BorderThickness.Top,
            Right = Padding.Right + BorderThickness.Right,
            Bottom = Padding.Bottom + BorderThickness.Bottom
        };
    }

    /// <summary>
    /// Drawn the <see cref="BorderThickness"/> more the <see cref="Padding"/>
    ///  more the <see cref="Border.BorderStyle"/> and the <see cref="Effect3D"/>.
    /// </summary>
    public void DrawContent()
    {
        if (Parent?.Border != null)
            DrawParentBorder(Parent.ViewToScreen(Parent.Bounds));
        else
            DrawChildBorder(Child.ViewToScreen(Child.Bounds));
    }

    /// <summary>
    /// Same as <see cref="DrawContent"/> but drawing full frames for all borders.
    /// </summary>
    public void DrawFullContent()
    {
        var borderThickness = BorderThickness;
        var padding = Padding;
        var marginFrame = DrawMarginFrame ? 1 : 0;
        var driver = Application.Driver;
        Rectangle scrRect;
        if (Parent?.Border != null)
            scrRect = Parent.ViewToScreen(Parent.Bounds);
        else
            scrRect = Child.ViewToScreen(Child.Bounds);
        Rectangle borderRect;
        if (Parent?.Border != null)
            borderRect = scrRect;
        else
            borderRect = new Rectangle()
            {
                X = scrRect.X - marginFrame - padding.Left - borderThickness.Left,
                Y = scrRect.Y - marginFrame - padding.Top - borderThickness.Top,
                Width = ActualWidth,
                Height = ActualHeight
            };
        var savedAttribute = driver.GetAttribute();

        // Draw 3D effects
        if (Effect3D)
        {
            driver.SetAttribute(GetEffect3DBrush());

            var effectBorder = new Rectangle()
            {
                X = borderRect.X + Effect3DOffset.X,
                Y = borderRect.Y + Effect3DOffset.Y,
                Width = ActualWidth,
                Height = ActualHeight
            };
            //Child.Clear (effectBorder);
            for (var r = effectBorder.Y; r < Math.Min(effectBorder.Bottom, driver.Rows); r++)
            {
                for (var c = effectBorder.X; c < Math.Min(effectBorder.Right, driver.Cols); c++)
                    AddRuneAt(driver, c, r, driver.Contents[r, c].Rune);
            }
        }

        // Draw border thickness
        driver.SetAttribute(new CellAttributes(BorderBrush, BorderBrush));
        Child.Clear(borderRect);

        borderRect = new Rectangle()
        {
            X = borderRect.X + borderThickness.Left,
            Y = borderRect.Y + borderThickness.Top,
            Width = Math.Max(
                             borderRect.Width - borderThickness.Right - borderThickness.Left,
                             0),
            Height = Math.Max(
                             borderRect.Height - borderThickness.Bottom - borderThickness.Top,
                             0)
        };
        if (borderRect != scrRect)
        {
            // Draw padding
            driver.SetAttribute(new CellAttributes(Background, Background));
            Child.Clear(borderRect);
        }

        driver.SetAttribute(savedAttribute);

        // Draw margin frame
        if (Parent?.Border != null)
        {
            var sumPadding = GetSumThickness();
            borderRect = new Rectangle()
            {
                X = scrRect.X + sumPadding.Left,
                Y = scrRect.Y + sumPadding.Top,
                Width = Math.Max(scrRect.Width - sumPadding.Right - sumPadding.Left, 0),
                Height = Math.Max(scrRect.Height - sumPadding.Bottom - sumPadding.Top, 0)
            };
        }
        else
        {
            borderRect = new Rectangle()
            {
                X = borderRect.X + padding.Left,
                Y = borderRect.Y + padding.Top,
                Width = Math.Max(borderRect.Width - padding.Right - padding.Left, 0),
                Height = Math.Max(borderRect.Height - padding.Bottom - padding.Top, 0)
            };
        }

        if (borderRect.Width > 0 && borderRect.Height > 0)
            driver.DrawWindowFrame(borderRect, 1, 1, 1, 1, BorderStyle != BorderStyle.None, true, this);
    }

    private void DrawChildBorder(Rectangle frame)
    {
        var drawMarginFrame = DrawMarginFrame ? 1 : 0;
        var sumThickness = GetSumThickness();
        var padding = Padding;
        var effect3DOffset = Effect3DOffset;
        var driver = Application.Driver;

        var savedAttribute = driver.GetAttribute();

        driver.SetAttribute(new CellAttributes(BorderBrush,BorderBrush));

        // Draw the upper BorderThickness
        for (var r = frame.Y - drawMarginFrame - sumThickness.Top;
            r < frame.Y - drawMarginFrame - padding.Top;
            r++)
        {
            for (var c = frame.X - drawMarginFrame - sumThickness.Left;
                c < Math.Min(frame.Right + drawMarginFrame + sumThickness.Right, driver.Cols);
                c++)
                AddRuneAt(driver, c, r, ' ');
        }

        // Draw the left BorderThickness
        for (var r = frame.Y - drawMarginFrame - padding.Top;
            r < Math.Min(frame.Bottom + drawMarginFrame + padding.Bottom, driver.Rows);
            r++)
        {
            for (var c = frame.X - drawMarginFrame - sumThickness.Left;
                c < frame.X - drawMarginFrame - padding.Left;
                c++)
                AddRuneAt(driver, c, r, ' ');
        }

        // Draw the right BorderThickness
        for (var r = frame.Y - drawMarginFrame - padding.Top;
            r < Math.Min(frame.Bottom + drawMarginFrame + padding.Bottom, driver.Rows);
            r++)
        {
            for (var c = frame.Right + drawMarginFrame + padding.Right;
                c < Math.Min(frame.Right + drawMarginFrame + sumThickness.Right, driver.Cols);
                c++)
                AddRuneAt(driver, c, r, ' ');
        }

        // Draw the lower BorderThickness
        for (var r = frame.Bottom + drawMarginFrame + padding.Bottom;
            r < Math.Min(frame.Bottom + drawMarginFrame + sumThickness.Bottom, driver.Rows);
            r++)
        {
            for (var c = frame.X - drawMarginFrame - sumThickness.Left;
                c < Math.Min(frame.Right + drawMarginFrame + sumThickness.Right, driver.Cols);
                c++)
                AddRuneAt(driver, c, r, ' ');
        }

        driver.SetAttribute(new CellAttributes(Background, Background));

        // Draw the upper Padding
        for (var r = frame.Y - drawMarginFrame - padding.Top;
            r < frame.Y - drawMarginFrame;
            r++)
        {
            for (var c = frame.X - drawMarginFrame - padding.Left;
                c < Math.Min(frame.Right + drawMarginFrame + padding.Right, driver.Cols);
                c++)
                AddRuneAt(driver, c, r, ' ');
        }

        // Draw the left Padding
        for (var r = frame.Y - drawMarginFrame;
            r < Math.Min(frame.Bottom + drawMarginFrame, driver.Rows);
            r++)
        {
            for (var c = frame.X - drawMarginFrame - padding.Left;
                c < frame.X - drawMarginFrame;
                c++)
                AddRuneAt(driver, c, r, ' ');
        }

        // Draw the right Padding
        for (var r = frame.Y - drawMarginFrame;
            r < Math.Min(frame.Bottom + drawMarginFrame, driver.Rows);
            r++)
        {
            for (var c = frame.Right + drawMarginFrame;
                c < Math.Min(frame.Right + drawMarginFrame + padding.Right, driver.Cols);
                c++)
                AddRuneAt(driver, c, r, ' ');
        }

        // Draw the lower Padding
        for (var r = frame.Bottom + drawMarginFrame;
            r < Math.Min(frame.Bottom + drawMarginFrame + padding.Bottom, driver.Rows);
            r++)
        {
            for (var c = frame.X - drawMarginFrame - padding.Left;
                c < Math.Min(frame.Right + drawMarginFrame + padding.Right, driver.Cols);
                c++)
                AddRuneAt(driver, c, r, ' ');
        }

        driver.SetAttribute(savedAttribute);

        // Draw the MarginFrame
        var rect = new Rectangle()
        {
            X = frame.X - drawMarginFrame,
            Y = frame.Y - drawMarginFrame,
            Width = frame.Width + 2 * drawMarginFrame,
            Height = frame.Height + 2 * drawMarginFrame
        };
        if (rect.Width > 0 && rect.Height > 0)
            driver.DrawWindowFrame(rect, 1, 1, 1, 1, BorderStyle != BorderStyle.None, true, this);

        if (Effect3D)
        {
            driver.SetAttribute(GetEffect3DBrush());

            // Draw the upper Effect3D
            for (var r = frame.Y - drawMarginFrame - sumThickness.Top + effect3DOffset.Y;
                r < frame.Y - drawMarginFrame - sumThickness.Top;
                r++)
            {
                for (var c = frame.X - drawMarginFrame - sumThickness.Left + effect3DOffset.X;
                    c < Math.Min(
                        frame.Right + drawMarginFrame + sumThickness.Right + effect3DOffset.X,
                        driver.Cols);
                    c++)
                    if (c >= 0 && c < driver.Cols && r >= 0 && r < driver.Rows)
                        AddRuneAt(driver, c, r, driver.Contents![r, c].Rune);
            }

            // Draw the left Effect3D
            for (var r = frame.Y - drawMarginFrame - sumThickness.Top + effect3DOffset.Y;
                r < Math.Min(frame.Bottom + drawMarginFrame + sumThickness.Bottom + effect3DOffset.Y, driver.Rows);
                r++)
            {
                for (var c = frame.X - drawMarginFrame - sumThickness.Left + effect3DOffset.X;
                    c < frame.X - drawMarginFrame - sumThickness.Left;
                    c++)
                    AddRuneAt(driver, c, r, driver.Contents![r, c].Rune);
            }

            // Draw the right Effect3D
            for (var r = frame.Y - drawMarginFrame - sumThickness.Top + effect3DOffset.Y;
                r < Math.Min(frame.Bottom + drawMarginFrame + sumThickness.Bottom + effect3DOffset.Y, driver.Rows);
                r++)
            {
                for (var c = frame.Right + drawMarginFrame + sumThickness.Right;
                    c < Math.Min(
                        frame.Right + drawMarginFrame + sumThickness.Right + effect3DOffset.X,
                        driver.Cols);
                    c++)
                    AddRuneAt(driver, c, r, driver.Contents![r, c].Rune);
            }

            // Draw the lower Effect3D
            for (var r = frame.Bottom + drawMarginFrame + sumThickness.Bottom;
                r < Math.Min(frame.Bottom + drawMarginFrame + sumThickness.Bottom + effect3DOffset.Y, driver.Rows);
                r++)
            {
                for (var c = frame.X - drawMarginFrame - sumThickness.Left + effect3DOffset.X;
                    c < Math.Min(
                        frame.Right + drawMarginFrame + sumThickness.Right + effect3DOffset.X,
                        driver.Cols);
                    c++)
                    AddRuneAt(driver, c, r, driver.Contents![r, c].Rune);
            }
        }
    }

    private void DrawParentBorder(Rectangle frame)
    {
        var sumThickness = GetSumThickness();
        var borderThickness = BorderThickness;
        var effect3DOffset = Effect3DOffset;
        var driver = Application.Driver;

        var savedAttribute = driver.GetAttribute();

        driver.SetAttribute(new CellAttributes(BorderBrush, BorderBrush));

        // Draw the upper BorderThickness
        for (var r = frame.Y;
            r < Math.Min(frame.Y + borderThickness.Top, frame.Bottom);
            r++)
        {
            for (var c = frame.X;
                c < Math.Min(frame.Right, driver.Cols);
                c++)
                AddRuneAt(driver, c, r, ' ');
        }

        // Draw the left BorderThickness
        for (var r = Math.Min(frame.Y + borderThickness.Top, frame.Bottom);
            r < Math.Min(frame.Bottom - borderThickness.Bottom, driver.Rows);
            r++)
        {
            for (var c = frame.X;
                c < Math.Min(frame.X + borderThickness.Left, frame.Right);
                c++)
                AddRuneAt(driver, c, r, ' ');
        }

        // Draw the right BorderThickness
        for (var r = Math.Min(frame.Y + borderThickness.Top, frame.Bottom);
            r < Math.Min(frame.Bottom - borderThickness.Bottom, driver.Rows);
            r++)
        {
            for (var c = Math.Max(frame.Right - borderThickness.Right, frame.X);
                c < Math.Min(frame.Right, driver.Cols);
                c++)
                AddRuneAt(driver, c, r, ' ');
        }

        // Draw the lower BorderThickness
        for (var r = Math.Max(frame.Bottom - borderThickness.Bottom, frame.Y);
            r < Math.Min(frame.Bottom, driver.Rows);
            r++)
        {
            for (var c = frame.X;
                c < Math.Min(frame.Right, driver.Cols);
                c++)
                AddRuneAt(driver, c, r, ' ');
        }

        driver.SetAttribute(new CellAttributes(Background, Background));

        // Draw the upper Padding
        for (var r = frame.Y + borderThickness.Top;
            r < Math.Min(frame.Y + sumThickness.Top, frame.Bottom - borderThickness.Bottom);
            r++)
        {
            for (var c = frame.X + borderThickness.Left;
                c < Math.Min(frame.Right - borderThickness.Right, driver.Cols);
                c++)
                AddRuneAt(driver, c, r, ' ');
        }

        // Draw the left Padding
        for (var r = frame.Y + sumThickness.Top;
            r < Math.Min(frame.Bottom - sumThickness.Bottom, driver.Rows);
            r++)
        {
            for (var c = frame.X + borderThickness.Left;
                c < Math.Min(frame.X + sumThickness.Left, frame.Right - borderThickness.Right);
                c++)
                AddRuneAt(driver, c, r, ' ');
        }

        // Draw the right Padding
        for (var r = frame.Y + sumThickness.Top;
            r < Math.Min(frame.Bottom - sumThickness.Bottom, driver.Rows);
            r++)
        {
            for (var c = Math.Max(frame.Right - sumThickness.Right, frame.X + sumThickness.Left);
                c < Math.Max(frame.Right - borderThickness.Right, frame.X + sumThickness.Left);
                c++)
                AddRuneAt(driver, c, r, ' ');
        }

        // Draw the lower Padding
        for (var r = Math.Max(frame.Bottom - sumThickness.Bottom, frame.Y + borderThickness.Top);
            r < Math.Min(frame.Bottom - borderThickness.Bottom, driver.Rows);
            r++)
        {
            for (var c = frame.X + borderThickness.Left;
                c < Math.Min(frame.Right - borderThickness.Right, driver.Cols);
                c++)
                AddRuneAt(driver, c, r, ' ');
        }

        driver.SetAttribute(savedAttribute);

        // Draw the MarginFrame
        var rect = new Rectangle()
        {
            X = frame.X + sumThickness.Left,
            Y = frame.Y + sumThickness.Top,
            Width = Math.Max(frame.Width - sumThickness.Right - sumThickness.Left, 0),
            Height = Math.Max(frame.Height - sumThickness.Bottom - sumThickness.Top, 0)
        };
        if (rect.Width > 0 && rect.Height > 0)
            driver.DrawWindowFrame(rect, 1, 1, 1, 1, BorderStyle != BorderStyle.None, true, this);

        if (Effect3D)
        {
            driver.SetAttribute(GetEffect3DBrush());

            // Draw the upper Effect3D
            for (var r = Math.Max(frame.Y + effect3DOffset.Y, 0);
                r < frame.Y;
                r++)
            {
                for (var c = Math.Max(frame.X + effect3DOffset.X, 0);
                    c < Math.Min(frame.Right + effect3DOffset.X, driver.Cols);
                    c++)
                    AddRuneAt(driver, c, r, driver.Contents![r, c].Rune);
            }

            // Draw the left Effect3D
            for (var r = Math.Max(frame.Y + effect3DOffset.Y, 0);
                r < Math.Min(frame.Bottom + effect3DOffset.Y, driver.Rows);
                r++)
            {
                for (var c = Math.Max(frame.X + effect3DOffset.X, 0);
                    c < frame.X;
                    c++)
                    AddRuneAt(driver, c, r, driver.Contents![r, c].Rune);
            }

            // Draw the right Effect3D
            for (var r = Math.Max(frame.Y + effect3DOffset.Y, 0);
                r < Math.Min(frame.Bottom + effect3DOffset.Y, driver.Rows);
                r++)
            {
                for (var c = frame.Right;
                    c < Math.Min(frame.Right + effect3DOffset.X, driver.Cols);
                    c++)
                    AddRuneAt(driver, c, r, driver.Contents![r, c].Rune);
            }

            // Draw the lower Effect3D
            for (var r = frame.Bottom;
                r < Math.Min(frame.Bottom + effect3DOffset.Y, driver.Rows);
                r++)
            {
                for (var c = Math.Max(frame.X + effect3DOffset.X, 0);
                    c < Math.Min(frame.Right + effect3DOffset.X, driver.Cols);
                    c++)
                    AddRuneAt(driver, c, r, driver.Contents![r, c].Rune);
            }
        }
    }

    private CellAttributes GetEffect3DBrush()
    {
        return Effect3DBrush == null
            ? new CellAttributes(ConsoleColor.Gray, ConsoleColor.DarkGray)
            : (CellAttributes)Effect3DBrush;
    }

    private void AddRuneAt(ConsoleDriver driver, int col, int row, Rune ch)
    {
        driver.Move(col, row);
        driver.AddRune(ch);
    }

    private void AddRuneAt(ConsoleDriver driver, int col, int row, char ch)
    {
        driver.Move(col, row);
        driver.AddRune((Rune)ch);
    }


    /// <summary>
    /// Drawn the view text from a <see cref="View"/>.
    /// </summary>
    public void DrawTitle(View view, Rectangle rect)
    {
        var driver = Application.Driver;
        if (BorderStyle != BorderStyle.None)
        {
            driver.SetAttribute(view.GetNormalColor());
            if (view.HasFocus) driver.SetAttribute(view.ColorScheme.HotNormal);
            var padding = GetSumThickness();
            driver.DrawWindowTitle(
                rect,
                view.Text,
                padding.Left,
                padding.Top,
                padding.Right,
                padding.Bottom);
        }

        driver.SetAttribute(view.GetNormalColor());
    }

    /// <summary>
    /// Invoke the <see cref="BorderChanged"/> event.
    /// </summary>
    public virtual void OnBorderChanged() { BorderChanged?.Invoke(this); }
}
