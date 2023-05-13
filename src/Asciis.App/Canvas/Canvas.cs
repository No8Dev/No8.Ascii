using System.Data;
using System.Diagnostics.CodeAnalysis;
using Asciis.App.Controls;

namespace Asciis.App;

using static Pixel;

public class Canvas
{
    private static readonly Rune DefaultChar = new(' '); // space
    private static readonly Rune Zero        = new(0);

    /// The current canvas state
    //public Array2D<Glyph> Glyphs { get; private set; }
    public Glyph[] Glyphs { get; private set; }

    public int Width { get; private set; }
    public int Height { get; private set; }
    public Vec Size { get; private set; }

    public Color Background { get; set; } = Color.Black;
    public Color Foreground { get; set; } = Color.White;

    public Vec Cursor { get; set; } = Vec.Unknown;

    private readonly Stack<Rect> _clips = new();
    private          Rect?       _clip;

    public Canvas(int width, int height)
    {
        Width = width;
        Height = height;
        Size = new Vec(width, height);

        Glyphs = new Glyph[width * height];
        var len = Glyphs.Length;
        for (int i = 0; i < len; i++)
            Glyphs[i] = new Glyph(DefaultChar, Foreground, Background);
    }

    public Glyph this[int y, int x]
    {
        get => Glyphs[y.Clamp(0, Height - 1) * Width + x.Clamp(0, Width - 1)];
        set => Glyphs[y.Clamp(0, Height - 1) * Width + x.Clamp(0, Width - 1)] = value;
    }

    public Glyph this[int index] => Glyphs[index];

    public void Clear()
    {
        Fill(0, 0, Width, Height, DefaultChar, Foreground, Background);
    }

    private bool IsOutsideClip(int x, int y)
    {
        int clipX = _clip?.Left ?? 0;
        int clipY = _clip?.Top ?? 0;
        int clipMaxX = clipX + (_clip?.Width ?? Width);
        int clipMaxY = clipY + (_clip?.Height ?? Height);

        if (x < clipX) return true;
        if (x >= clipMaxX) return true;
        if (y < clipY) return true;
        if (y >= clipMaxY) return true;

        return false;
    }

    public void SetGlyph(float x, float y, Glyph glyph) => SetGlyph((int)x, (int)y, glyph);

    public void SetGlyph(int x, int y, Glyph glyph)
    {
        SetGlyph(x, y, glyph.Chr, glyph.Fore, glyph.Back, glyph.OffsetX, glyph.OffsetY);
    }

    public void SetGlyph(int x, int y, Rune chr, Color? foreground = null, Color? background = null, float offsetX = 0f, float offsetY = 0f)
    {
        if (IsOutsideClip(x, y))
            return;

        var glyph = this[y, x];

        glyph.OffsetX = offsetX;
        glyph.OffsetY = offsetY;

        if (chr != Zero)
            glyph.Chr = chr;
        else if (glyph.Chr == Zero)
            glyph.Chr = DefaultChar;
        if (foreground != null)
            glyph.Fore = foreground;
        if (background != null)
            glyph.Back = background;
    }

    public void WriteAt(int x, int y, string text, Color? foreground = null, Color? background = null)
    {
        if (x < 0) x = Width - text.Length;

        for (var i = 0; i < text.Length; i++)
        {
            if (x + i >= Width) break;
            SetGlyph(x + i, y, (Rune)text[i], foreground, background);
        }
    }

    public void WriteAt(int x, int y, char chr, Color? foreground = null, Color? background = null)
    {
        SetGlyph(x, y, (Rune)chr, foreground, background);
    }

    public void WriteAt(int x, int y, Rune rune, Color? foreground = null, Color? background = null)
    {
        SetGlyph(x, y, rune, foreground, background);
    }

    public void WriteAt(float x, float y, char chr, Color? foreground = null, Color? background = null)
    {
        SetGlyph((int)x, (int)y, (Rune)chr, foreground, background);
    }

    public void Resize(int wide, int high)
    {
        var newGlyphs = new Glyph[wide * high];

        for (int x = 0; x < Math.Min(Width, wide); x++)
        {
            for (int y = 0; y < Math.Min(Height, high); y++)
                newGlyphs[y * wide + x] = this[y, x];
        }
        for (int i = 0; i < newGlyphs.Length; i++)
            newGlyphs[i] ??= new Glyph(DefaultChar, Background, Foreground);

        Width = wide;
        Height = high;
        Size = new Vec(Width, Height);
        Glyphs = newGlyphs;
    }

