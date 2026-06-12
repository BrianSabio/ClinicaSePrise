namespace SePrise.WinForms.UX;

/// <summary>
/// Tipos de notificaciones disponibles en el sistema
/// </summary>
public enum NotificationType
{
    Success,
    Error,
    Warning,
    Info
}

/// <summary>
/// Gestor centralizado de notificaciones y mensajes al usuario
/// </summary>
public static class NotificationManager
{
    /// <summary>
    /// Muestra una notificación de éxito
    /// </summary>
    public static void ShowSuccess(string title, string message, Form? owner = null)
    {
        ShowNotification(title, message, NotificationType.Success, owner);
    }

    /// <summary>
    /// Muestra una notificación de error
    /// </summary>
    public static void ShowError(string title, string message, Form? owner = null)
    {
        ShowNotification(title, message, NotificationType.Error, owner);
    }

    /// <summary>
    /// Muestra una notificación de advertencia
    /// </summary>
    public static void ShowWarning(string title, string message, Form? owner = null)
    {
        ShowNotification(title, message, NotificationType.Warning, owner);
    }

    /// <summary>
    /// Muestra una notificación de información
    /// </summary>
    public static void ShowInfo(string title, string message, Form? owner = null)
    {
        ShowNotification(title, message, NotificationType.Info, owner);
    }

    /// <summary>
    /// Muestra un mensaje de confirmación
    /// </summary>
    public static DialogResult ShowConfirm(string title, string message, Form? owner = null)
    {
        var colors = ThemeManager.CurrentColorScheme;
        var result = MessageBox.Show(
            message,
            title,
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question,
            MessageBoxDefaultButton.Button2
        );

        return result;
    }

    private static void ShowNotification(string title, string message, NotificationType type, Form? owner = null)
    {
        var colors = ThemeManager.CurrentColorScheme;
        var icon = type switch
        {
            NotificationType.Success => MessageBoxIcon.Information,
            NotificationType.Error => MessageBoxIcon.Error,
            NotificationType.Warning => MessageBoxIcon.Warning,
            NotificationType.Info => MessageBoxIcon.Information,
            _ => MessageBoxIcon.None
        };

        MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
    }
}

/// <summary>
/// Utilidades de validación con retroalimentación visual
/// </summary>
public static class ValidationHelper
{
    private static readonly Dictionary<Control, Label> ErrorLabels = new();
    private static readonly Dictionary<Control, Color> OriginalColors = new();

    /// <summary>
    /// Valida un campo de texto y muestra retroalimentación visual
    /// </summary>
    public static bool ValidateRequired(TextBox textBox, Label errorLabel, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(textBox.Text))
        {
            ShowValidationError(textBox, errorLabel, $"{fieldName} es obligatorio");
            return false;
        }

