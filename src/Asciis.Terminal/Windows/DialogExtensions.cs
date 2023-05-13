namespace Asciis.Terminal.Windows;

public static class DialogExtensions
{
    public static Dialog CreateDialog(this AsciiApplication application, string title, int width, int height, params Button[] buttons)
    {
        return new Dialog(application, title, width, height, buttons);
    }

    public static Dialog CreateDialog(this AsciiApplication application)
    {
        return new Dialog(application, string.Empty, 0, 0);
    }

    public static Dialog CreateDialog(this AsciiApplication application, string title, params Button[] buttons)
    {
        return new Dialog(application, title, 0, 0, buttons);
    }
}