    public override string ToString()
    {
        var sb = new StringBuilder(Width * Height + Height);

        for (int y = 0; y < Height; y++)
        {
            var line = new StringBuilder(Width + 2);
            for (int x = 0; x < Width; x++)
            {
                var g = this[y, x];
                if (g == Glyph.Clear || g.Chr == Zero)
                    line.Append(' ');
                else
                    line.Append(g.Chr);
            }
            sb.Append(line.ToString().TrimEnd());
            if (y < Height - 1)
                sb.AppendLine();
        }

        return sb.ToString().TrimEnd('\r', '\n');
    }

    public void FillRect(Rect rect, Rune? chr = null, Color? foreground = null, Color? background = null)
    {
        int left = rect.Left;
        int right = rect.Right;
        int top = rect.Top;
        int bottom = rect.Bottom;

        for (int y = top; y <= bottom; y++)
        {
            for (int x = left; x <= right; x++)
            {
                WriteAt(x, y, chr ?? Zero, foreground, background);
            }
        }
    }

    private bool IsLine(Rune rune)
    {
        return DoubleLine.Contains(rune) ||
               SingleLine.Contains(rune) ||
               RoundLine.Contains(rune) ||
               DoubleHorz.Contains(rune) ||
               SingleHorz.Contains(rune);
    }

    private LineDrawSet? GetLineDrawSet(LineSet lineSet)
    {
        return lineSet switch
        {
            LineSet.Single => SingleLine,
            LineSet.Double => DoubleLine,
            LineSet.Rounded => RoundLine,
            LineSet.SingleHorz => SingleHorz,
            LineSet.DoubleHorz => DoubleHorz,
            LineSet.DoubleOver => SingleLine,
            LineSet.DoubleUnder => SingleLine,
            LineSet.DoubleRaised => SingleLine,
            LineSet.DoublePressed => SingleLine,
            LineSet.None => NoLine,
            _ => null
        };
    }

    public void DrawLine(
        RectF   rect,
        LineSet lineSet    = LineSet.Single,
        Color?  foreground = null,
        Color?  background = null) => DrawLine((int)rect.Left, (int)rect.Top, (int)rect.Right, (int)rect.Bottom, lineSet, foreground, background);
    public void DrawLine(int startX, int startY, int endX, int endY,
                          LineSet lineSet = LineSet.Single,
                          Color? foreground = null,
                          Color? background = null)
    {
        LineDrawSet lineDraw = GetLineDrawSet(lineSet) ?? throw new ArgumentException(nameof(lineSet));

        // Force line to go left to right
        if (endX < startX)
        {
            var tmp = startX;
            startX = endX;
            endX = tmp;
            tmp = startY;
            startY = endY;
            endY = tmp;
        }
        var width = endX - startX; // always zero or positive
        var height = endY - startY; // may be negative
        bool goingUp = endY < startY;
        bool horizontal = width > Math.Abs(height);

        float stepX;
        float stepY;
        int stepCount;

        if (horizontal)
        {
            stepX = 1f;
            if (height < 0)
                stepY = (height - 0.9f) / width;
            else if (height == 0)
                stepY = 0;
            else
                stepY = (height + 0.9f) / width;

            stepCount = width + 1;
        }
        else
        {
            stepX = width == 0 ? 0 : (width + 0.9f) / height;
            stepY = goingUp ? -1f : 1f;
            stepCount = Math.Abs(height) + 1;
        }
        int lastX = -1;
        int lastY = -1;
        for (int step = 0; step < stepCount; step++)
        {
            var x = startX + stepX * step;
            var y = startY + stepY * step;

            var yToCompare = (int)(goingUp
                                        ? Math.Ceiling(y)
                                        : y);

            Rune chr;
            if (horizontal)
            {
                if (step == 0)
                    chr = lineDraw.HorzStart;
                else if (lastY != yToCompare)
                {
                    if (goingUp)
                    {
                        chr = TryCombine((int)x, lastY, lineDraw.BotRight, lineSet);
                        WriteAt((int)x, lastY, chr, foreground, background);
                    }
                    else
                    {
                        chr = TryCombine((int)x, lastY, lineDraw.TopRight, lineSet);
                        WriteAt((int)x, lastY, chr, foreground, background);
                    }
                    chr = goingUp ? lineDraw.TopLeft : lineDraw.BotLeft;
                }
                else if (step == stepCount - 1)
                    chr = lineDraw.HorzEnd;
                else
                    chr = lineDraw.Horz;
            }
            else
            {
                if (step == 0)
                    chr = goingUp ? lineDraw.VertEnd : lineDraw.VertStart;
                else if (lastX != (int)x)
                {
                    chr = TryCombine(lastX, (int)y, goingUp ? lineDraw.TopRight : lineDraw.BotLeft, lineSet);
                    WriteAt(lastX, (int)y, chr, foreground, background);
                    chr = goingUp ? lineDraw.BotLeft : lineDraw.TopRight;
                }
                //else if (nextX != (int)x)
                //    chr = lineDraw.BotLeft;
                else if (step == stepCount - 1)
                    chr = goingUp ? lineDraw.VertStart : lineDraw.VertEnd;
                else
                    chr = lineDraw.Vert;
            }
            chr = TryCombine((int)x, yToCompare, chr, lineSet);
            WriteAt((int)x, yToCompare, chr, foreground, background);

            lastX = (int)x;
            lastY = yToCompare;
        }
    }

