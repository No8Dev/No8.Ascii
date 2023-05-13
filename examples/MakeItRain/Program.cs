using No8.Ascii;
using No8.Ascii.Controls.Animation;
using No8.Ascii.ParticleSystem;
using System.Drawing;
using System.Text;

var rnd = Random.Shared;

List<MatrixStream> streams = CreateStreams(Direction.Right);
List<Explosion> explosions = new();
ParticleSystem particleSystem = new();
float nextExplosion = 0f;

var app = new AsciiApp(args);
app.AddWindow( new No8.Ascii.Controls.Window(out var window) );

window.Update += (object? sender, float elapsed) =>
{
    foreach (var stream in streams.ToArray())
    {
        stream.Update(elapsed);

        if (stream.Pos > stream.Length + (stream.NumChars * 2))
            streams.Remove(stream);

        else if (!stream.Flag && stream.Pos > stream.Length)
        {
            stream.Flag = true;
            streams.Add(CreateSteam(stream.Index, stream.Direction));
        }
    }

    nextExplosion += elapsed;
    if (nextExplosion >= 1f)
    {
        nextExplosion = 0f;
        CreateExplosion();
    }

    foreach (var explosion in explosions.ToArray())
    {
        explosion.Update(elapsed);
        if (explosion.Completed)
            explosions.Remove(explosion);
    }
    particleSystem.Update(elapsed);

    window.NeedsPainting = true;
};

window.Draw += (object? _, Canvas canvas) =>
{
    canvas.Background = Color.Black;
    canvas.Foreground = Color.White;

    foreach (var stream in streams)
    {
        for (int offset = 0; offset < stream.Length; offset++)
        {
            var transparency = stream.TransparencyPercentage(offset);

            int x = stream.Direction switch
            {
                Direction.Left => stream.Length - offset - 1,
                Direction.Up => stream.Index,
                Direction.Right => offset,
                Direction.Down => stream.Index,
                _ => default
            };
            int y = stream.Direction switch
            {
                Direction.Left => stream.Index,
                Direction.Up => stream.Length - offset - 1,
                Direction.Right => stream.Index,
                Direction.Down => offset,
                _ => default
            };

            if (transparency <= 0)
                canvas.WriteAt(x, y,
                    ' ',
                    Color.White,
                    Color.Black);
            else
                canvas.WriteAt(x, y, 
                    stream.Get(offset), 
                    FakeOpacity(Color.DarkGreen, transparency), 
                    Color.Black );
        }
        int leadX = 0, leadY = 0;
        switch(stream.Direction)
        {
            case Direction.Down:
                leadX = stream.Index;
                leadY = stream.Pos;
                break;
            case Direction.Up:
                leadX = stream.Index;
                leadY = stream.Length - stream.Pos - 1;
                break;
            case Direction.Right:
                leadX = stream.Pos;
                leadY = stream.Index;
                break;
            case Direction.Left:
                leadX = stream.Length - stream.Pos - 1;
                leadY = stream.Index;
                break;
        }
        canvas.WriteAt(leadX, leadY, stream.LeadChar, Color.White, FakeOpacity(Color.DarkBlue, 0.7f));
    }

    foreach (var explosion in explosions)
        explosion.Draw(canvas);

    particleSystem.Draw(canvas);
};

await app.StartAsync();


//-----------------------------------------------------------------------------------

// Create Random sreams
List<MatrixStream> CreateStreams(Direction? direction = null)
{
    var streamCount = Math.Max(Console.WindowWidth, Console.WindowHeight);
    var streams = new List<MatrixStream>();

    for (int index = 0; index < streamCount; index++)
    {
        streams.Add(CreateSteam(
            index, 
            direction ?? rnd.Next(4) switch
            {
                0 => Direction.Left,
                1 => Direction.Up,
                2 => Direction.Right,
                _ => Direction.Down
            }, 
            -1));
    }

    return streams;
}

MatrixStream CreateSteam(int index, Direction direction, int pos = 0)
{
    var length = (direction == Direction.Down || direction == Direction.Up)
        ? Console.WindowHeight
        : Console.WindowWidth;
    return new MatrixStream(index, length, direction, pos: pos);
}

void CreateExplosion()
{
    var x = rnd.Next(5, Console.WindowWidth - 5);
    var y = rnd.Next(2, Console.WindowHeight - 2);
    var size = rnd.Next(10, 20);

    explosions.Add(new Explosion(2f, x, y, size));
    explosions.Add(new Explosion(5f, x, y, size + 5));

    for (int i = 0; i < 200; i++)
        particleSystem.Add(new Particle(
            new VecF(x, y),
            new VecF(Random.Shared.NextSingle() * 8f - 4f, Random.Shared.NextSingle() * 5f - 2.5f),
            lifespan: Random.Shared.NextSingle() * 3f + 2,
            onDraw: drawParticle));

    void drawParticle(Particle particle, Canvas canvas) 
    {
        char rune = particle.PercentRemaining switch
        {
            > 0.9f => '@',
            > 0.7f => 'W',
            > 0.5f => 'o',
            > 0.3f => '+',
            > 0.1f => '~',
            _ => ' '
        };
        Color? fore = particle.PercentRemaining switch
        {
            < 0.1f => null,
            _ => Color.Black
        };
        Color back = particle.PercentRemaining switch
        {
            > 0.8f => Color.FromArgb(0xFFffFF),
            > 0.7f => Color.FromArgb(0xFFff00),
            > 0.6f => Color.FromArgb(0xFF6600),
            > 0.5f => Color.FromArgb(0xFF0000),
            > 0.4f => Color.FromArgb(0xAA0000),
            > 0.3f => Color.FromArgb(0x990000),
            > 0.2f => Color.FromArgb(0x660000),
            > 0.1f => Color.FromArgb(0x330000),
            _ => Color.FromArgb(0x110000)
        };

        canvas.WriteAt(
            (int)particle.Location.X, 
            (int)particle.Location.Y, 
            (Rune)rune, fore, back);
    }
}

