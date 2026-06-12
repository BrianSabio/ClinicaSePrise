namespace SePrise.WinForms.UX;
public enum AccessibilityLevel
{
    Normal,
    HighContrast,
    LargeText,
    HighContrastLargeText
}
public static class AccessibilityManager
{
    private static AccessibilityLevel _currentLevel = AccessibilityLevel.Normal;
    public static event EventHandler<AccessibilityChangedEventArgs>? AccessibilityChanged;
public static AccessibilityLevel CurrentLevel => _currentLevel;
public static void SetAccessibilityLevel(AccessibilityLevel level)
    {
        _currentLevel = level;
        SaveAccessibilityPreference();
        AccessibilityChanged?.Invoke(null, new AccessibilityChangedEventArgs(level));
    }
public static float GetTextScaleFactor()
    {
        return _currentLevel switch
        {
            AccessibilityLevel.LargeText => 1.25f,
            AccessibilityLevel.HighContrastLargeText => 1.25f,
            _ => 1.0f
        };
    }
public static bool IsHighContrastEnabled()
    {
        return _currentLevel is AccessibilityLevel.HighContrast or AccessibilityLevel.HighContrastLargeText;
    }
    public static void SaveAccessibilityPreference()
    {
    }
    public static void LoadAccessibilityPreference()
    {
        SetAccessibilityLevel(AccessibilityLevel.Normal);
    }
}
public class AccessibilityChangedEventArgs : EventArgs
{
    public AccessibilityLevel Level { get; }

    public AccessibilityChangedEventArgs(AccessibilityLevel level)
    {
        Level = level;
    }
}
public static class AccessibilityHelper
{
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
public static double GetColorContrast(Color color1, Color color2)
    {
        var luminance1 = GetRelativeLuminance(color1);
        var luminance2 = GetRelativeLuminance(color2);

        var lighter = Math.Max(luminance1, luminance2);
        var darker = Math.Min(luminance1, luminance2);

        return (lighter + 0.05) / (darker + 0.05);
    }
    private static double GetRelativeLuminance(Color color)
    {
        var r = Linearize(color.R / 255.0);
        var g = Linearize(color.G / 255.0);
        var b = Linearize(color.B / 255.0);

        return 0.2126 * r + 0.7152 * g + 0.0722 * b;
    }
    private static double Linearize(double value)
    {
        return value <= 0.03928
            ? value / 12.92
            : Math.Pow((value + 0.055) / 1.055, 2.4);
    }
    public static bool HasSufficientContrast(Color foreground, Color background)
    {
        return GetColorContrast(foreground, background) >= 4.5;
    }
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
    public static void AddAccessibleDescription(Control control, string description)
    {
        control.AccessibleName = control.Text ?? control.Name;
        control.AccessibleDescription = description;
        control.AccessibleRole = AccessibleRole.PushButton;
    }
}
public static class AccessibleMessageBox
{
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


