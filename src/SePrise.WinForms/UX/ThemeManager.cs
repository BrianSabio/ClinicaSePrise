namespace SePrise.WinForms.UX;
public enum ThemeMode
{
    Light,
    Dark
}
public static class ThemeManager
{
    private static ThemeMode _currentTheme = ThemeMode.Light;
    private static ColorScheme _currentColorScheme = ColorScheme.CreateLightTheme();
    public static event EventHandler<ThemeChangedEventArgs>? ThemeChanged;
public static ColorScheme CurrentColorScheme => _currentColorScheme;
public static ThemeMode CurrentTheme => _currentTheme;
public static void SetTheme(ThemeMode theme)
    {
        _currentTheme = theme;
        _currentColorScheme = theme == ThemeMode.Light 
            ? ColorScheme.CreateLightTheme() 
            : ColorScheme.CreateDarkTheme();

        // Notificar a todos los suscriptores
        ThemeChanged?.Invoke(null, new ThemeChangedEventArgs(theme, _currentColorScheme));
    }
    public static void ToggleTheme()
    {
        SetTheme(_currentTheme == ThemeMode.Light ? ThemeMode.Dark : ThemeMode.Light);
    }
    public static void SaveThemePreference()
    {
    }
    public static void LoadThemePreference()
    {
        SetTheme(ThemeMode.Light);
    }
}
public class ThemeChangedEventArgs : EventArgs
{
    public ThemeMode Theme { get; }
    public ColorScheme ColorScheme { get; }

    public ThemeChangedEventArgs(ThemeMode theme, ColorScheme colorScheme)
    {
        Theme = theme;
        ColorScheme = colorScheme;
    }
}


