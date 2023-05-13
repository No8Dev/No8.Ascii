using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

namespace NodeLayout;

public class Node : INode, IEnumerable<Node>
{
    public Node(Node other)
    {
        _plan = new LayoutPlan();
        _plan.CopyFrom(other.Plan);

        Container = other.Container;
        Context = other.Context;

        foreach (var element in other._elements)
        {
            Add(new Node(element));
        }

        _isDirty = Plan.IsDirty;
        SetupPlanChanged();
    }

    public Node(LayoutPlan plan = null)
    {
        _plan = new LayoutPlan();
        _plan.CopyFrom(plan);

        _isDirty = Plan.IsDirty;
        SetupPlanChanged();
    }
    public Node([CanBeNull] string name, LayoutPlan plan = null)
    {
        _plan = new LayoutPlan();
        _plan.CopyFrom(plan);

        Name = name;

        _isDirty = Plan.IsDirty;
        SetupPlanChanged();
    }

    public Node(out Node node,
                 LayoutPlan plan = null,
                 string name = null)
        : this(name, plan)
    {
        node = this;
    }

    public event EventHandler Dirtied;

    private void SetupPlanChanged() { _plan.Changed += PlanOnChanged; }
    private void PlanOnChanged(object sender, LayoutPlan e) { MarkDirty(); }

    protected readonly List<Node> _elements = new List<Node>();
    private Layout _layout;
    private bool _isDirty;

    public IEnumerator<Node> GetEnumerator() =>
        _elements.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        GetEnumerator();

    [CanBeNull]
    public string Name { get; set; }

    public INode Container { get; set; }

    public IReadOnlyList<INode> Elements =>
        _elements.ToList<INode>().AsReadOnly();

    private LayoutPlan _plan;

    public LayoutPlan Plan
    {
        get => _plan ??= new LayoutPlan().OnChanged(PlanOnChanged);
        set
        {
            if (_plan != null)
            {
                _plan.CopyFrom(value);
            }
            else
            {
                _plan = new LayoutPlan(value);
                _plan.Changed += PlanOnChanged;
            }
        }
    }

    public Layout Layout
    {
        get => _layout ??= new Layout();
        set
        {
            if (_layout != value)
            {
                _layout = value;
                MarkDirty();
            }
        }
    }

    public object Context { get; set; }

    public Node Add(Node node)
    {
        _elements.Add(node);
        node.Container = this;
        node.Layout = null;

        return this;
    }

    public Node Add(IEnumerable<Node> nodes)
    {
        foreach (var node in nodes)
        {
            Add(node);
        }

        return this;
    }

    public Node Remove(Node node)
    {
        if (_elements.Contains(node))
        {
            _elements.Remove(node);
            node.ClearElements();
            node.Container = null;
            node.Layout = null;

            MarkDirty();
        }

        return this;
    }

    public Node ClearElements()
    {
        foreach (var element in _elements)
        {
            element.ClearElements();
            element.Container = null;
            element.Layout = null;
        }
        _elements.Clear();
        MarkDirty();

        return this;
    }

    public Node SetElements(IEnumerable<Node> elements)
    {
        var collection = elements.ToList();
        if (collection.Count == 0)
            ClearElements();
        else
        {
            if (_elements.Count > 0)
            {
                foreach (var oldElement in _elements)
                {
                    // Our new elements may have nodes in common with the old elements. We don't reset these common nodes.
                    if (!collection.Contains(oldElement))
                    {
                        oldElement.Layout = null;
                        oldElement.Container = null;
                    }
                }
            }

            _elements.Clear();
            Add(collection);
        }

        return this;
    }

    public void CopyPlan(Node node) =>
        Plan.CopyFrom(node.Plan);

    //**********************************************

    public void Traverse(Action<Node> action)
    {
        action(this);
        foreach (var element in _elements)
        {
            element.Traverse(action);
        }
    }

    //**********************************************

    public bool IsDirty
    {
        get => _isDirty;
        set
        {
            if (_isDirty != value)
            {
                _isDirty = value;
                if (value)
                    RaiseDirtied();
                else
                    Plan.IsDirty = false;
            }
        }
    }

    public Node MarkDirty()
    {
        if (!IsDirty)
        {
            IsDirty = true;
            ((Node)Container)?.MarkDirty();
        }

        return this;
    }

    /// <summary>
    /// Propagate clearing of the flag to all child elements
    /// </summary>
    public void ClearDirtyFlag()
    {
        _isDirty = false;
        _plan.IsDirty = false;
        foreach (var node in Elements)
        {
            ((Node)node).ClearDirtyFlag();
        }
    }

    //**********************************************

    public MeasureFunc MeasureNode { get; set; }

    public override string ToString() =>
        new NodePrint(this).ToString();

    protected virtual void RaiseDirtied() { Dirtied?.Invoke(this, EventArgs.Empty); }
}