    public void PaintBackground(
        RectF         rect,
        Brush brush) =>
        PaintBackground((int)rect.Left, (int)rect.Top, (int)rect.Right, (int)rect.Bottom, brush);
    public void PaintBackground(
        int           left,
        int           top,
        int           right,
        int           bottom,
        Brush brush)
    {
        if (brush is GradientBrush gradientBrush && gradientBrush.Direction == GradientDirection.Vertical)
        {
            var height = bottom - top + 1;

            for (int y = top; y < bottom; y++)
            {
                var percent = (float)(y - top) / (float)height;
                var color   = brush.GetColorAt(percent);

                for (int x = left; x < right; x++)
                    WriteAt(x, y, Zero, background: color);
            }
        }
        else
        {
            var width  = right - left + 1;

            for (int x = left; x < right; x++)
            {
                var percent = (float)(x - left) / (float)width;
                var color   = brush.GetColorAt(percent);

                for (int y = top; y < bottom; y++)
                    WriteAt(x, y, Zero, background: color);
            }
        }
    }

    public void PaintForeground(
        RectF         rect,
        Brush brush) =>
        PaintForeground((int)rect.Left, (int)rect.Top, (int)rect.Right, (int)rect.Bottom, brush);
    public void PaintForeground(
        int           left,
        int           top,
        int           right,
        int           bottom,
        Brush brush)
    {
        if (brush is GradientBrush gradientBrush && gradientBrush.Direction == GradientDirection.Vertical)
        {
            var height = bottom - top + 1;

            for (int y = top; y < bottom; y++)
            {
                var percent = (float)(y - top) / (float)height;
                var color   = brush.GetColorAt(percent);

                for (int x = left; x < right; x++)
                    WriteAt(x, y, Zero, foreground: color);
            }
        }
        else
        {
            var width  = right - left + 1;

            for (int x = left; x < right; x++)
            {
                var percent = (float)(x - left) / (float)width;
                var color   = brush.GetColorAt(percent);

                for (int y = top; y < bottom; y++)
                    WriteAt(x, y, Zero, foreground: color);
            }
        }
    }

    public void PaintBorderBackground(
        RectF         rect,
        Brush brush) =>
        PaintBorderBackground((int)rect.Left, (int)rect.Top, (int)rect.Right, (int)rect.Bottom, brush);

    public void PaintBorderBackground(
        int           left,
        int           top,
        int           right,
        int           bottom,
        Brush brush)
    {
        if (brush is GradientBrush gradientBrush && gradientBrush.Direction == GradientDirection.Vertical)
        {
            var height = bottom - top + 1;

            for (int y = top; y < bottom; y++)
            {
                var percent = (float)(y - top) / (float)height;
                var color   = brush.GetColorAt(percent);

                for (int x = left; x < right; x++)
                {
                    if (x == left || x == right - 1 || y == top || y == bottom - 1)
                        WriteAt(x, y, Zero, background: color);
                }
            }
        }
        else
        {
            var width = right - left + 1;

            for (int x = left; x < right; x++)
            {
                var percent = (float)(x - left) / (float)width;
                var color   = brush.GetColorAt(percent);

                for (int y = top; y < bottom; y++)
                {
                    if (x == left || x == right - 1 || y == top || y == bottom - 1)
                        WriteAt(x, y, Zero, background: color);
                }
            }
        }
    }

