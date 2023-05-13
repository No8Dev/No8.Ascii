using Asciis.Terminal.Helpers;

namespace Asciis.Terminal.Views;

/// <summary>
/// <see cref="RadioGroup"/> shows a group of radio labels, only one of those can be selected at a given time
/// </summary>
public class RadioGroup : View
{
    private int selected = -1;
    private int cursor;
    private DisplayModeLayout displayMode;
    private int horizontalSpace = 2;
    private List<(int pos, int length)> horizontal;

    private void Init(Rectangle rect, string[] radioLabels, int selected)
    {
        if (radioLabels == null)
            this.radioLabels = new List<string>();
        else
            this.radioLabels = radioLabels.ToList();

        this.selected = selected;
        SetWidthHeight(this.radioLabels);
        CanFocus = true;
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="RadioGroup"/> class using <see cref="LayoutStyle.Computed"/> layout.
    /// </summary>
    public RadioGroup()
        : this(new string[] { }) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RadioGroup"/> class using <see cref="LayoutStyle.Computed"/> layout.
    /// </summary>
    /// <param name="radioLabels">The radio labels; an array of strings that can contain hotkeys using an underscore before the letter.</param>
    /// <param name="selected">The index of the item to be selected, the value is clamped to the number of items.</param>
    public RadioGroup(string[] radioLabels, int selected = 0)
        : base()
    {
        Init(Rectangle.Empty, radioLabels, selected);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RadioGroup"/> class using <see cref="LayoutStyle.Absolute"/> layout.
    /// </summary>
    /// <param name="rect">Boundaries for the radio group.</param>
    /// <param name="radioLabels">The radio labels; an array of strings that can contain hotkeys using an underscore before the letter.</param>
    /// <param name="selected">The index of item to be selected, the value is clamped to the number of items.</param>
    public RadioGroup(Rectangle rect, string[] radioLabels, int selected = 0)
        : base(rect)
    {
        Init(rect, radioLabels, selected);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RadioGroup"/> class using <see cref="LayoutStyle.Absolute"/> layout.
    /// The <see cref="View"/> frame is computed from the provided radio labels.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="radioLabels">The radio labels; an array of strings that can contain hotkeys using an underscore before the letter.</param>
    /// <param name="selected">The item to be selected, the value is clamped to the number of items.</param>		
    public RadioGroup(int x, int y, string[] radioLabels, int selected = 0)
        :
        this(MakeRect(x, y, radioLabels != null ? radioLabels.ToList() : null), radioLabels, selected)
    { }

    /// <summary>
    /// Gets or sets the <see cref="DisplayModeLayout"/> for this <see cref="RadioGroup"/>.
    /// </summary>
    public DisplayModeLayout DisplayMode
    {
        get => displayMode;
        set
        {
            if (displayMode != value)
            {
                displayMode = value;
                SetWidthHeight(radioLabels);
                SetNeedsDisplay();
            }
        }
    }

    /// <summary>
    /// Gets or sets the horizontal space for this <see cref="RadioGroup"/> if the <see cref="DisplayMode"/> is <see cref="DisplayModeLayout.Horizontal"/>
    /// </summary>
    public int HorizontalSpace
    {
        get => horizontalSpace;
        set
        {
            if (horizontalSpace != value && displayMode == DisplayModeLayout.Horizontal)
            {
                horizontalSpace = value;
                SetWidthHeight(radioLabels);
                SetNeedsDisplay();
            }
        }
    }

    private void SetWidthHeight(List<string> radioLabels)
    {
        switch (displayMode)
        {
            case DisplayModeLayout.Vertical:
                var r = MakeRect(0, 0, radioLabels);
                if (LayoutStyle == LayoutStyle.Computed)
                {
                    Width = r.Width;
                    Height = radioLabels.Count;
                }
                else
                {
                    base.Frame = new Rectangle(base.Frame.Location, new Size(r.Width, radioLabels.Count));
                }

                break;
            case DisplayModeLayout.Horizontal:
                CalculateHorizontalPositions();
                var length = 0;
                foreach (var item in horizontal) length += item.length;
                var hr = new Rectangle(0, 0, length, 1);
                if (LayoutStyle == LayoutStyle.Computed)
                {
                    Width = hr.Width;
                    Height = 1;
                }

                break;
        }
    }

    private static Rectangle MakeRect(int x, int y, List<string> radioLabels)
    {
        var width = 0;

        if (radioLabels == null) return new Rectangle(x, y, width, 0);

        foreach (var s in radioLabels)
            width = Math.Max(s.RuneCount() + 3, width);
        return new Rectangle(x, y, width, radioLabels.Count);
    }


    private List<string> radioLabels = new();

    /// <summary>
    /// The radio labels to display
    /// </summary>
    /// <value>The radio labels.</value>
    public string[] RadioLabels
    {
        get => radioLabels.ToArray();
        set
        {
            var prevCount = radioLabels.Count;
            radioLabels = value.ToList();
            if (prevCount != radioLabels.Count) SetWidthHeight(radioLabels);
            SelectedItem = 0;
            cursor = 0;
            SetNeedsDisplay();
        }
    }

    private void CalculateHorizontalPositions()
    {
        if (displayMode == DisplayModeLayout.Horizontal)
        {
            horizontal = new List<(int pos, int length)>();
            var start = 0;
            var length = 0;
            for (var i = 0; i < radioLabels.Count; i++)
            {
                start += length;
                length = radioLabels[i].RuneCount() + horizontalSpace;
                horizontal.Add((start, length));
            }
        }
    }

    //// Redraws the RadioGroup 
    //void Update(List<string> newRadioLabels)
    //{
    //	for (int i = 0; i < radioLabels.Count; i++) {
    //		Move(0, i);
    //		Driver.SetAttribute(ColorScheme.Normal);
    //		Driver.AddStr(string.Make(new string (' ', radioLabels[i].RuneCount + 4)));
    //	}
    //	if (newRadioLabels.Count != radioLabels.Count) {
    //		SetWidthHeight(newRadioLabels);
    //	}
    //}

    ///<inheritdoc/>
    public override void Redraw(Rectangle bounds)
    {
        Driver.SetAttribute(GetNormalColor());
        Clear();
        for (var i = 0; i < radioLabels.Count; i++)
        {
            switch (DisplayMode)
            {
                case DisplayModeLayout.Vertical:
                    Move(0, i);
                    break;
                case DisplayModeLayout.Horizontal:
                    Move(horizontal[i].pos, 0);
                    break;
            }

            Driver.SetAttribute(GetNormalColor());
            Driver.AddStr(
                new Rune[] { i == selected ? Driver.Selected : Driver.UnSelected, (Rune)' ' }.MakeString());
            DrawHotString(radioLabels[i], HasFocus && i == cursor, ColorScheme);
        }
    }

    ///<inheritdoc/>
    public override void PositionCursor()
    {
        switch (DisplayMode)
        {
            case DisplayModeLayout.Vertical:
                Move(0, cursor);
                break;
            case DisplayModeLayout.Horizontal:
                Move(horizontal[cursor].pos, 0);
                break;
        }
    }

    // TODO: Make this a global class
    /// <summary>
    /// Event arguments for the SelectedItemChagned event.
    /// </summary>
    public class SelectedItemChangedArgs : System.EventArgs
    {
        /// <summary>
        /// Gets the index of the item that was previously selected. -1 if there was no previous selection.
        /// </summary>
        public int PreviousSelectedItem { get; }

        /// <summary>
        /// Gets the index of the item that is now selected. -1 if there is no selection.
        /// </summary>
        public int SelectedItem { get; }

        /// <summary>
        /// Initializes a new <see cref="SelectedItemChangedArgs"/> class.
        /// </summary>
        /// <param name="selectedItem"></param>
        /// <param name="previousSelectedItem"></param>
        public SelectedItemChangedArgs(int selectedItem, int previousSelectedItem)
        {
            PreviousSelectedItem = previousSelectedItem;
            SelectedItem = selectedItem;
        }
    }

    /// <summary>
    /// Invoked when the selected radio label has changed.
    /// </summary>
    public event Action<SelectedItemChangedArgs> SelectedItemChanged;

    /// <summary>
    /// The currently selected item from the list of radio labels
    /// </summary>
    /// <value>The selected.</value>
    public int SelectedItem
    {
        get => selected;
        set
        {
            OnSelectedItemChanged(value, SelectedItem);
            cursor = selected;
            SetNeedsDisplay();
        }
    }

    /// <summary>
    /// Allow to invoke the <see cref="SelectedItemChanged"/> after their creation.
    /// </summary>
    public void Refresh() { OnSelectedItemChanged(selected, -1); }

    /// <summary>
    /// Called whenever the current selected item changes. Invokes the <see cref="SelectedItemChanged"/> event.
    /// </summary>
    /// <param name="selectedItem"></param>
    /// <param name="previousSelectedItem"></param>
    public virtual void OnSelectedItemChanged(int selectedItem, int previousSelectedItem)
    {
        selected = selectedItem;
        SelectedItemChanged?.Invoke(new SelectedItemChangedArgs(selectedItem, previousSelectedItem));
    }

    ///<inheritdoc/>
    public override bool ProcessColdKey(KeyEvent kb)
    {
        var key = kb.KeyValue;
        if (key < char.MaxValue && char.IsLetterOrDigit((char)key))
        {
            var i = 0;
            key = char.ToUpper((char)key);
            foreach (var l in radioLabels)
            {
                var nextIsHot = false;
                foreach (var c in l)
                    if (c == '_')
                    {
                        nextIsHot = true;
                    }
                    else
                    {
                        if (nextIsHot && c == key)
                        {
                            SelectedItem = i;
                            cursor = i;
                            if (!HasFocus)
                                SetFocus();
                            return true;
                        }

                        nextIsHot = false;
                    }

                i++;
            }
        }

        return false;
    }

    ///<inheritdoc/>
    public override bool ProcessKey(KeyEvent kb)
    {
        switch (kb.Key)
        {
            case Key.CursorUp:
                if (cursor > 0)
                {
                    cursor--;
                    SetNeedsDisplay();
                    return true;
                }

                break;
            case Key.CursorDown:
                if (cursor + 1 < radioLabels.Count)
                {
                    cursor++;
                    SetNeedsDisplay();
                    return true;
                }

                break;
            case Key.Space:
                SelectedItem = cursor;
                return true;
        }

        return base.ProcessKey(kb);
    }

    ///<inheritdoc/>
    public override bool MouseEvent(MouseEvent me)
    {
        if (!me.Flags.HasFlag(MouseFlags.Button1Clicked)) return false;
        if (!CanFocus) return false;
        SetFocus();

        var pos = displayMode == DisplayModeLayout.Horizontal ? me.X : me.Y;
        var rCount = displayMode == DisplayModeLayout.Horizontal
            ? horizontal.Last().pos + horizontal.Last().length
            : radioLabels.Count;

        if (pos < rCount)
        {
            var c = displayMode == DisplayModeLayout.Horizontal
                ? horizontal.FindIndex((x) => x.pos <= me.X && x.pos + x.length - 2 >= me.X)
                : me.Y;
            if (c > -1)
            {
                cursor = SelectedItem = c;
                SetNeedsDisplay();
            }
        }

        return true;
    }

    ///<inheritdoc/>
    public override bool OnEnter(View view)
    {
        Application.Driver.SetCursorVisibility(CursorVisibility.Invisible);

        return base.OnEnter(view);
    }
}

/// <summary>
/// Used for choose the display mode of this <see cref="RadioGroup"/>
/// </summary>
public enum DisplayModeLayout
{
    /// <summary>
    /// Vertical mode display. It's the default.
    /// </summary>
    Vertical,

    /// <summary>
    /// Horizontal mode display.
    /// </summary>
    Horizontal
}
