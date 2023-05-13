using No8.Ascii.Console;
using System.IO;

namespace No8.Ascii;

public class Sprite
{
    public Color DefaultForeground = Color.White;
    public Color DefaultBackground = Color.Black;

    public Sprite() { }

    public Sprite(int width, int height)
    {
        Create(width, height);
    }

    public Sprite(string buffer)
    {
        if (!Read(buffer))
            Create(8, 8);
    }

    public int Width;
    public int Height;

    private Rune[] _glyphs = Array.Empty<Rune>();
    private Color[] _foreColours = Array.Empty<Color>();
    private Color[] _backColours = Array.Empty<Color>();

    private void Create(int w, int h)
    {
        Width = w;
        Height = h;
        _glyphs = new Rune[w * h];
        _foreColours = new Color[w * h];
        _backColours = new Color[w * h];
        for (int i = 0; i < w * h; i++)
        {
            _glyphs[i] = Runes.Space;
            _foreColours[i] = DefaultForeground;
            _backColours[i] = DefaultBackground;
        }
    }

    public void SetGlyph(int x, int y, Rune c)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
            return;
        _glyphs[y * Width + x] = c;
    }

    public void SetColour(int x, int y, Color fore, Color back)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
            return;
        _foreColours[y * Width + x] = fore;
        _backColours[y * Width + x] = back;
    }

    public Rune GetGlyph(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
            return Runes.Space;
        return _glyphs[y * Width + x];
    }

    public Color GetForeColour(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
            return Color.White;
        return _foreColours[y * Width + x];
    }
    public Color GetBackColour(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
            return Color.White;
        return _backColours[y * Width + x];
    }

    public Rune SampleGlyph(int x, int y)
    {
        int sx = x * Width;
        int sy = y * Height - 1;
        if (sx < 0 || sx >= Width || sy < 0 || sy >= Height)
            return Runes.Space;
        return _glyphs[sy * Width + sx];
    }

    public Color SampleColour(int x, int y)
    {
        int sx = x * Width;
        int sy = y * Height - 1;
        if (sx < 0 || sx >= Width || sy < 0 || sy >= Height)
            return DefaultForeground;
        return _foreColours[sy * Width + sx];
    }

    public bool Read(string buffer)
    {
        var lines = buffer.Split('\n');
        Width = Convert.ToInt32(lines[0]);
        Height = Convert.ToInt32(lines[1]);
        Create(Width, Height);
        if (_foreColours == null || _backColours == null || _glyphs == null)
            throw new InvalidDataException();

        for (int y = 0; y < Height; y++)
        {
            var line = lines[2 + y].ToRuneList();
            for (int x = 0; x < Width; x++)
                _glyphs[y * Width + x] = line[x];
        }
        for (int y = 0; y < Height; y++)
        {
            var line = lines[2 + Height + y];
            for (int x = 0; x < Width; x++)
                _foreColours[y * Width + x] = Color.FromArgb(Convert.ToInt32(line.Substring(x * 2, 2), 16));
        }
        for (int y = 0; y < Height; y++)
        {
            var line = lines[2 + Height + y];
            for (int x = 0; x < Width; x++)
                _backColours[y * Width + x] = Color.FromArgb(Convert.ToInt32(line.Substring(x * 2, 2), 16));
        }

        return true;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append(Width)
          .AppendCRLF()
          .Append(Height)
          .AppendCRLF();
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
                sb.Append(_glyphs[y * Width + x]);
            sb.AppendCRLF();
        }
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
                sb.Append(_foreColours[y * Width + x].ToArgb().ToString("X8"));
            sb.AppendCRLF();
        }
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
                sb.Append(_backColours[y * Width + x].ToArgb().ToString("X8"));
            sb.AppendCRLF();
        }

        return sb.ToString();
    }
}