    public void PaintBorderForeground(
        RectF         rect,
        Brush brush) =>
        PaintBorderForeground((int)rect.Left, (int)rect.Top, (int)rect.Right, (int)rect.Bottom, brush);
    public void PaintBorderForeground(
        int           left,
        int           top,
        int           right,
        int           bottom,
        Brush brush)
    {
        if (brush is GradientBrush gradientBrush && gradientBrush.Direction == GradientDirection.Vertical)
        {
            var height = bottom - top + 1;

            for (int y = top; y < bottom; y++)
            {
                var percent = (float)(y - top) / (float)height;
                var color   = brush.GetColorAt(percent);

                for (int x = left; x < right; x++)
                {
                    if (x == left || x == right - 1 || y == top || y == bottom - 1)
                        WriteAt(x, y, Zero, foreground: color);
                }
            }
        }
        else
        {
            var width = right - left + 1;

            for (int x = left; x < right; x++)
            {
                var percent = (float)(x - left) / (float)width;
                var color   = brush.GetColorAt(percent);

                for (int y = top; y < bottom; y++)
                {
                    if (x == left || x == right - 1 || y == top || y == bottom - 1)
                        WriteAt(x, y, Zero, foreground: color);
                }
            }
        }
    }

    public void DrawRect(
        Rect    rect,
        LineSet lineSet    = LineSet.Single,
        Brush?  foreground = null,
        Brush?  background = null)
    {
        DrawRect(rect.Left, rect.Top, rect.Right, rect.Bottom, lineSet, foreground, background);
    }

    public void DrawRect(
        int left,
        int top,
        int right,
        int bottom,
        LineSet lineSet = LineSet.Single,
        Brush? foregroundBrush = null,
        Brush? backgroundBrush = null)
    {
        Color? foreground = (foregroundBrush as SolidColorBrush)?.Color;
        Color? background = (backgroundBrush as SolidColorBrush)?.Color;

        if (foregroundBrush is not null)
            PaintForeground(left, top, right, bottom, foregroundBrush);
        if (backgroundBrush is not null)
            PaintBackground(left, top, right, bottom, backgroundBrush);

        if (lineSet is LineSet.DoubleUnder 
            or LineSet.DoubleOver 
            or LineSet.DoubleRaised 
            or LineSet.DoublePressed)
        {
            // Draw singles
            // Top
            if (lineSet is LineSet.DoubleUnder or LineSet.DoubleRaised)
                DrawLine(left, top, right, top, LineSet.Single, foreground, background);
            // Left
            if (lineSet is LineSet.DoubleUnder or LineSet.DoubleOver or LineSet.DoubleRaised)
                DrawLine(left, top, left, bottom, LineSet.Single, foreground, background);
            // Right
            if (lineSet is LineSet.DoubleUnder or LineSet.DoubleOver or LineSet.DoublePressed)
                DrawLine(right, top, right, bottom, LineSet.Single, foreground, background);
            // Bottom
            if (lineSet is LineSet.DoubleOver or LineSet.DoublePressed)
                DrawLine(left, bottom, right, bottom, LineSet.Single, foreground, background);

            // Draw Doubles
            // Top
            if (lineSet is LineSet.DoubleOver or LineSet.DoublePressed)
                DrawLine(left, top, right, top, LineSet.Double, foreground, background);
            // Left
            if (lineSet == LineSet.DoublePressed)
                DrawLine(left, top, left, bottom, LineSet.Double, foreground, background);
            // Right
            if (lineSet == LineSet.DoubleRaised)
                DrawLine(right, top, right, bottom, LineSet.Double, foreground, background);
            // Bottom
            if (lineSet is LineSet.DoubleUnder or LineSet.DoubleRaised)
                DrawLine(left, bottom, right, bottom, LineSet.Double, foreground, background);

            return;
        }

        LineDrawSet lineDraw = GetLineDrawSet(lineSet) ?? throw new ArgumentException(nameof(lineSet));

        for (int x = left; x <= right; x++)
        {
            for (int y = top; y <= bottom; y++)
            {
                Rune chr = (Rune)' ';
                if (x == left)
                {
                    if (y == top)
                        chr = lineDraw.TopLeft;
                    else if (y == bottom)
                        chr = lineDraw.BotLeft;
                    else
                        chr = lineDraw.Vert;
                }
                else if (x == right)
                {
                    if (y == top)
                        chr = lineDraw.TopRight;
                    else if (y == bottom)
                        chr = lineDraw.BotRight;
                    else
                        chr = lineDraw.Vert;
                }
                else if (y == top || y == bottom)
                    chr = lineDraw.Horz;
                else if (background == null)
                    continue;

                chr = TryCombine(x, y, chr, lineSet);
                WriteAt(x, y, chr, foreground, background);
            }
        }
    }

