using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using No8.Ascii;
using No8.Ascii.Controls;

// Before the App Loop, write to the current screen
Console.WriteLine(Figlet.Render("Allo, World."));

// This is the basics for a CLI style app
// var exCon = ExConsole.Create(options => { options.StartFullScreen = true; });
// await exCon.Run();

float x = -1;
float y = -1;
float velocityX = 1;
float velocityY = 1;
int frames = 0;
float totalElapsed = 0f;

var app = new AsciiApp(args);

app.AddWindow( new Window(out var window));

window.Update += WindowOnUpdate; 
window.Draw += WindowOnDraw;

await app.StartAsync().ConfigureAwait(false);

Console.WriteLine(Figlet.Render("Bye, World."));

void WindowOnUpdate(object? sender, float elapsedTime)
{
    x += velocityX;
    y += velocityY;

    if (x < window.Bounds.Left)
    {
        x = window.Bounds.Left;
        velocityX = 1;
    }
    else if (x >= window.Bounds.Right - 1)
    {
        x = window.Bounds.Right - 1;
        velocityX = -1;
    }

    if (y < window.Bounds.Top)
    {
        y = window.Bounds.Top;
        velocityY = 1;
    }

    if (y >= window.Bounds.Bottom - 1)
    {
        y = window.Bounds.Bottom - 1;
        velocityY = -1;
    }

    window.NeedsPainting = true;
    frames++;
    totalElapsed += elapsedTime;

    if (totalElapsed > 10f)
    {
        totalElapsed = 0f;
        frames = 0;
    }
}

void WindowOnDraw(object? sender, Canvas canvas)
{
    canvas.SetGlyph((int)x, (int)y, (Rune)'@',
        foreground: Color.Black,
        background: Color.FromArgb(
            Random.Shared.Next(255),
            Random.Shared.Next(255),
            Random.Shared.Next(255))
        );

    var framesPerSecond = (float)frames / totalElapsed;
    canvas.WriteAt(0,0,$" {framesPerSecond:F3} = {frames} : {totalElapsed} ", Color.Black, Color.White);
}