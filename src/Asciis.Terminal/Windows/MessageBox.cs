using Asciis.Terminal.Helpers;

namespace Asciis.Terminal.Windows;

/// <summary>
/// MessageBox displays a modal message to the user, with a title, a message and a series of options that the user can choose from.
/// </summary>
/// <para>
///   The difference between the <see cref="Query(string, string, string[])"/> and <see cref="ErrorQuery(string, string, string[])"/> 
///   method is the default set of colors used for the message box.
/// </para>
/// <para>
/// The following example pops up a <see cref="MessageBox"/> with the specified title and text, plus two <see cref="Button"/>s.
/// The value -1 is returned when the user cancels the <see cref="MessageBox"/> by pressing the ESC key.
/// </para>
/// <example>
/// <code lang="c#">
/// var n = MessageBox.Query ("Quit Demo", "Are you sure you want to quit this demo?", "Yes", "No");
/// if (n == 0)
///    quit = true;
/// else
///    quit = false;
/// </code>
/// </example>
public static class MessageBox
{
    /// <summary>
    /// Presents a normal <see cref="MessageBox"/> with the specified title and message and a list of buttons to show to the user.
    /// </summary>
    /// <returns>The index of the selected button, or -1 if the user pressed ESC to close the dialog.</returns>
    /// <param name="width">Width for the window.</param>
    /// <param name="height">Height for the window.</param>
    /// <param name="title">Title for the query.</param>
    /// <param name="message">Message to display, might contain multiple lines.</param>
    /// <param name="buttons">Array of buttons to add.</param>
    /// <remarks>
    /// Use <see cref="Query(string, string, string[])"/> instead; it automatically sizes the MessageBox based on the contents.
    /// </remarks>
    public static int Query(AsciiApplication application, int width, int height, string title, string message, params string[] buttons)
    {
        return QueryFull(application, false, width, height, title, message, 0, buttons);
    }

    /// <summary>
    /// Presents an error <see cref="MessageBox"/> with the specified title and message and a list of buttons to show to the user.
    /// </summary>
    /// <returns>The index of the selected button, or -1 if the user pressed ESC to close the dialog.</returns>
    /// <param name="title">Title for the query.</param>
    /// <param name="message">Message to display, might contain multiple lines.</param>
    /// <param name="buttons">Array of buttons to add.</param>
    /// <remarks>
    /// The message box will be vertically and horizontally centered in the container and the size will be automatically determined
    /// from the size of the message and buttons.
    /// </remarks>
    public static int Query(AsciiApplication application, string title, string message, params string[] buttons)
    {
        return QueryFull(application, false, 0, 0, title, message, 0, buttons);
    }

    /// <summary>
    /// Presents an error <see cref="MessageBox"/> with the specified title and message and a list of buttons to show to the user.
    /// </summary>
    /// <returns>The index of the selected button, or -1 if the user pressed ESC to close the dialog.</returns>
    /// <param name="width">Width for the window.</param>
    /// <param name="height">Height for the window.</param>
    /// <param name="title">Title for the query.</param>
    /// <param name="message">Message to display, might contain multiple lines.</param>
    /// <param name="buttons">Array of buttons to add.</param>
    /// <remarks>
    /// Use <see cref="ErrorQuery(string, string, string[])"/> instead; it automatically sizes the MessageBox based on the contents.
    /// </remarks>
    public static int ErrorQuery(AsciiApplication application, int width, int height, string title, string message, params string[] buttons)
    {
        return QueryFull(application, true, width, height, title, message, 0, buttons);
    }

    /// <summary>
    /// Presents an error <see cref="MessageBox"/> with the specified title and message and a list of buttons to show to the user.
    /// </summary>
    /// <returns>The index of the selected button, or -1 if the user pressed ESC to close the dialog.</returns>
    /// <param name="title">Title for the query.</param>
    /// <param name="message">Message to display, might contain multiple lines.</param>
    /// <param name="buttons">Array of buttons to add.</param>
    /// <remarks>
    /// The message box will be vertically and horizontally centered in the container and the size will be automatically determined
    /// from the size of the title, message. and buttons.
    /// </remarks>
    public static int ErrorQuery(AsciiApplication application, string title, string message, params string[] buttons)
    {
        return QueryFull(application, true, 0, 0, title, message, 0, buttons);
    }

    /// <summary>
    /// Presents a normal <see cref="MessageBox"/> with the specified title and message and a list of buttons to show to the user.
    /// </summary>
    /// <returns>The index of the selected button, or -1 if the user pressed ESC to close the dialog.</returns>
    /// <param name="width">Width for the window.</param>
    /// <param name="height">Height for the window.</param>
    /// <param name="title">Title for the query.</param>
    /// <param name="message">Message to display, might contain multiple lines.</param>
    /// <param name="defaultButton">Index of the default button.</param>
    /// <param name="buttons">Array of buttons to add.</param>
    /// <remarks>
    /// Use <see cref="Query(string, string, string[])"/> instead; it automatically sizes the MessageBox based on the contents.
    /// </remarks>
    public static int Query(
        AsciiApplication application,
        int width,
        int height,
        string title,
        string message,
        int defaultButton = 0,
        params string[] buttons)
    {
        return QueryFull(application, false, width, height, title, message, defaultButton, buttons);
    }