    private static void Swap<T>(ref T x, ref T y)
    {
        (x, y) = (y, x);
    }

    private Rune TryCombine(int x, int y, Rune ch, LineSet lineSet)
    {
        // Can we combine this character with whatever is underneath
        var oldGlyph = this[y, x];
        var oldCh = oldGlyph.Chr;

        if (IsLine(oldCh))
        {
            if (lineSet == LineSet.Single && SingleLine.Contains(oldCh))
                return SingleLine.Combine(oldCh, ch);

            if (lineSet == LineSet.Rounded && RoundLine.Contains(oldCh))
                return RoundLine.Combine(oldCh, ch);

            if (lineSet == LineSet.Double && DoubleLine.Contains(oldCh))
                return DoubleLine.Combine(oldCh, ch);

            return TryCombine(oldCh, ch);
        }

        return ch;
    }

    private Rune TryCombine(Rune oldCh, Rune ch)
    {
        int underIndex = -1;
        int overIndex = -1;
        if (DoubleLine.Contains(oldCh))
        {
            underIndex = DoubleLine.IndexOf(oldCh);
            overIndex = SingleLine.IndexOf(ch);
        }
        else if (SingleLine.Contains(oldCh))
        {
            underIndex = SingleLine.IndexOf(oldCh);
            overIndex = DoubleLine.IndexOf(ch);
        }
        else if (RoundLine.Contains(oldCh))
        {
            underIndex = RoundLine.IndexOf(oldCh);
            overIndex = DoubleLine.IndexOf(ch);
        }

        if (underIndex >= 0 && overIndex >= 0)
        {
            if (ch == SingleLine.Horz ||
                 ch == SingleLine.HorzStart ||
                 ch == SingleLine.HorzEnd ||
                 ch == DoubleLine.Vert ||
                 ch == DoubleLine.VertStart ||
                 ch == DoubleLine.VertEnd)
                return SingleHorz[underIndex | overIndex];
            if (ch == DoubleLine.Horz ||
                 ch == DoubleLine.HorzStart ||
                 ch == DoubleLine.HorzEnd ||
                 ch == SingleLine.Vert ||
                 ch == SingleLine.VertStart ||
                 ch == SingleLine.VertEnd)
                return DoubleHorz[underIndex | overIndex];
        }

        return ch;
    }

    public void CloneTo(Glyph[] glyphArray)
    {
        if (glyphArray == null) return;

        var len = Width * Height;
        for (int i = 0; i < len; i++)
            glyphArray[i].CopyFrom( Glyphs[i] );
        
    }

    public virtual void Draw(int x, int y, Rune? c = null, Color? foreground = null, Color? background = null)
    {
        SetGlyph(x, y, c ?? Zero, foreground, background);
    }

    public void Fill(
        int x,
        int y,
        int width,
        int height,
        Rune? c = null,
        Color? foreground = null,
        Color? background = null)
    {
        c ??= Zero;

        for (var py = y; py < y + height; py++)
        {
            for (var px = x; px < x + width; px++)
            {
                SetGlyph(px, py, c ?? Zero, foreground, background);
            }
        }
    }

    public void DrawString(int x, int y, string str, Brush? foregroundBrush = null, Brush? backgroundBrush = null)
    {
        Color? foreground = (foregroundBrush as SolidColorBrush)?.Color;
        Color? background = (backgroundBrush as SolidColorBrush)?.Color;

        if (foregroundBrush is GradientBrush foreGradientBrush)
            PaintForeground(x, y, str.Length, 1, foreGradientBrush);
        if (backgroundBrush is GradientBrush backGradientBrush)
            PaintBackground(x, y, str.Length, 1, backGradientBrush);

        for (int i = 0; i < str.Length; i++)
            SetGlyph(x + i, y, (Rune)str[i], foreground, background);
    }

