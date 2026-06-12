namespace SePrise.WinForms.UX;

/// <summary>
/// Niveles de accesibilidad soportados por la aplicación.
/// </summary>
public enum AccessibilityLevel
{
    Normal,
    HighContrast,
    LargeText,
    HighContrastLargeText
}

/// <summary>
/// Gestor centralizado de opciones de accesibilidad.
/// </summary>
public static class AccessibilityManager
{
    private static AccessibilityLevel _currentLevel = AccessibilityLevel.Normal;

    /// <summary>
    /// Evento que se dispara cuando cambian las configuraciones de accesibilidad.
    /// </summary>
    public static event EventHandler<AccessibilityChangedEventArgs>? AccessibilityChanged;

    /// <summary>
    /// Obtiene el nivel de accesibilidad actual.
    /// </summary>
    public static AccessibilityLevel CurrentLevel => _currentLevel;

    /// <summary>
    /// Establece el nivel de accesibilidad.
    /// </summary>
    public static void SetAccessibilityLevel(AccessibilityLevel level)
    {
        _currentLevel = level;
        SaveAccessibilityPreference();
        AccessibilityChanged?.Invoke(null, new AccessibilityChangedEventArgs(level));
    }

    /// <summary>
    /// Obtiene el factor de escala para texto.
    /// </summary>
    public static float GetTextScaleFactor()
    {
        return _currentLevel switch
        {
            AccessibilityLevel.LargeText => 1.25f,
            AccessibilityLevel.HighContrastLargeText => 1.25f,
            _ => 1.0f
        };
    }

    /// <summary>
    /// Obtiene si está activado el contraste alto.
    /// </summary>
    public static bool IsHighContrastEnabled()
    {
        return _currentLevel is AccessibilityLevel.HighContrast or AccessibilityLevel.HighContrastLargeText;
    }

    /// <summary>
    /// Guarda la preferencia de accesibilidad.
    /// </summary>
    public static void SaveAccessibilityPreference()
    {
    }

    /// <summary>
    /// Carga la preferencia de accesibilidad desde configuración local.
    /// </summary>
    public static void LoadAccessibilityPreference()
    {
        SetAccessibilityLevel(AccessibilityLevel.Normal);
    }
}

/// <summary>
/// Argumentos del evento de cambio de accesibilidad.
/// </summary>
public class AccessibilityChangedEventArgs : EventArgs
{
    public AccessibilityLevel Level { get; }

    public AccessibilityChangedEventArgs(AccessibilityLevel level)
    {
        Level = level;
    }
}

/// <summary>
/// Utilidades de accesibilidad para aplicar estilos accesibles.
/// </summary>
public static class AccessibilityHelper
{
    /// <summary>
    /// Aplica estilos de accesibilidad a un formulario completo.
    /// </summary>
    public static void ApplyAccessibilityToForm(Form form)
    {
        ArgumentNullException.ThrowIfNull(form);

        var scaleFactor = AccessibilityManager.GetTextScaleFactor();
        var isHighContrast = AccessibilityManager.IsHighContrastEnabled();

        form.Font = new Font("Segoe UI", form.Font.Size * scaleFactor);

        if (isHighContrast)
        {
            ApplyHighContrastToControls(form.Controls);
        }
    }

    /// <summary>
    /// Aplica estilos de alto contraste recursivamente a controles.
    /// </summary>
    private static void ApplyHighContrastToControls(Control.ControlCollection controls)
    {
        var colors = ThemeManager.CurrentColorScheme;

        foreach (Control control in controls)
        {
            switch (control)
            {
                case Label lbl:
                    lbl.ForeColor = colors.TextPrimary;
                    lbl.BackColor = colors.BackgroundColor;
                    break;

                case TextBox tb:
                    tb.BackColor = colors.SurfaceColor;
                    tb.ForeColor = colors.TextPrimary;
                    tb.BorderStyle = BorderStyle.Fixed3D;
                    break;

                case ComboBox cb:
                    cb.BackColor = colors.SurfaceColor;
                    cb.ForeColor = colors.TextPrimary;
                    break;

                case Button btn:
                    btn.BackColor = colors.PrimaryColor;
                    btn.ForeColor = Color.White;
                    btn.Font = new Font(btn.Font.Name, btn.Font.Size * 1.1f, FontStyle.Bold);
                    break;
            }

            if (control.HasChildren)
            {
                ApplyHighContrastToControls(control.Controls);
            }
        }
    }

    /// <summary>
    /// Obtiene el contraste de color entre dos colores (escala 0-21).
    /// </summary>
    public static double GetColorContrast(Color color1, Color color2)
    {
        var luminance1 = GetRelativeLuminance(color1);
        var luminance2 = GetRelativeLuminance(color2);

        var lighter = Math.Max(luminance1, luminance2);
        var darker = Math.Min(luminance1, luminance2);

        return (lighter + 0.05) / (darker + 0.05);
    }

    /// <summary>
    /// Calcula la luminancia relativa de un color según WCAG.
    /// </summary>
    private static double GetRelativeLuminance(Color color)
    {
        var r = Linearize(color.R / 255.0);
        var g = Linearize(color.G / 255.0);
        var b = Linearize(color.B / 255.0);

        return 0.2126 * r + 0.7152 * g + 0.0722 * b;
    }

    /// <summary>
    /// Lineariza un valor RGB.
    /// </summary>
    private static double Linearize(double value)
    {
        return value <= 0.03928
            ? value / 12.92
            : Math.Pow((value + 0.055) / 1.055, 2.4);
    }

    /// <summary>
    /// Verifica si dos colores tienen suficiente contraste (WCAG AA).
    /// </summary>
    public static bool HasSufficientContrast(Color foreground, Color background)
    {
        return GetColorContrast(foreground, background) >= 4.5;
    }

    /// <summary>
    /// Obtiene un color de contraste mejorado si es necesario.
    /// </summary>
    public static Color GetAccessibleColor(Color suggested, Color background)
    {
        if (HasSufficientContrast(suggested, background))
        {
            return suggested;
        }

        // Ajustar el color para mejorar contraste
        var luminance = GetRelativeLuminance(background);
        return luminance > 0.5 ? Color.Black : Color.White;
    }

    /// <summary>
    /// Agrega un label de descripción accesible a un control.
    /// </summary>
    public static void AddAccessibleDescription(Control control, string description)
    {
        control.AccessibleName = control.Text ?? control.Name;
        control.AccessibleDescription = description;
        control.AccessibleRole = AccessibleRole.PushButton;
    }
}

/// <summary>
/// Filtro de accesibilidad para diálogos y alertas.
/// </summary>
public static class AccessibleMessageBox
{
    /// <summary>
    /// Muestra un diálogo accesible.
    /// </summary>
    public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
    {
        var scaleFactor = AccessibilityManager.GetTextScaleFactor();

        var form = new Form
        {
            Text = caption,
            Width = (int)(400 * scaleFactor),
            Height = (int)(200 * scaleFactor),
            StartPosition = FormStartPosition.CenterParent,
            MaximizeBox = false,
            MinimizeBox = false,
            Font = new Font("Segoe UI", 10 * scaleFactor)
        };

        var lblText = new Label
        {
            Text = text,
            Dock = DockStyle.Fill,
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Padding = new Padding(20),
            Font = new Font("Segoe UI", 11 * scaleFactor)
        };
        form.Controls.Add(lblText);

        var result = form.ShowDialog();
        form.Dispose();
        return result;
    }
}
