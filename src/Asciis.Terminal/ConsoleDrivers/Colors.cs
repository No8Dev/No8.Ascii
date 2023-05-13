using System.Runtime.CompilerServices;

namespace Asciis.Terminal.Core;

/// <summary>
/// The default <see cref="ColorScheme"/>s for the application.
/// </summary>
public static class Colors
{
    static Colors()
    {
        // Use reflection to dynamically create the default set of ColorSchemes from the list defined 
        // by the class. 
        ColorSchemes = typeof(Colors).GetProperties()
                                     .Where(p => p.PropertyType == typeof(ColorScheme))
                                     .Select(
                                          p => new KeyValuePair<string, ColorScheme>(
                                              p.Name,
                                              new ColorScheme())) // (ColorScheme)p.GetValue (p)))
                                     .ToDictionary(t => t.Key, t => t.Value);
    }

    /// <summary>
    /// The application toplevel color scheme, for the default toplevel views.
    /// </summary>
    /// <remarks>
    /// <para>
    ///	This API will be deprecated in the future. Use <see cref="Colors.ColorSchemes"/> instead (e.g. <c>edit.ColorScheme = Colors.ColorSchemes["TopLevel"];</c>
    /// </para>
    /// </remarks>
    public static ColorScheme TopLevel
    {
        get => GetColorScheme();
        set => SetColorScheme(value);
    }

    /// <summary>
    /// The base color scheme, for the default toplevel views.
    /// </summary>
    /// <remarks>
    /// <para>
    ///	This API will be deprecated in the future. Use <see cref="Colors.ColorSchemes"/> instead (e.g. <c>edit.ColorScheme = Colors.ColorSchemes["Base"];</c>
    /// </para>
    /// </remarks>
    public static ColorScheme Base
    {
        get => GetColorScheme();
        set => SetColorScheme(value);
    }

    /// <summary>
    /// The dialog color scheme, for standard popup dialog boxes
    /// </summary>
    /// <remarks>
    /// <para>
    ///	This API will be deprecated in the future. Use <see cref="Colors.ColorSchemes"/> instead (e.g. <c>edit.ColorScheme = Colors.ColorSchemes["Dialog"];</c>
    /// </para>
    /// </remarks>
    public static ColorScheme Dialog
    {
        get => GetColorScheme();
        set => SetColorScheme(value);
    }

    /// <summary>
    /// The menu bar color
    /// </summary>
    /// <remarks>
    /// <para>
    ///	This API will be deprecated in the future. Use <see cref="Colors.ColorSchemes"/> instead (e.g. <c>edit.ColorScheme = Colors.ColorSchemes["Menu"];</c>
    /// </para>
    /// </remarks>
    public static ColorScheme Menu
    {
        get => GetColorScheme();
        set => SetColorScheme(value);
    }

    /// <summary>
    /// The color scheme for showing errors.
    /// </summary>
    /// <remarks>
    /// <para>
    ///	This API will be deprecated in the future. Use <see cref="Colors.ColorSchemes"/> instead (e.g. <c>edit.ColorScheme = Colors.ColorSchemes["Error"];</c>
    /// </para>
    /// </remarks>
    public static ColorScheme Error
    {
        get => GetColorScheme();
        set => SetColorScheme(value);
    }

    private static ColorScheme GetColorScheme([CallerMemberName] string callerMemberName = null)
    {
        return ColorSchemes[callerMemberName];
    }

    private static void SetColorScheme(ColorScheme colorScheme, [CallerMemberName] string callerMemberName = null)
    {
        ColorSchemes[callerMemberName] = colorScheme;
        colorScheme.caller = callerMemberName;
    }

    /// <summary>
    /// Provides the defined <see cref="ColorScheme"/>s.
    /// </summary>
    public static Dictionary<string, ColorScheme> ColorSchemes { get; }
}