    public void DrawStringAlpha(int x, int y, string c, Color? foreground = null, Color? background = null)
    {
        for (int i = 0; i < c.Length; i++)
        {
            if (c[i] != ' ')
                SetGlyph(x + i, y, (Rune)c[i], foreground, background);
        }
    }

    public void Clip(ref int x, ref int y)
    {
        if (x < 0)
            x = 0;
        if (x >= Width)
            x = Width;
        if (y < 0)
            y = 0;
        if (y >= Height)
            y = Height;
    }

    public void DrawLine(int x1, int y1, int x2, int y2, Rune? c = null, Color? foreground = null, Color? background = null)
    {
        c ??= Zero;
        int x, y, dx, dy, dx1, dy1, px, py, xe, ye;
        dx = x2 - x1; dy = y2 - y1;
        dx1 = Math.Abs(dx); dy1 = Math.Abs(dy);
        px = 2 * dy1 - dx1; py = 2 * dx1 - dy1;
        if (dy1 <= dx1)
        {
            if (dx >= 0)
            { x = x1; y = y1; xe = x2; }
            else
            { x = x2; y = y2; xe = x1; }

            Draw(x, y, c, foreground, background);

            while (x < xe)
            {
                x = x + 1;
                if (px < 0)
                    px = px + 2 * dy1;
                else
                {
                    if (dx < 0 && dy < 0 || dx > 0 && dy > 0) y = y + 1; else y = y - 1;
                    px = px + 2 * (dy1 - dx1);
                }
                Draw(x, y, c, foreground, background);
            }
        }
        else
        {
            if (dy >= 0)
            { x = x1; y = y1; ye = y2; }
            else
            { x = x2; y = y2; ye = y1; }

            Draw(x, y, c, foreground, background);

            while (y < ye)
            {
                y = y + 1;
                if (py <= 0)
                    py = py + 2 * dx1;
                else
                {
                    if (dx < 0 && dy < 0 || dx > 0 && dy > 0) x = x + 1; else x = x - 1;
                    py = py + 2 * (dx1 - dy1);
                }
                Draw(x, y, c, foreground, background);
            }
        }
    }

    public void DrawTriangle(int x1, int y1, int x2, int y2, int x3, int y3, Rune c, Color? foreground = null, Color? background = null)
    {
        DrawLine(x1, y1, x2, y2, c, foreground, background);
        DrawLine(x2, y2, x3, y3, c, foreground, background);
        DrawLine(x3, y3, x1, y1, c, foreground, background);
    }

