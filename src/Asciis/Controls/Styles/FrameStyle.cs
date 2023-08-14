namespace No8.Ascii.Controls
{
    public class FrameStyle : Style
    {
        public Edges Border
        {
            get => Get<Edges?>(nameof(Border)) ?? Frame.DefaultBorder;
            set => Set(nameof(Border), value);
        }

        public Brush? BorderBrush
        {
            get => Get<Brush?>(nameof(BorderBrush)) ?? Frame.DefaultBorderBrush;
            set => Set(nameof(BorderBrush), value);
        }
        public LineSet? LineSet
        {
            get => Get<LineSet?>(nameof(LineSet)) ?? Frame.DefaultLineSet;
            set => Set(nameof(LineSet), value);
        }
    }
}