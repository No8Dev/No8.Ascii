using System.Diagnostics.CodeAnalysis;

namespace Asciis.Terminal.Core.Clipboard;

/// <summary>
/// Shared abstract class to enforce rules from the implementation of the <see cref="IClipboard"/> interface.
/// </summary>
public abstract class ClipboardBase : IClipboard
{
    private string? contents = null;

    /// <summary>
    /// Get or sets the operation system clipboard, otherwise the contents field.
    /// </summary>
    public string? Contents
    {
        get
        {
            try
            {
                if (IsSupported == true)
                    return GetClipboardData();
                else
                    return contents;
            }
            catch (Exception)
            {
                return contents;
            }
        }
        set
        {
            try
            {
                if (IsSupported == true && value is not null)
                    SetClipboardData(value.ToString());
                contents = value;
            }
            catch (Exception)
            {
                contents = value;
            }
        }
    }

    /// <summary>
    /// Returns true if the environmental dependencies are in place to interact with the OS clipboard
    /// </summary>
    public abstract bool IsSupported { get; }

    /// <summary>
    /// Get the operation system clipboard.
    /// </summary>
    /// <exception cref="NotSupportedException">Thrown if it was not possible to read the clipboard contents</exception>
    public string GetClipboardData()
    {
        try
        {
            return GetClipboardDataImpl();
        }
        catch (Exception ex)
        {
            throw new NotSupportedException("Failed to read clipboard.", ex);
        }
    }

    /// <summary>
    /// Get the operation system clipboard.
    /// </summary>
    protected abstract string GetClipboardDataImpl();

    /// <summary>
    /// Sets the operation system clipboard.
    /// </summary>
    /// <param name="text"></param>
    /// <exception cref="NotSupportedException">Thrown if it was not possible to set the clipboard contents</exception>
    public void SetClipboardData(string text)
    {
        try
        {
            SetClipboardDataImpl(text);
        }
        catch (Exception ex)
        {
            throw new NotSupportedException("Failed to write to clipboard.", ex);
        }
    }

    /// <summary>
    /// Sets the operation system clipboard.
    /// </summary>
    /// <param name="text"></param>
    protected abstract void SetClipboardDataImpl(string text);

    /// <summary>
    /// Gets the operation system clipboard if possible.
    /// </summary>
    /// <param name="result">Clipboard contents read</param>
    /// <returns>true if it was possible to read the OS clipboard.</returns>
    public bool TryGetClipboardData( [NotNullWhen(true)] out string result)
    {
        // Don't even try to read because environment is not set up.
        if (!IsSupported)
        {
            result = contents ?? String.Empty;
            return true;
        }

        try
        {
            result = GetClipboardDataImpl();
            while (result == null) 
                result = GetClipboardDataImpl();
            return true;
        }
        catch (Exception)
        {
            result = contents ?? String.Empty;
            return false;
        }
    }

    /// <summary>
    /// Sets the operation system clipboard if possible.
    /// </summary>
    /// <param name="text"></param>
    /// <returns>True if the clipboard content was set successfully</returns>
    public bool TrySetClipboardData(string text)
    {
        // Don't even try to set because environment is not set up
        if (!IsSupported)
        {
            contents = text;
            return true;
        }

        try
        {
            SetClipboardDataImpl(text);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
