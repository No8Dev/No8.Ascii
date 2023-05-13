using Asciis.UI.Controls;

namespace Asciis.UI.Engine;

public class FocusManager
{
    private readonly Screen _screen;
    private Composer? _focused;

    public FocusManager(Screen screen)
    {
        _screen = screen;
    }

    private Composer? Root =>
        _screen.RootComposer;

    public Composer? Current =>
        _focused;

    public void ClearFocus()
    {
        _focused = null;
    }

    public bool SetFocus(Composer? composer)
    {
        if (_focused != null)
        {
            if (ReferenceEquals(_focused, composer))
                return true;

            _focused.Control.HasFocus = false;
            _focused = null;
        }

        if (composer == null)
            return true;

        if (composer.Control.CanFocus &&
            composer.Control.IsEnabled)
        {
            _focused = composer;
            composer.Control.HasFocus = true;

            _screen.FocusUpdated(_focused);

            return true;
        }

        return false;
    }

    public void CursorUpdated()
    {
        _screen.FocusUpdated(_focused);
    }

    private bool Next(Composer current, bool skipGroup, string? groupName)
    {
        int childIndex = 0;
        var children = current.ChildComposers;
        int childCount = children.Count;

        if (_focused != null)
        {
            // skip until we find last child
            while (childIndex < childCount)
            {
                var thisChild = children[childIndex];
                childIndex++;

                if (ReferenceEquals(thisChild, _focused))
                {
                    break;
                }
            }
            _focused = null;
        }

        while (childIndex < childCount)
        {
            var nextChild = children[childIndex];
            childIndex++;

            if (nextChild.Control.CanFocus)
            {
                var nextGroupNav = nextChild.Control as IGroupNavigation;
                var nextGroupName = nextGroupNav?.GroupName;

                // If currently focused item has a groupName (skipGroup),
                // then skip to the next item with a null and/or different groupName
                if (skipGroup)
                {
                    if (nextGroupName != groupName)
                    {
                        _focused = nextChild;

                        return true;
                    }
                }

                // If groupName is supplied, then next item with the same groupName
                else if (!string.IsNullOrEmpty(groupName))
                {
                    if (groupName == nextGroupName)
                    {
                        _focused = nextChild;

                        return true;
                    }
                }
                else
                {
                    _focused = nextChild;
                    if (!string.IsNullOrWhiteSpace(nextGroupName))
                        MoveFocusToSelected(nextGroupNav);

                    return true;
                }
            }
            if (Next(nextChild, skipGroup, groupName))
                return true;
        }

        return false;
    }

    public bool Next(string? groupName = null, bool skipGroup = true)
    {
        if (Root == null)
            return false;

        var lastFocused = _focused;
        var current = _focused ?? Root;
        var focusedGroupNav = _focused?.Control as IGroupNavigation;
        if (skipGroup && groupName == null)
        {
            groupName = focusedGroupNav?.GroupName;
            skipGroup = !string.IsNullOrEmpty(groupName);
        }

        if (lastFocused != null)
            lastFocused.Control.HasFocus = false;

        if (_focused != null)   // Go to the parent
            current = _focused.Parent;

        while (current != null)
        {
            if (Next(current, skipGroup, groupName))
            {
                if (_focused != null)
                {
                    _focused.Control.HasFocus = true;
                    _screen.FocusUpdated(_focused);
                }

                return true;
            }

            _focused = current; // Go to the parent
            current = _focused.Parent;
        }

        _focused = null;
        if (lastFocused == null)
            return false;

        return Next(groupName, skipGroup);  // Start again from the beginning
    }

    public bool NextInGroup()
    {
        if (_focused?.Control is IGroupNavigation groupNav)
            return Next(groupNav.GroupName, false);

        return Next(null, false);
    }

