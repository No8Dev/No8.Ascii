using System.ComponentModel;

namespace Asciis.Terminal.Views.TextValidateProviders;


/// <summary>
/// .Net MaskedTextProvider Provider for TextValidateField.
/// <para></para>
/// <para><a href="https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.maskedtextprovider?view=net-5.0">Wrapper around MaskedTextProvider</a></para>
/// <para><a href="https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.maskedtextbox.mask?view=net-5.0">Masking elements</a></para>
/// </summary>
public class NetMaskedTextProvider : ITextValidateProvider
{
    private MaskedTextProvider provider;

    /// <summary>
    /// Empty Constructor
    /// </summary>
    public NetMaskedTextProvider(string mask) { Mask = mask; }

    /// <summary>
    /// Mask property
    /// </summary>
    public string Mask
    {
        get => provider?.Mask;
        set
        {
            var current = provider != null ? provider.ToString(false, false) : string.Empty;
            provider = new MaskedTextProvider(value == string.Empty ? "&&&&&&" : value.ToString());
            if (string.IsNullOrEmpty(current) == false) provider.Set(current);
        }
    }

    ///<inheritdoc/>
    public string Text
    {
        get => provider.ToString();
        set => provider.Set(value.ToString());
    }

    ///<inheritdoc/>
    public bool IsValid => provider.MaskCompleted;

    ///<inheritdoc/>
    public bool Fixed => true;

    ///<inheritdoc/>
    public string DisplayText => provider.ToDisplayString();

    ///<inheritdoc/>
    public int Cursor(int pos)
    {
        if (pos < 0)
        {
            return CursorStart();
        }
        else if (pos > provider.Length)
        {
            return CursorEnd();
        }
        else
        {
            var p = provider.FindEditPositionFrom(pos, false);
            if (p == -1) p = provider.FindEditPositionFrom(pos, true);
            return p;
        }
    }

    ///<inheritdoc/>
    public int CursorStart()
    {
        return
            provider.IsEditPosition(0)
                ? 0
                : provider.FindEditPositionFrom(0, true);
    }

    ///<inheritdoc/>
    public int CursorEnd()
    {
        return
            provider.IsEditPosition(provider.Length - 1)
                ? provider.Length - 1
                : provider.FindEditPositionFrom(provider.Length, false);
    }

    ///<inheritdoc/>
    public int CursorLeft(int pos)
    {
        var c = provider.FindEditPositionFrom(pos - 1, false);
        return c == -1 ? pos : c;
    }

    ///<inheritdoc/>
    public int CursorRight(int pos)
    {
        var c = provider.FindEditPositionFrom(pos + 1, true);
        return c == -1 ? pos : c;
    }

    ///<inheritdoc/>
    public bool Delete(int pos)
    {
        return provider.Replace(' ', pos); // .RemoveAt (pos);
    }

    ///<inheritdoc/>
    public bool InsertAt(char ch, int pos) { return provider.Replace(ch, pos); }
}
