using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace Asciis.UI.Controls;

public class SkinManager
{
    private static SkinManager? _instance;

    public static SkinManager Instance =>
        _instance ??= new SkinManager();

    public List<Skin> Skins = new();
    public Skin Current { get; private set; }

    private SkinManager()
    {
        var baseFrameStyle = new FrameStyle
        {
            Background = Color.Black,
            Foreground = Color.White,
            BorderColor = Color.Blue,
            Border = 1,
            LineSet = LineSet.Single,
            HorzPosition = LayoutPosition.Stretch,
            VertPosition = LayoutPosition.Stretch
        };
        var baseTextEditStyle = new TextEditStyle
        {
            Background = Color.DarkBlue,
            Foreground = Color.White,
            HorzPosition = LayoutPosition.Start,
            VertPosition = LayoutPosition.Start
        };
        var baseLabelStyle = new LabelStyle
        {
            Foreground = Color.Black,
            HorzPosition = LayoutPosition.Start,
            VertPosition = LayoutPosition.Start
        };
        var baseButtonStyle = new ButtonStyle
        {
            Background = Color.LightGray,
            Foreground = Color.Black,
            BorderColor = Color.DarkBlue,
            Border = 1,
            Padding = new Edges(1, 0, 1, 0),
            LineSet = LineSet.DoubleUnder,
            HorzPosition = LayoutPosition.Start,
            VertPosition = LayoutPosition.Start,
            MinWidth = 5,
            MinHeight = 3
        };
        var baseCheckboxStyle = new CheckboxStyle
        {
            Background = Color.LightGray,
            Foreground = Color.Black,
            HorzPosition = LayoutPosition.Start,
            VertPosition = LayoutPosition.Start,
            MinWidth = 5,
            MinHeight = 1
        };
        var baseOptionboxStyle = new OptionboxStyle
        {
            Background = Color.LightGray,
            Foreground = Color.Black,
            HorzPosition = LayoutPosition.Start,
            VertPosition = LayoutPosition.Start,
            MinWidth = 5,
            MinHeight = 1
        };
        var baseStackStyle = new StackStyle
        {
            Background = Color.Black,
            Foreground = Color.White,
            Direction = Direction.Vertical,
            HorzPosition = LayoutPosition.Start,
            VertPosition = LayoutPosition.Start
        };


        Current = new Skin("Blue")
                 .Set(typeof(Frame).ToString(), baseFrameStyle)
                 .Set(typeof(Label).ToString(), baseLabelStyle)
                 .Set(typeof(TextEdit).ToString(), baseTextEditStyle)
                 .Set(typeof(Button).ToString(), baseButtonStyle)
                 .Set(typeof(Checkbox).ToString(), baseCheckboxStyle)
                 .Set(typeof(Optionbox).ToString(), baseOptionboxStyle)
                 .Set(typeof(Stack).ToString(), baseStackStyle)
                 .Set(
                      Skin.RootFrameKey,
                      new FrameStyle { BasedOn = baseFrameStyle, Background = Color.Black, Foreground = Color.White, LineSet = LineSet.None })
                 .Set(
                      Skin.ContentFrameKey,
                      new FrameStyle
                      {
                          BasedOn = baseFrameStyle,
                          Background = Color.White,
                          Foreground = Color.Blue,
                          BorderColor = Color.LightGray,
                          LineSet = LineSet.Double
                      })
                 .Set(
                      Skin.AltContentFrameKey,
                      new FrameStyle
                      {
                          BasedOn = baseFrameStyle,
                          Background = Color.White,
                          Foreground = Color.Green,
                          BorderColor = Color.GreenYellow,
                          LineSet = LineSet.Single
                      })
                 .Set(
                      Skin.ButtonKey,
                      new ButtonStyle { BasedOn = baseFrameStyle, Background = Color.LightBlue, Foreground = Color.DarkBlue, BorderColor = Color.BlueViolet });

        Skins.Add(Current);
    }

    public Skin? Select(string name)
    {
        var skin = Skins.FirstOrDefault(s => s.SkinName == name);
        if (skin != null)
            Current = skin;

        return skin;
    }
}

public class Skin
{
    public const string RootFrameKey = "RootFrame";
    public const string ContentFrameKey = "ContentFrame";
    public const string AltContentFrameKey = "AltContentFrame";
    public const string ButtonKey = "Button";

    public string SkinName { get; }
    private readonly Dictionary<string, Style> _controlStyles;

    public Skin(string skinName, IDictionary<string, Style>? controlStyles = null)
    {
        SkinName = skinName;
        _controlStyles = controlStyles == null
                             ? new Dictionary<string, Style>()
                             : new Dictionary<string, Style>(controlStyles);
    }

    public Skin Set(string key, Style style)
    {
        _controlStyles[key] = style;
        return this;
    }

    public void Apply([NotNull] Control control)
    {
        var style = GetStyle(control);
        if (control.Style != null)
            style?.CombineWith(control.Style);

        foreach (var child in control)
            Apply(child);
    }

    public Style? GetStyle(Control control)
    {
        var typeStyle = GetStyle(control.GetType().ToString());
        if (!string.IsNullOrWhiteSpace(control.StyleKey))
        {
            var keyStyle = GetStyle(control.StyleKey);
            if (keyStyle != null && typeStyle != null)
                return keyStyle.CombineWith(typeStyle);
        }

        return typeStyle;
    }
    public Style? GetStyle(string key)
    {
        if (!string.IsNullOrEmpty(key))
        {
            if (_controlStyles.ContainsKey(key))
                return _controlStyles[key];
        }

        return null;
    }
}
