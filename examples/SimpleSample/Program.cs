//#define STRESS_OUTPUT

using System.Diagnostics;
using System.Drawing;
using System.Text;
using No8.Ascii;
using No8.Ascii.ElementLayout;
using No8.Ascii.Controls;
using No8.Ascii.VirtualTerminal;

char[] values = " .,! abcdefghijklmnopqrstuvwxyz".ToArray();

Console.InputEncoding  = Encoding.UTF8;
Console.OutputEncoding = Encoding.UTF8;
Console.WriteLine(Figlet.Render("Simple Sample"));
Console.WriteLine();
Console.Write("\x1b[32m");
Console.WriteLine(@"
     ╭────────────╮
     │ Loading... │
     ╰────────────╯");

Console.Write(Terminal.Mode.LocatorReportingCells);

while (true)
{
    if (Console.KeyAvailable)
    {
        var key = Console.ReadKey(false);
        if (char.IsControl(key.KeyChar))
            Debug.Write($"{key.KeyChar:X2}");
        else
            Debug.Write($"{key.KeyChar}");
    }
}

/*


var app = new AsciiApp(args);

var labelStyle = new LabelStyle
                 {
                     BackgroundBrush = Color.LightGreen,
                     ForegroundBrush = Color.OrangeRed
                 };

var window = new Window
             {
                 new Frame(
                        new FrameLayoutPlan{ Height = 100.Percent(), Border = 1, Padding = 1 },
                        new FrameStyle
                        {
                            //ForegroundBrush = Color.GreenYellow,
                            BackgroundBrush   = new GradientBrush(ColorTranslator.FromHtml("#EE6FF8"), ColorTranslator.FromHtml("#5A56E0"), GradientDirection.Vertical),
                            BorderBrush = new GradientBrush(ColorTranslator.FromHtml("#5A56E0"), ColorTranslator.FromHtml("#EE6FF8")),
                        })
                 {
                     "Master Frame",
                     new Stack(out var frame1, style:new Style{ BackgroundBrush = Color.OrangeRed })
                     {
                         new Label(out var label1, style: labelStyle) { Text = "Label text. Click to add label" },
                         new Label(out var label2, style: labelStyle, plan: new LayoutPlan
                         {
                             ElementsWrap = LayoutWrap.Wrap,
                             Width = 100.Percent(),
                             Height = 100.Percent()
                         }) { Text = "Second label" }
                     },
                     new Frame(
                         new FrameLayoutPlan
                         {
                             Width  = 14,
                             Height = 8,
                             AlignSelf_Cross = AlignmentLine_Cross.End,
                             PositionType = PositionType.Absolute
                         },
                         new FrameStyle
                         {
                             BorderBrush     = Color.BurlyWood,
                             BackgroundBrush = Color.NavajoWhite,
                             LineSet         = LineSet.DoubleOver
                         }
                     )
                 }
             };

label1.PointerPressed += (_, _) =>
{
    frame1.Insert(1, new Label { Text = "Dynamic label" });
    Debug.WriteLine("NEW LABEL");
};


#if STRESS_OUTPUT
// Stress the output
var sb = new StringBuilder(120*50);

window.Update += (object? sender, float e) =>
{
    var max = Random.Shared.Next(80 * 25) + (80 * 25);
    sb.Clear();
    for (int i = 0; i < max; i++)
        sb.Append(values.RandomItem());
    label2.Text = sb.ToString();
};

var sw = Stopwatch.StartNew();
sw.Start();

window.Draw += (object? _, Canvas canvas) =>
{
    var ms = $"{sw.ElapsedMilliseconds}ms";
    canvas.DrawString(canvas.Width - ms.Length - 1, 0, ms);

    sw.Restart();
};
#endif


#if DEBUG
// for testing only
window.DoLayout(100, 20);  
#endif

app.AddWindow( window );
await app.StartAsync();

*/