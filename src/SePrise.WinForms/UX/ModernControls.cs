namespace SePrise.WinForms.UX;
public class ModernPanel : Panel
{
    private Color _borderColor;
    private int _borderRadius = 8;
    private int _borderWidth = 1;

    public ModernPanel()
    {
        _borderColor = ThemeManager.CurrentColorScheme.BorderColor;
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
        SetStyle(ControlStyles.SupportsTransparentBackColor, true);

        ThemeManager.ThemeChanged += (s, e) =>
        {
            _borderColor = e.ColorScheme.BorderColor;
            BackColor = e.ColorScheme.SurfaceColor;
            Invalidate();
        };
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        using (var pen = new Pen(_borderColor, _borderWidth))
        {
            e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
        }
    }
}
public class ModernTextBox : TextBox
{
    private Color _normalColor;
    private Color _errorColor;
    private bool _hasError;

    public bool HasError
    {
        get => _hasError;
        set
        {
            _hasError = value;
            BackColor = value ? _errorColor : _normalColor;
        }
    }

    public ModernTextBox()
    {
        var colors = ThemeManager.CurrentColorScheme;
        _normalColor = colors.SurfaceColor;
        _errorColor = colors.ErrorColor;

        this.BorderStyle = BorderStyle.FixedSingle;
        this.Font = new Font("Segoe UI", 10);
        this.Padding = new Padding(8);

        ThemeManager.ThemeChanged += (s, e) =>
        {
            _normalColor = e.ColorScheme.SurfaceColor;
            _errorColor = e.ColorScheme.ErrorColor;
            BackColor = _hasError ? _errorColor : _normalColor;
            ForeColor = e.ColorScheme.TextPrimary;
        };
    }
}
public class ModernComboBox : ComboBox
{
    public ModernComboBox()
    {
        var colors = ThemeManager.CurrentColorScheme;
        this.DropDownStyle = ComboBoxStyle.DropDownList;
        this.Font = new Font("Segoe UI", 10);
        this.Height = 32;

        ThemeManager.ThemeChanged += (s, e) =>
        {
            BackColor = e.ColorScheme.SurfaceColor;
            ForeColor = e.ColorScheme.TextPrimary;
        };
    }
    public void FilterItems(string searchText)
    {
        if (string.IsNullOrEmpty(searchText))
        {
            return;
        }

        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i].ToString()?.Contains(searchText, StringComparison.OrdinalIgnoreCase) == true)
            {
                SelectedIndex = i;
                return;
            }
        }
    }
}
public class ModernButton : Button
{
    private Color _primaryColor;
    private Color _hoverColor;
    private Color _pressedColor;
    private Color _disabledColor;

    public ModernButton()
    {
        var colors = ThemeManager.CurrentColorScheme;
        _primaryColor = colors.PrimaryColor;
        _hoverColor = colors.PrimaryLight;
        _pressedColor = colors.PrimaryDark;
        _disabledColor = colors.DisabledColor;

        this.BackColor = _primaryColor;
        this.ForeColor = Color.White;
        this.FlatStyle = FlatStyle.Flat;
        this.Font = new Font("Segoe UI", 10, FontStyle.Regular);
        this.Height = 40;
        this.Cursor = Cursors.Hand;

        this.FlatAppearance.BorderColor = _primaryColor;
        this.FlatAppearance.BorderSize = 0;
        this.FlatAppearance.MouseDownBackColor = _pressedColor;
        this.FlatAppearance.MouseOverBackColor = _hoverColor;

        ThemeManager.ThemeChanged += (s, e) =>
        {
            _primaryColor = e.ColorScheme.PrimaryColor;
            _hoverColor = e.ColorScheme.PrimaryLight;
            _pressedColor = e.ColorScheme.PrimaryDark;
            _disabledColor = e.ColorScheme.DisabledColor;

            BackColor = Enabled ? _primaryColor : _disabledColor;
            FlatAppearance.BorderColor = Enabled ? _primaryColor : _disabledColor;
        };
    }

    protected override void OnEnabledChanged(EventArgs e)
    {
        base.OnEnabledChanged(e);
        BackColor = Enabled ? _primaryColor : _disabledColor;
        FlatAppearance.BorderColor = Enabled ? _primaryColor : _disabledColor;
    }
}
public class ErrorLabel : Label
{
    public ErrorLabel()
    {
        var colors = ThemeManager.CurrentColorScheme;
        this.AutoSize = true;
        this.ForeColor = colors.ErrorColor;
        this.Font = new Font("Segoe UI", 9, FontStyle.Regular);
        this.Visible = false;

        ThemeManager.ThemeChanged += (s, e) =>
        {
            ForeColor = e.ColorScheme.ErrorColor;
        };
    }
}
public class InfoLabel : Label
{
    private Color _infoColor;

    public InfoLabel()
    {
        var colors = ThemeManager.CurrentColorScheme;
        _infoColor = colors.InfoColor;
        this.AutoSize = true;
        this.ForeColor = _infoColor;
        this.Font = new Font("Segoe UI", 9, FontStyle.Regular);

        ThemeManager.ThemeChanged += (s, e) =>
        {
            _infoColor = e.ColorScheme.InfoColor;
            ForeColor = _infoColor;
        };
    }
}
public class SuccessLabel : Label
{
    private Color _successColor;

    public SuccessLabel()
    {
        var colors = ThemeManager.CurrentColorScheme;
        _successColor = colors.SuccessColor;
        this.AutoSize = true;
        this.ForeColor = _successColor;
        this.Font = new Font("Segoe UI", 9, FontStyle.Regular);

        ThemeManager.ThemeChanged += (s, e) =>
        {
            _successColor = e.ColorScheme.SuccessColor;
            ForeColor = _successColor;
        };
    }
}