    /// <summary>
    /// Presents an error <see cref="MessageBox"/> with the specified title and message and a list of buttons to show to the user.
    /// </summary>
    /// <returns>The index of the selected button, or -1 if the user pressed ESC to close the dialog.</returns>
    /// <param name="title">Title for the query.</param>
    /// <param name="message">Message to display, might contain multiple lines.</param>
    /// <param name="defaultButton">Index of the default button.</param>
    /// <param name="buttons">Array of buttons to add.</param>
    /// <remarks>
    /// The message box will be vertically and horizontally centered in the container and the size will be automatically determined
    /// from the size of the message and buttons.
    /// </remarks>
    public static int Query(AsciiApplication application, string title, string message, int defaultButton = 0, params string[] buttons)
    {
        return QueryFull(application, false, 0, 0, title, message, defaultButton, buttons);
    }

    /// <summary>
    /// Presents an error <see cref="MessageBox"/> with the specified title and message and a list of buttons to show to the user.
    /// </summary>
    /// <returns>The index of the selected button, or -1 if the user pressed ESC to close the dialog.</returns>
    /// <param name="width">Width for the window.</param>
    /// <param name="height">Height for the window.</param>
    /// <param name="title">Title for the query.</param>
    /// <param name="message">Message to display, might contain multiple lines.</param>
    /// <param name="defaultButton">Index of the default button.</param>
    /// <param name="buttons">Array of buttons to add.</param>
    /// <remarks>
    /// Use <see cref="ErrorQuery(string, string, string[])"/> instead; it automatically sizes the MessageBox based on the contents.
    /// </remarks>
    public static int ErrorQuery(
        AsciiApplication application,
        int width,
        int height,
        string title,
        string message,
        int defaultButton = 0,
        params string[] buttons)
    {
        return QueryFull(application, true, width, height, title, message, defaultButton, buttons);
    }

    /// <summary>
    /// Presents an error <see cref="MessageBox"/> with the specified title and message and a list of buttons to show to the user.
    /// </summary>
    /// <returns>The index of the selected button, or -1 if the user pressed ESC to close the dialog.</returns>
    /// <param name="title">Title for the query.</param>
    /// <param name="message">Message to display, might contain multiple lines.</param>
    /// <param name="defaultButton">Index of the default button.</param>
    /// <param name="buttons">Array of buttons to add.</param>
    /// <remarks>
    /// The message box will be vertically and horizontally centered in the container and the size will be automatically determined
    /// from the size of the title, message. and buttons.
    /// </remarks>
    public static int ErrorQuery(AsciiApplication application, string title, string message, int defaultButton = 0, params string[] buttons)
    {
        return QueryFull(application, true, 0, 0, title, message, defaultButton, buttons);
    }

    private static int QueryFull(
        AsciiApplication application, 
        bool useErrorColors,
        int width,
        int height,
        string title,
        string message,
        int defaultButton = 0,
        params string[] buttons)
    {
        const int defaultWidth = 50;
        var textWidth = TextFormatter.MaxWidth(message, width == 0 ? defaultWidth : width);
        var textHeight = TextFormatter.MaxLines(message, textWidth); // message.Count (string.Make ('\n')) + 1;
        var msgboxHeight = Math.Max(1, textHeight) + 3; // textHeight + (top + top padding + buttons + bottom)

        // Create button array for Dialog
        var count = 0;
        List<Button> buttonList = new();
        if (buttons != null && defaultButton > buttons.Length - 1) defaultButton = buttons.Length - 1;
        foreach (var s in buttons)
        {
            var b = new Button(s);
            if (count == defaultButton) b.IsDefault = true;
            buttonList.Add(b);
            count++;
        }

        // Create Dialog (retain backwards compat by supporting specifying height/width)
        Dialog d;
        if ((width == 0) & (height == 0))
        {
            d = application.CreateDialog(title, buttonList.ToArray());
            d.Height = msgboxHeight;
        }
        else
        {
            d = application.CreateDialog(title, Math.Max(width, textWidth) + 4, height, buttonList.ToArray());
        }

        if (useErrorColors) d.ColorScheme = Colors.Error;

        if (message != null)
        {
            var l = new Label(textWidth > width ? 0 : (width - 4 - textWidth) / 2, 1, message);
            l.LayoutStyle = LayoutStyle.Computed;
            l.TextAlignment = TextAlignment.Centered;
            l.X = Pos.Center();
            l.Y = Pos.Center();
            l.Width = Dim.Fill(2);
            l.Height = Dim.Fill(1);
            d.Add(l);
        }

        // Dynamically size Width
        var msgboxWidth = Math.Max(
            defaultWidth,
            Math.Max(
                title.RuneCount() + 8,
                Math.Max(
                    textWidth + 4,
                    d.GetButtonsWidth()) + 8)); // textWidth + (left + padding + padding + right)
        d.Width = msgboxWidth;

        // Setup actions
        var clicked = -1;
        for (var n = 0; n < buttonList.Count; n++)
        {
            var buttonId = n;
            var b = buttonList[n];
            b.Clicked += () =>
            {
                clicked = buttonId;
                application.RequestStop();
            };
            if (b.IsDefault) b.SetFocus();
        }

        // Run the modal; do not shutdown the mainloop driver when done
        application.Run(d);
        return clicked;
    }
}
