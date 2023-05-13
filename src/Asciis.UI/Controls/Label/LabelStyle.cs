namespace Asciis.UI.Controls;

public class LabelStyle : Style
{
    public Wrap? TextWrap
    {
        get => Get<Wrap?>(nameof(TextWrap));
        set => Set(nameof(TextWrap), value);
    }

}
