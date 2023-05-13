using Asciis.UI.Controls;

namespace Asciis.UI.Engine;

public class Screen
{
    private FocusManager? _focusManager;

    public FocusManager FocusManager =>
        _focusManager ??= new FocusManager(this);

    /// If a keypress has a binding defined for it and is pressed, this will be
    /// called with the bound input when this screen is active.
    ///
    /// If this returns `false` (the default), then the lower-level [keyDown]
    /// method will be called.
    public virtual bool HandleInput(string input) => false;

    public virtual bool KeyDown(int keyCode, bool shift = false, bool alt = false) => false;

    public virtual bool KeyUp(int keyCode, bool shift = false, bool alt = false) => false;

    /// Called when the screen above this one ([popped]) has been popped and this
    /// screen is now the top-most screen. If a value was passed to [pop()], it
    /// will be passed to this as [result].
    public virtual void Activate(Screen popped, object result) { }

    private Control? _root;

    /// <summary>
    /// The root layout control
    /// </summary>
    public Control? Root
    {
        get => _root;
        set
        {
            _root = value;
            if (_root != null)
                _root.Host = this;
        }
    }

    private Composer? _rootComposer;

    public Composer? RootComposer
    {
        get => _rootComposer;
        set
        {
            _rootComposer = value;
            FocusManager.ClearFocus();
        }
    }

    public Layout? RootLayout =>
        RootComposer?.Layout;

    public IEnumerable<Composer> ComposersAt(Vec? pos)
    {
        var list = new List<Composer>();

        if (pos != null && pos != Vec.Unknown)
            ComposersAt(RootComposer, pos, list);

        return list;
    }

    private void ComposersAt(Composer? composer, Vec pos, List<Composer> list)
    {
        if (composer == null)
            return;

        if (composer.Layout?.Contains(pos) ?? false)
        {
            foreach (var child in composer.ChildComposers)
                ComposersAt(child, pos, list);

            list.Add(composer);
        }
    }


    public virtual void Paint(Canvas canvas) { }

    /// <summary>
    /// Called when a screen is first added *and* when the layout has been invalidated
    /// </summary>
    public virtual void UpdateLayout() { }

    public bool NeedsLayout { get; set; } = true;
    public bool NeedsPainting { get; set; } = true;

    public bool IsModal { get; } = false;

    public string? Title { get; set; }

    public IAsciiEngine? Engine { get; set; }

    /// <summary>
    /// Called when the [UserInterface] has been bound to a new terminal with a
    /// different size while this [Screen] is present.
    /// </summary>
    public virtual void Resize(Vec size) { }

    public virtual bool HandleScrollWheelEvent(ScrollWheelEventArgs scrollWheelEvent) =>
        scrollWheelEvent.Handled;

    public virtual bool HandlePointerEvent(PointerEventArgs pointerEvent) =>
        pointerEvent.Handled;

    public virtual bool HandleKeyEvent(AKeyEventArgs keyArgs) =>
        keyArgs.Handled;

    public virtual bool Update(float elapsedTimeTotalSeconds) =>
        true;

    public void Clean()
    {
        Root?.Clean();
        NeedsLayout = false;
        NeedsPainting = false;
    }

    public void Close()
    {
        Engine?.CloseScreen();
    }

    public void FocusUpdated(Composer? focused)
    {
        Engine?.FocusUpdated(focused);
        FocusManager.ClearMouseGrab();
    }

    public void AllComposersForGroupName(string? groupName, Action<Composer> action)
    {
        if (string.IsNullOrEmpty(groupName))
            return;

        AllComposersForGroupName(RootComposer, groupName, action);
    }

    private void AllComposersForGroupName(Composer? composer, string? groupName, Action<Composer> action)
    {
        if (composer == null)
            return;

        if (composer.Control is IGroupNavigation groupNav && groupNav.GroupName == groupName)
            action(composer);

        foreach (var childComposer in composer.ChildComposers)
            AllComposersForGroupName(childComposer, groupName, action);
    }
}
