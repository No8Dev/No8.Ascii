namespace No8.Ascii.Controls
{
    public class RowStyle : Style
    {
        private readonly Style _style;

        public RowStyle(Style style)
        {
            _style = style;
        }

        public RowStyle()
        {
            _style = this;
        }
        
        public Edges Border
        {
            get => _style.Get<Edges?>(nameof(Border)) ?? Row.DefaultBorder;
            set => _style.Set(nameof(Border), value);
        }

        public Brush? BorderBrush
        {
            get => _style.Get<Brush?>(nameof(BorderBrush)) ?? Row.DefaultBorderBrush;
            set => _style.Set(nameof(BorderBrush), value);
        }
        public LineSet? LineSet
        {
            get => _style.Get<LineSet?>(nameof(LineSet)) ?? Row.DefaultLineSet;
            set => _style.Set(nameof(LineSet), value);
        }
    }
}