    // https://www.avrfreaks.net/sites/default/files/triangles.c
    public void FillTriangle(int x1, int y1, int x2, int y2, int x3, int y3, Rune c, Color? foreground = null, Color? background = null)
    {
        int t1x, t2x, y, minx, maxx, t1xp, t2xp;
        bool changed1 = false;
        bool changed2 = false;
        int signx1, signx2;
        int e1, e2;

        // Sort vertices
        if (y1 > y2) { Swap(ref y1, ref y2); Swap(ref x1, ref x2); }
        if (y1 > y3) { Swap(ref y1, ref y3); Swap(ref x1, ref x3); }
        if (y2 > y3) { Swap(ref y2, ref y3); Swap(ref x2, ref x3); }

        t1x = t2x = x1;
        y = y1;   // Starting points
        var dx1 = x2 - x1;
        if (dx1 < 0)
        {
            dx1 = -dx1;
            signx1 = -1;
        }
        else
            signx1 = 1;
        var dy1 = y2 - y1;

        var dx2 = x3 - x1;
        if (dx2 < 0)
        {
            dx2 = -dx2;
            signx2 = -1;
        }
        else
            signx2 = 1;
        var dy2 = y3 - y1;

        if (dy1 > dx1)
        {   // swap values
            Swap(ref dx1, ref dy1);
            changed1 = true;
        }
        if (dy2 > dx2)
        {   // swap values
            Swap(ref dy2, ref dx2);
            changed2 = true;
        }

        e2 = dx2 >> 1;
        // Flat top, just process the second half
        if (y1 == y2) goto next;
        e1 = dx1 >> 1;

        for (int i = 0; i < dx1;)
        {
            t1xp = 0; t2xp = 0;
            if (t1x < t2x)
            {
                minx = t1x;
                maxx = t2x;
            }
            else
            {
                minx = t2x;
                maxx = t1x;
            }
            // process first line until y value is about to change
            while (i < dx1)
            {
                i++;
                e1 += dy1;
                while (e1 >= dx1)
                {
                    e1 -= dx1;
                    if (changed1)
                        t1xp = signx1;
                    else
                        goto next1;
                }
                if (changed1)
                    break;
                t1x += signx1;
            }
        next1:

            // process second line until y value is about to change
            while (true)
            {
                e2 += dy2;
                while (e2 >= dx2)
                {
                    e2 -= dx2;
                    if (changed2) t2xp = signx2;
                    else goto next2;
                }
                if (changed2)
                    break;
                t2x += signx2;
            }
        next2:

            if (minx > t1x)
                minx = t1x;
            if (minx > t2x)
                minx = t2x;
            if (maxx < t1x)
                maxx = t1x;
            if (maxx < t2x)
                maxx = t2x;
            Drawline(minx, maxx, y, c, foreground, background);    // Draw line from min to max points found on the y
                                                                 // Now increase y
            if (!changed1)
                t1x += signx1;
            t1x += t1xp;
            if (!changed2)
                t2x += signx2;
            t2x += t2xp;
            y += 1;
            if (y == y2)
                break;

        }
    next:

        // Second half
        dx1 = x3 - x2;
        if (dx1 < 0)
        {
            dx1 = -dx1;
            signx1 = -1;
        }
        else
            signx1 = 1;
        dy1 = y3 - y2;
        t1x = x2;

        if (dy1 > dx1)
        {   // swap values
            Swap(ref dy1, ref dx1);
            changed1 = true;
        }
        else
            changed1 = false;

        e1 = dx1 >> 1;

        for (int i = 0; i <= dx1; i++)
        {
            t1xp = 0; t2xp = 0;
            if (t1x < t2x)
            {
                minx = t1x;
                maxx = t2x;
            }
            else
            {
                minx = t2x;
                maxx = t1x;
            }
            // process first line until y value is about to change
            while (i < dx1)
            {
                e1 += dy1;
                while (e1 >= dx1)
                {
                    e1 -= dx1;

                    if (changed1)
                    {
                        t1xp = signx1;
                        break;
                    } //t1x += signx1;
                    goto next3;
                }
                if (changed1)
                    break;
                t1x += signx1;
                if (i < dx1)
                    i++;
            }
        next3:

            // process second line until y value is about to change
            while (t2x != x3)
            {
                e2 += dy2;
                while (e2 >= dx2)
                {
                    e2 -= dx2;
                    if (changed2)
                        t2xp = signx2;
                    else
                        goto next4;
                }
                if (changed2)
                    break;
                t2x += signx2;
            }
        next4:

            if (minx > t1x)
                minx = t1x;
            if (minx > t2x)
                minx = t2x;
            if (maxx < t1x)
                maxx = t1x;
            if (maxx < t2x)
                maxx = t2x;
            Drawline(minx, maxx, y, c, foreground, background);
            if (!changed1)
                t1x += signx1;
            t1x += t1xp;
            if (!changed2)
                t2x += signx2;
            t2x += t2xp;
            y += 1;
            if (y > y3)
                return;
        }
    }

    public void DrawCircle(int xc, int yc, int r, Rune c, Color? foreground = null, Color? background = null)
    {
        int x = 0;
        int y = r;
        int p = 3 - 2 * r;
        if (r == 0) return;

        while (y >= x) // only formulate 1/8 of circle
        {
            Draw(xc - x, yc - (int)(y*0.7), c, foreground, background);   //upper left left
            Draw(xc - y, yc - (int)(x*0.7), c, foreground, background);   //upper upper left
            Draw(xc + y, yc - (int)(x*0.7), c, foreground, background);   //upper upper right
            Draw(xc + x, yc - (int)(y*0.7), c, foreground, background);   //upper right right
            Draw(xc - x, yc + (int)(y*0.7), c, foreground, background);   //lower left left
            Draw(xc - y, yc + (int)(x*0.7), c, foreground, background);   //lower lower left
            Draw(xc + y, yc + (int)(x*0.7), c, foreground, background);   //lower lower right
            Draw(xc + x, yc + (int)(y*0.7), c, foreground, background);   //lower right right
            if (p < 0)
                p += 4 * x++ + 6;
            else
                p += 4 * (x++ - y--) + 10;
        }
    }

