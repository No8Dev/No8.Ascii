using System;
using System.Collections.Generic;

namespace Asciis.App.Tests.DependencyInjectionTests.TestData;

// Nested dependencies with func
public interface IStateManager
{
    bool MoveToState(string state, object data);
    void Init();
}

public interface IView
{
    object GetView();
}

public interface IViewManager
{
    bool LoadView(IView view);
}

public class MainView : IView, IViewManager
{

    public IView? LoadedView { get; private set; }

    public object GetView()
    {
        return this;
    }

 

    public bool LoadView(IView view)
    {
        LoadedView = view ?? throw new ArgumentNullException(nameof(view));
        return true;
    }

}

public class ViewCollection
{
    public ViewCollection(IEnumerable<IView> views)
    {
        Views = views;
    }

    public IEnumerable<IView> Views { get; }
}

public class SplashView : IView
{
    public object GetView()
    {
        return this;
    }
}

public class StateManager : IStateManager
{
    readonly IViewManager        _viewManager;
    readonly Func<string, IView> _viewFactory;

    public bool MoveToState(string state, object? data)
    {
        var view = _viewFactory.Invoke(state);
        return _viewManager.LoadView(view);
    }

    public void Init()
    {
        MoveToState("SplashView", null);
    }

    /// <summary>
    /// Initializes a new instance of the StateManager class.
    /// </summary>
    /// <param name="viewManager"></param>
    /// <param name="viewFactory"></param>
    public StateManager(IViewManager viewManager, Func<string, IView> viewFactory)
    {
        _viewManager = viewManager;
        _viewFactory = viewFactory;
    }
}