        ClearValidationError(textBox, errorLabel);
        return true;
    }

    /// <summary>
    /// Valida un email
    /// </summary>
    public static bool ValidateEmail(TextBox textBox, Label errorLabel)
    {
        if (string.IsNullOrWhiteSpace(textBox.Text))
        {
            ClearValidationError(textBox, errorLabel);
            return true;
        }

        try
        {
            var addr = new System.Net.Mail.MailAddress(textBox.Text);
            ClearValidationError(textBox, errorLabel);
            return true;
        }
        catch
        {
            ShowValidationError(textBox, errorLabel, "Email inválido");
            return false;
        }
    }

    /// <summary>
    /// Valida un DNI (formato básico)
    /// </summary>
    public static bool ValidateDNI(TextBox textBox, Label errorLabel)
    {
        if (string.IsNullOrWhiteSpace(textBox.Text))
        {
            ShowValidationError(textBox, errorLabel, "DNI es obligatorio");
            return false;
        }

        var dni = textBox.Text.Trim();
        if (!System.Text.RegularExpressions.Regex.IsMatch(dni, @"^\d{7,9}$"))
        {
            ShowValidationError(textBox, errorLabel, "DNI debe contener 7-9 dígitos");
            return false;
        }

        ClearValidationError(textBox, errorLabel);
        return true;
    }

    /// <summary>
    /// Valida un número
    /// </summary>
    public static bool ValidateNumber(TextBox textBox, Label errorLabel, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(textBox.Text))
        {
            ClearValidationError(textBox, errorLabel);
            return true;
        }

        if (!int.TryParse(textBox.Text, out _) && !double.TryParse(textBox.Text, out _))
        {
            ShowValidationError(textBox, errorLabel, $"{fieldName} debe ser un número");
            return false;
        }

        ClearValidationError(textBox, errorLabel);
        return true;
    }

    /// <summary>
    /// Valida un rango de números
    /// </summary>
    public static bool ValidateNumberRange(NumericUpDown control, Label errorLabel, decimal min, decimal max)
    {
        if (control.Value < min || control.Value > max)
        {
            ShowValidationError(control, errorLabel, $"Debe estar entre {min} y {max}");
            return false;
        }

        ClearValidationError(control, errorLabel);
        return true;
    }

    /// <summary>
    /// Valida que un ComboBox tenga selección
    /// </summary>
    public static bool ValidateSelection(ComboBox comboBox, Label errorLabel, string fieldName)
    {
        if (comboBox.SelectedIndex < 0)
        {
            ShowValidationError(comboBox, errorLabel, $"Debe seleccionar {fieldName}");
            return false;
        }

        ClearValidationError(comboBox, errorLabel);
        return true;
    }

    /// <summary>
    /// Muestra un error de validación en rojo
    /// </summary>
    private static void ShowValidationError(Control control, Label errorLabel, string message)
    {
        var colors = ThemeManager.CurrentColorScheme;

        if (!OriginalColors.ContainsKey(control))
        {
            OriginalColors[control] = control.BackColor;
        }

        control.BackColor = colors.ErrorColor;
        control.ForeColor = Color.White;

        errorLabel.Text = message;
        errorLabel.ForeColor = colors.ErrorColor;
        errorLabel.Visible = true;
    }

    /// <summary>
    /// Limpia el error de validación
    /// </summary>
    private static void ClearValidationError(Control control, Label errorLabel)
    {
        var colors = ThemeManager.CurrentColorScheme;

        if (OriginalColors.ContainsKey(control))
        {
            control.BackColor = OriginalColors[control];
            control.ForeColor = colors.TextPrimary;
        }

        errorLabel.Text = "";
        errorLabel.Visible = false;
    }
}

/// <summary>
/// Extensiones útiles para controles
/// </summary>
public static class ControlExtensions
{
    /// <summary>
    /// Deshabilita un control con retroalimentación visual
    /// </summary>
    public static void DisableWithFeedback(this Control control)
    {
        var colors = ThemeManager.CurrentColorScheme;
        control.Enabled = false;
        control.BackColor = colors.DisabledColor;
        control.ForeColor = colors.TextDisabled;
    }

    /// <summary>
    /// Habilita un control con retroalimentación visual
    /// </summary>
    public static void EnableWithFeedback(this Control control)
    {
        var colors = ThemeManager.CurrentColorScheme;
        control.Enabled = true;
        control.BackColor = colors.SurfaceColor;
        control.ForeColor = colors.TextPrimary;
    }

    /// <summary>
    /// Añade un tooltip a un control
    /// </summary>
    public static void AddTooltip(this Control control, string tooltip)
    {
        var toolTip = new ToolTip();
        toolTip.SetToolTip(control, tooltip);
        toolTip.AutoPopDelay = 5000;
        toolTip.InitialDelay = 1000;
        toolTip.ReshowDelay = 500;
    }

    /// <summary>
    /// Muestra un indicador de carga/procesamiento
    /// </summary>
    public static void ShowLoading(this Button button)
    {
        var originalText = button.Text;
        button.Text = "Procesando...";
        button.Enabled = false;
        button.Tag = originalText;
    }

    /// <summary>
    /// Oculta el indicador de carga
    /// </summary>
    public static void HideLoading(this Button button)
    {
        if (button.Tag is string originalText)
        {
            button.Text = originalText;
            button.Enabled = true;
        }
    }

    /// <summary>
    /// Limpia todos los controles de entrada en un contenedor
    /// </summary>
    public static void ClearAll(this Control container)
    {
        foreach (Control control in container.Controls)
        {
            switch (control)
            {
                case TextBox tb:
                    tb.Clear();
                    break;
                case ComboBox cb:
                    cb.SelectedIndex = -1;
                    break;
                case NumericUpDown nud:
                    nud.Value = nud.Minimum;
                    break;
                case DateTimePicker dtp:
                    dtp.Value = DateTime.Now;
                    break;
                case CheckBox chk:
                    chk.Checked = false;
                    break;
            }

            if (control.HasChildren)
            {
                control.ClearAll();
            }
        }
    }

    /// <summary>
    /// Aplica enfoque visual a un control
    /// </summary>
    public static void ApplyFocus(this Control control)
    {
        control.Focus();
    }
}
