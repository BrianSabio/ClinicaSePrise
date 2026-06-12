namespace SePrise.WinForms.UX;
public static class ThemeApplier
{
    public static void ApplyThemeToForm(Form form)
    {
        ArgumentNullException.ThrowIfNull(form);

        var colors = ThemeManager.CurrentColorScheme;

        // Aplicar tema al formulario
        form.BackColor = colors.BackgroundColor;
        form.ForeColor = colors.TextPrimary;

        // Aplicar a todos los controles recursivamente
        ApplyThemeToControls(form.Controls, colors);
    }
    private static void ApplyThemeToControls(Control.ControlCollection controls, ColorScheme colors)
    {
        foreach (Control control in controls)
        {
            ApplyThemeToControl(control, colors);

            // Recursivamente aplicar a controles contenidos
            if (control.HasChildren)
            {
                ApplyThemeToControls(control.Controls, colors);
            }
        }
    }
    private static void ApplyThemeToControl(Control control, ColorScheme colors)
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
                tb.BorderStyle = BorderStyle.FixedSingle;
                break;

            case ComboBox cb:
                cb.BackColor = colors.SurfaceColor;
                cb.ForeColor = colors.TextPrimary;
                break;

            case Button btn:
                btn.BackColor = colors.PrimaryColor;
                btn.ForeColor = Color.White;
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderColor = colors.PrimaryDark;
                btn.FlatAppearance.MouseDownBackColor = colors.PrimaryDark;
                btn.FlatAppearance.MouseOverBackColor = colors.PrimaryLight;
                break;

            case Panel pnl:
                pnl.BackColor = colors.SurfaceColor;
                pnl.ForeColor = colors.TextPrimary;
                break;

            case GroupBox gb:
                gb.BackColor = colors.SurfaceColor;
                gb.ForeColor = colors.TextPrimary;
                break;

            case DataGridView dgv:
                dgv.BackgroundColor = colors.BackgroundColor;
                dgv.ForeColor = colors.TextPrimary;
                dgv.GridColor = colors.BorderColor;
                dgv.ColumnHeadersDefaultCellStyle.BackColor = colors.PrimaryColor;
                dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dgv.AlternatingRowsDefaultCellStyle.BackColor = colors.SurfaceColor;
                break;

            case MenuStrip ms:
                ms.BackColor = colors.SurfaceColor;
                ms.ForeColor = colors.TextPrimary;
                break;

            case ToolStrip ts:
                ts.BackColor = colors.SurfaceColor;
                ts.ForeColor = colors.TextPrimary;
                break;

            default:
                control.BackColor = colors.BackgroundColor;
                control.ForeColor = colors.TextPrimary;
                break;
        }
    }
    public static Button CreateStyledButton(string text, Color? overrideColor = null)
    {
        var colors = ThemeManager.CurrentColorScheme;
        var btnColor = overrideColor ?? colors.PrimaryColor;

        var btn = new Button
        {
            Text = text,
            BackColor = btnColor,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Height = 40,
            Font = new Font("Segoe UI", 10, FontStyle.Regular),
            Cursor = Cursors.Hand
        };

        btn.FlatAppearance.BorderColor = btnColor;
        btn.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(btnColor, 0.2f);
        btn.FlatAppearance.MouseOverBackColor = ControlPaint.Light(btnColor, 0.2f);

        return btn;
    }
    public static TextBox CreateStyledTextBox(string placeholder = "")
    {
        var colors = ThemeManager.CurrentColorScheme;

        var txt = new TextBox
        {
            BackColor = colors.SurfaceColor,
            ForeColor = colors.TextPrimary,
            BorderStyle = BorderStyle.FixedSingle,
            Height = 32,
            Font = new Font("Segoe UI", 10, FontStyle.Regular),
            Padding = new Padding(8)
        };

        if (!string.IsNullOrEmpty(placeholder))
        {
            txt.Text = placeholder;
            txt.ForeColor = colors.PlaceholderColor;

            txt.GotFocus += (s, e) =>
            {
                if (txt.Text == placeholder)
                {
                    txt.Text = "";
                    txt.ForeColor = colors.TextPrimary;
                }
            };

            txt.LostFocus += (s, e) =>
            {
                if (string.IsNullOrEmpty(txt.Text))
                {
                    txt.Text = placeholder;
                    txt.ForeColor = colors.PlaceholderColor;
                }
            };
        }

        return txt;
    }
    public static ComboBox CreateStyledComboBox()
    {
        var colors = ThemeManager.CurrentColorScheme;

        var cmb = new ComboBox
        {
            BackColor = colors.SurfaceColor,
            ForeColor = colors.TextPrimary,
            Height = 32,
            Font = new Font("Segoe UI", 10, FontStyle.Regular),
            DropDownStyle = ComboBoxStyle.DropDownList
        };

        return cmb;
    }
}