    private bool Prev(Composer current, bool skipGroup, string? groupName)
    {
        var children = current.ChildComposers;
        int childIndex = children.Count - 1;

        if (_focused != null)
        {
            // skip until we find first child
            while (childIndex >= 0)
            {
                var thisChild = children[childIndex];
                childIndex--;

                if (ReferenceEquals(thisChild, _focused))
                {
                    break;
                }
            }
            _focused = null;
        }

        while (childIndex >= 0)
        {
            var prevChild = children[childIndex];
            childIndex--;

            if (prevChild.Control.CanFocus)
            {
                var nextGroupNav = prevChild.Control as IGroupNavigation;
                var nextGroupName = nextGroupNav?.GroupName;

                // If currently focused item has a groupName (isGroup && skipGroup),
                // then skip to the next item with a different groupName
                if (skipGroup)
                {
                    if (nextGroupName != groupName)
                    {
                        _focused = prevChild;

                        return true;
                    }
                }

                // If groupName is supplied, then next item with the same groupName
                else if (!string.IsNullOrEmpty(groupName))
                {
                    if (groupName == nextGroupName)
                    {
                        _focused = prevChild;

                        return true;
                    }
                }
                else
                {
                    _focused = prevChild;
                    if (!string.IsNullOrWhiteSpace(nextGroupName))
                        MoveFocusToSelected(nextGroupNav);

                    return true;
                }
            }
            if (Prev(prevChild, skipGroup, groupName))
                return true;
        }

        return false;
    }

    public bool Prev(string? groupName = null, bool skipGroup = true)
    {
        if (Root == null)
            return false;
        var lastFocused = _focused;
        var current = _focused ?? Root;
        var focusedGroupNav = _focused?.Control as IGroupNavigation;
        if (skipGroup && groupName == null)
        {
            groupName = focusedGroupNav?.GroupName;
            skipGroup = !string.IsNullOrEmpty(groupName);
        }

        if (lastFocused != null)
            lastFocused.Control.HasFocus = false;

        if (_focused != null) // Go to the parent
            current = _focused.Parent;

        while (current != null)
        {
            if (Prev(current, skipGroup, groupName))
            {
                if (_focused != null)
                {
                    _focused.Control.HasFocus = true;
                    _screen.FocusUpdated(_focused);
                }

                return true;
            }

            _focused = current; // Go to the parent
            current = _focused.Parent;
        }

        _focused = null;
        if (lastFocused == null)
            return false;

        return Prev(groupName, skipGroup); // Start again from the end
    }

    public bool PrevInGroup()
    {
        if (_focused?.Control is IGroupNavigation groupNav)
            return Prev(groupNav.GroupName, false);

        return Prev(null, false);
    }

    /// <summary>
    /// Have moved to a group.  If one item in the group is checked, then select that one
    /// </summary>
    private void MoveFocusToSelected(IGroupNavigation? groupNav)
    {
        if (groupNav == null) return;

        if (groupNav.Checked == true)
            return;

        // It is possible that none of the options have been selected yet
        var list = new List<(Composer, IGroupNavigation)>();

        FindAll(list, Root, groupNav);
        var selectedPair = list
            .FirstOrDefault((pair) => pair.Item2.Checked == true);
        if (selectedPair != default)
        {
            _focused = selectedPair.Item1;
        }
    }

    /// <summary>
    /// Recursively find all items that match a Group Nav
    /// </summary>
    /// <remarks>Not optimised for most likely scenario where all optionbox items are under a single parent.</remarks>
    private void FindAll(List<(Composer, IGroupNavigation)> list, Composer? current, IGroupNavigation groupNav)
    {
        if (current == null || current.ChildComposers == null || current.ChildComposers.Count == 0)
            return;

        foreach (var child in current.ChildComposers)
        {
            var childGroupNav = child.Control as IGroupNavigation;
            var childGroupName = childGroupNav?.GroupName;

            if (childGroupNav != null && child.Control.CanFocus)
            {
                if (!string.IsNullOrWhiteSpace(childGroupName) &&
                    string.Equals(childGroupName, groupNav.GroupName))
                    list.Add((child, childGroupNav));
            }

            FindAll(list, child, groupNav);
        }
    }

    public void ClearMouseGrab()
    {

    }
}