    public void FillCircle(int xc, int yc, int r, Rune c, Color? foreground = null, Color? background = null)
    {
        int x = 0;
        int y = r;
        int p = 3 - 2 * r;
        if (r == 0)
            return;

        while (y >= x)
        {
            Drawline(xc - x, xc + x, yc - (int)(y*0.7), c, foreground, background);
            Drawline(xc - y, xc + y, yc - (int)(x*0.7), c, foreground, background);
            Drawline(xc - x, xc + x, yc + (int)(y*0.7), c, foreground, background);
            Drawline(xc - y, xc + y, yc + (int)(x*0.7), c, foreground, background);
            if (p < 0) p += 4 * x++ + 6;
            else p += 4 * (x++ - y--) + 10;
        }
    }

    private void Drawline(int sx, int ex, int ny, Rune c, Color? foreground = null, Color? background = null)
    {
        for (int i = sx; i <= ex; i++)
            Draw(i, ny, c, foreground, background);
    }

    public void DrawSprite(int x, int y, [NotNull] Sprite sprite)
    {
        for (int i = 0; i < sprite.Width; i++)
        {
            for (int j = 0; j < sprite.Height; j++)
            {
                if (sprite.GetGlyph(i, j) != Runes.Space)
                    Draw(x + i, y + j, sprite.GetGlyph(i, j), sprite.GetForeColour(i, j));
            }
        }
    }

    public Sprite? ExportSprite(int x, int y, int width, int height)
    {
        if (x < 0 || y < 0 || x + width >= Width || y + height >= Height)
            return null;

        var sprite = new Sprite(width, height);
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                var glyph = this[ y + j, x + i];
                sprite.SetGlyph(i, j, glyph.Chr);
                sprite.SetColour(i, j, glyph.Fore ?? Foreground, glyph.Back ?? Background);
            }
        }

        return sprite;
    }

    public void DrawPartialSprite(int x, int y, [NotNull] Sprite sprite, int ox, int oy, int w, int h)
    {
        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                if (sprite.GetGlyph(i + ox, j + oy) != Runes.Space)
                    Draw(x + i, y + j, sprite.GetGlyph(i + ox, j + oy), sprite.GetForeColour(i + ox, j + oy));
            }
        }
    }

    public void DrawWireFrameModel(
        List<(float, float)> coordinates,
        float x,
        float y,
        float r = 0.0f,
        float s = 1.0f,
        Color? foreground = null,
        Color? background = null,
        Rune? c = null)
    {
        c ??= Pixel.Block.Solid;
        int count = coordinates.Count;
        var transformedCoordinates = new (float, float)[count];

        for (int i = 0; i < count; i++)
        {
            float item1 = coordinates[i].Item1;
            float item2 = coordinates[i].Item2;

            // Rotate
            if (Math.Abs(r) > float.Epsilon)
            {
                item1 = (float)(coordinates[i].Item1 * Math.Cos(r) - coordinates[i].Item2 * Math.Sin(r));
                item2 = (float)(coordinates[i].Item1 * Math.Sin(r) + coordinates[i].Item2 * Math.Cos(r));
            }

            // Scale
            if (Math.Abs(s - 1.0) > float.Epsilon)
            {
                item1 *= s;
                item2 *= s;
            }

            // Translate
            transformedCoordinates[i] = (item1 + x, item2 + y);
        }

        // Draw Closed Polygon
        for (int i = 0; i < count + 1; i++)
        {
            int j = i + 1;
            DrawLine(
                     (int)transformedCoordinates[i % count].Item1,
                     (int)transformedCoordinates[i % count].Item2,
                     (int)transformedCoordinates[j % count].Item1,
                     (int)transformedCoordinates[j % count].Item2,
                     c, foreground, background);
        }
    }

    internal bool PushClip(Rect? clip)
    {
        if (clip == null)
            return false;
        _clips.Push(clip);
        _clip = clip;
        return true;
    }

    internal void PopClip()
    {
        if (_clips.Count <= 0)
        {
            _clip = null;
            return;
        }
        _clip = _clips.Pop();
    }

    public void Overwrite(Canvas other)
    {
        var minWidth = Math.Min(Width, other.Width);
        var minHeight = Math.Min(Height, other.Height);

        for (int y = 0; y < minHeight; y++)
        {
            for (int x = 0; x < minWidth; x++)
            {
                var g = other[y, x];
                if (g?.Chr.Value >= 32)     // Printable character
                    this[y, x] = other[y, x];
            }
        }
    }
}
