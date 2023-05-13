namespace Asciis.App;

/// <summary>
///     provides the sync context set while executing code in Terminal.Gui, to let users use async/await on their code
/// </summary>
internal class AsciiAppSyncContext : SynchronizationContext
{
    private readonly AsciiApp _asciiApp;

    public AsciiAppSyncContext(AsciiApp asciiApp) { _asciiApp = asciiApp; }

    public override SynchronizationContext CreateCopy() { return new AsciiAppSyncContext(_asciiApp); }

    public override void Post(SendOrPostCallback callback, object? state)
    {
        _asciiApp.AddIdle(
            () =>
            {
                callback(state);
                return false;
            });
    }

    public override void Send(SendOrPostCallback callback, object? state)
    {
        _asciiApp.Invoke(() => { callback(state); });
    }
}


internal class AsciiSlimAppSyncContext : SynchronizationContext
{
    private readonly AsciiApp _app;

    public AsciiSlimAppSyncContext(AsciiApp app) { _app = app; }

    public override SynchronizationContext CreateCopy() { return new AsciiSlimAppSyncContext(_app); }

    public override void Post(SendOrPostCallback callback, object? state)
    {
        _app.AddIdle(
            () =>
            {
                callback(state);
                return false;
            });
    }

    public override void Send(SendOrPostCallback callback, object? state)
    {
        _app.Invoke(() => { callback(state); });
    }
}

