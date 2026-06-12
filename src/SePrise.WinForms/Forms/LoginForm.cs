using SePrise.WinForms.Services;
using SePrise.WinForms.UX;

namespace SePrise.WinForms.Forms;

public partial class LoginForm : Form
{
    private readonly AuthService _authService;

    private Label _lblTitulo = null!;
    private Label _lblSubtitulo = null!;
    private Label _lblDni = null!;
    private ModernTextBox _txtDni = null!;
    private ModernButton _btnIngresar = null!;
    private ErrorLabel _lblError = null!;
    private LoadingIndicator _loadingIndicator = null!;

    public LoginForm(AuthService authService)
    {
        _authService = authService;
        InitializeComponentCustom();
        ApplyTheme();
    }

    private void InitializeComponentCustom()
    {
        this.Text = "Clínica SePrise - Autenticación";
        this.Size = new Size(450, 500);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Font = new Font("Segoe UI", 10);
        var pnlTitle = new Panel
        {
            Dock = DockStyle.Top,
            Height = 120,
            Padding = new Padding(20)
        };

        _lblTitulo = new Label
        {
            Text = "CLÍNICA SEPRISE",
            Font = new Font("Segoe UI", 20, FontStyle.Bold),
            Location = new Point(20, 20),
            AutoSize = true
        };
        pnlTitle.Controls.Add(_lblTitulo);

        _lblSubtitulo = new Label
        {
            Text = "Sistema de Gestión Clínica",
            Font = new Font("Segoe UI", 11, FontStyle.Regular),
            Location = new Point(25, 65),
            AutoSize = true
        };
        pnlTitle.Controls.Add(_lblSubtitulo);
        var pnlContent = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(30, 20, 30, 30)
        };

        int y = 30;

        // DNI Label
        _lblDni = new Label
        {
            Text = "Ingrese su DNI",
            Font = new Font("Segoe UI", 11, FontStyle.Regular),
            Location = new Point(30, y),
            AutoSize = true
        };
        pnlContent.Controls.Add(_lblDni);
        y += 35;

        // DNI TextBox
        _txtDni = new ModernTextBox
        {
            Location = new Point(30, y),
            Width = 370,
            Height = 40
        };
        _txtDni.AddTooltip("Ingrese su número de DNI para autenticarse");
        _txtDni.KeyPress += (s, e) =>
        {
            if (e.KeyChar == '\r')
            {
                e.Handled = true;
                BtnIngresar_Click(null, EventArgs.Empty);
            }
        };
        pnlContent.Controls.Add(_txtDni);
        y += 55;

        // Error Label
        _lblError = new ErrorLabel
        {
            Location = new Point(30, y),
            Width = 370,
            AutoSize = false,
            Visible = false
        };
        pnlContent.Controls.Add(_lblError);
        y += 40;

        // Loading Indicator
        _loadingIndicator = new LoadingIndicator
        {
            Location = new Point(165, y),
            Size = new Size(100, 30),
            Visible = false
        };
        pnlContent.Controls.Add(_loadingIndicator);

        // Ingresar Button
        _btnIngresar = new ModernButton
        {
            Text = "🔐 Ingresar",
            Location = new Point(30, y + 45),
            Width = 370,
            Height = 45,
            Font = new Font("Segoe UI", 11, FontStyle.Bold)
        };
        _btnIngresar.Click += BtnIngresar_Click;
        _btnIngresar.AddTooltip("Pulse para iniciar sesión (también Enter)");
        pnlContent.Controls.Add(_btnIngresar);

        this.Controls.Add(pnlContent);
        this.Controls.Add(pnlTitle);
        ThemeManager.ThemeChanged += (s, e) => ApplyTheme();
    }

    private void ApplyTheme()
    {
        var colors = ThemeManager.CurrentColorScheme;
        this.BackColor = colors.BackgroundColor;
        this.ForeColor = colors.TextPrimary;
        ThemeApplier.ApplyThemeToForm(this);
    }

    private async void BtnIngresar_Click(object? sender, EventArgs e)
    {
        string dni = _txtDni.Text.Trim();

        // Validación
        if (!ValidationHelper.ValidateRequired(_txtDni, _lblError, "DNI"))
            return;

        if (!ValidationHelper.ValidateDNI(_txtDni, _lblError))
            return;

        // Limpiar error previo
        _lblError.Visible = false;
        _btnIngresar.Enabled = false;
        _loadingIndicator.Visible = true;

        try
        {
            bool isValid = await _authService.LoginAsync(dni);

            if (isValid)
            {
                NotificationManager.ShowSuccess("✅ Éxito", "Autenticación exitosa. Bienvenido.", this);

                var mainForm = new MainForm(_authService);
                this.Hide();
                mainForm.ShowDialog();
                this.Close();
            }
            else
            {
                NotificationManager.ShowError("❌ Error", "DNI no encontrado o error de conexión.", this);
            }
        }
        catch (Exception ex)
        {
            NotificationManager.ShowError("⚠️ Error Inesperado", $"Ocurrió un error: {ex.Message}", this);
        }
        finally
        {
            _btnIngresar.Enabled = true;
            _loadingIndicator.Visible = false;
        }
    }
}



