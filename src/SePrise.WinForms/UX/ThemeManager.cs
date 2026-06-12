namespace SePrise.WinForms.UX;

/// <summary>
/// Enumeración de los temas disponibles en la aplicación
/// </summary>
public enum ThemeMode
{
    Light,
    Dark
}

/// <summary>
/// Administrador centralizado de temas para la aplicación WinForms.
/// Proporciona la interfaz para cambiar y aplicar temas globalmente.
/// </summary>
public static class ThemeManager
{
    private static ThemeMode _currentTheme = ThemeMode.Light;
    private static ColorScheme _currentColorScheme = ColorScheme.CreateLightTheme();

    /// <summary>
    /// Evento que se dispara cuando cambia el tema
    /// </summary>
    public static event EventHandler<ThemeChangedEventArgs>? ThemeChanged;

    /// <summary>
    /// Obtiene el esquema de color actual
    /// </summary>
    public static ColorScheme CurrentColorScheme => _currentColorScheme;

    /// <summary>
    /// Obtiene el tema actual
    /// </summary>
    public static ThemeMode CurrentTheme => _currentTheme;

    /// <summary>
    /// Establece el tema de la aplicación y aplica los colores a todos los controles registrados
    /// </summary>
    public static void SetTheme(ThemeMode theme)
    {
        _currentTheme = theme;
        _currentColorScheme = theme == ThemeMode.Light 
            ? ColorScheme.CreateLightTheme() 
            : ColorScheme.CreateDarkTheme();

        // Notificar a todos los suscriptores
        ThemeChanged?.Invoke(null, new ThemeChangedEventArgs(theme, _currentColorScheme));
    }

    /// <summary>
    /// Alterna entre Light y Dark Mode
    /// </summary>
    public static void ToggleTheme()
    {
        SetTheme(_currentTheme == ThemeMode.Light ? ThemeMode.Dark : ThemeMode.Light);
    }

    /// <summary>
    /// Guarda la preferencia de tema en configuración local
    /// </summary>
    public static void SaveThemePreference()
    {
    }

    /// <summary>
    /// Carga la preferencia de tema desde configuración local
    /// </summary>
    public static void LoadThemePreference()
    {
        SetTheme(ThemeMode.Light);
    }
}

/// <summary>
/// Argumentos del evento cuando cambia el tema
/// </summary>
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