// Console protocol doesn't support transparency, so fake it to the black background
Color FakeOpacity(Color rgb, float opacity)
{
    if (opacity > 1f)
        return rgb;
    if (opacity <= 0f)
        return Color.Black;

    var c = ColorHelpers.BlendLAB(rgb, Color.Black, opacity);
    return c;
}

public class MatrixStream
{
    private Rune[] _chars;
    float fPos = 0f;
    Random rnd = Random.Shared;

    public Rune LeadChar { get; private set; }
    public int Index { get; private set; }
    public int Length { get; }
    public int NumChars { get; }
    public int Pos => (int)fPos;
    public float Speed { get; private set; }
    public Direction Direction { get; }
    public bool Flag { get; set; }

    /// <param name="index">Placeholder index</param>
    /// <param name="maxLength">Max length of stream</param>
    /// <param name="speed">Milliseconds it takes to cross the distance (default: 3 - 10 seconds)</param>
    /// <param name="numChars">Number of characters to display (default: 30 - 50% of maxLength)</param>
    /// <param name="pos">Initial position of leading character</param>
    public MatrixStream(
        int index, 
        int maxLength,
        Direction direction,
        float speed = -1f, 
        int numChars = -1, 
        int pos = -1)
    {
        Index = index;
        Length = maxLength;
        Speed = (speed < 0)
            ? (rnd.NextSingle() * 7f) + 3f
            : speed;
        fPos = (pos < 0) 
            ? rnd.NextSingle() * Length
            : pos;
        _chars = new Rune[maxLength];
        Direction = direction;
        NumChars = (numChars < 0)
            ? Speed switch
            {
                > 9f => (int)(Length * 0.5f),       // Slow
                > 8f => (int)(Length * 0.4f),
                > 7f => (int)(Length * 0.35f),
                > 6f => (int)(Length * 0.3f),
                > 5f => (int)(Length * 0.25f),
                > 4f => (int)(Length * 0.2f),
                > 0f => (int)(Length * 0.1f),       // Fast
                _ => rnd.Next((int)(Length * 0.3), (int)(Length * 0.5))
            }
            : numChars;


        for (int i = 0; i < maxLength; i++)
            _chars[i] = RandomRune();

        LeadChar = RandomRune();
    }

    public void Update(float elapsed)
    {
        var oldPos = Pos;
        fPos += ((Length + NumChars) / Speed) * elapsed;
        if (Pos != oldPos)
            LeadChar = RandomRune();
    }

    public Rune Get(int i)
    {
        if (i == Pos)
            return LeadChar;
        return _chars[i];
    }

    public float TransparencyPercentage(int i)
    {
        if (i == Pos)
            return 1f;
        if (i < Pos - NumChars || i > Pos)
            return 0f;

        var distanceFromLead = (i < 0) ? Pos + i : Pos - i;
        return ((float)distanceFromLead / (float)NumChars);
    }

    Rune RandomRune()
    {
        if (Speed > 9) return "*WMB8&%$#@".RandomItem();
        if (Speed > 8) return "oahkbdpqwm".RandomItem();
        if (Speed > 7) return "LCJUYXZO0Q".RandomItem();
        if (Speed > 6) return "rcvunxzjft".RandomItem();
        if (Speed > 5) return "/\\|()1{}[]".RandomItem();
        if (Speed > 4) return "-_+<>i!lI?".RandomItem();
        if (Speed > 0) return ".'`,^:\";~".RandomItem();

        return (Rune)(rnd.Next(0x1E, 0x7a));
    }
}

public enum Direction
{
    Left, Up, Right, Down
}

public class Explosion
{
    float totalTime;
    float remaining;
    int _x;
    int _y;
    int _size;

    public Explosion(float time, int x, int y, int size = 10)
    {
        totalTime = remaining = time;
        _x = x;
        _y = y;
        _size = size;
    }

    public void Update(float elapsed)
    {
        remaining -= elapsed;
    }

    public bool Completed => remaining < 0f;

    public void Draw(Canvas canvas)
    {
        if (remaining <= 0)
            return;

        var perc = remaining / totalTime;
        perc = Easings.CubicIn.Ease(perc);
        int radius = _size - (int)(_size * perc);

        DrawExplosion(canvas, perc, radius);
    }

    public void DrawExplosion(Canvas canvas, float perc, int radius)
    {
        char rune = perc switch
        {
            > 0.9f => '@',
            > 0.7f => 'W',
            > 0.5f => 'o',
            > 0.3f => '+',
            > 0.1f => '~',
            _      => ' '
        };
        Color? fore = perc switch
        {
            < 0.1f => null,
            _      => Color.Black
        };
        Color back = perc switch
        {
            > 0.8f => Color.FromArgb(0xFFffFF),
            > 0.7f => Color.FromArgb(0xFFff00),
            > 0.6f => Color.FromArgb(0xFF6600),
            > 0.5f => Color.FromArgb(0xFF0000),
            > 0.4f => Color.FromArgb(0xAA0000),
            > 0.3f => Color.FromArgb(0x990000),
            > 0.2f => Color.FromArgb(0x660000),
            > 0.1f => Color.FromArgb(0x330000),
            _      => Color.FromArgb(0x110000)
        };

        canvas.DrawCircle(_x, _y, radius, (Rune)0, fore, back);
    }


}
