using SePrise.WinForms.Models;
using SePrise.WinForms.Services;
using SePrise.WinForms.UX;

namespace SePrise.WinForms.Forms.Pacientes;

public partial class CrearPacienteForm : Form
{
    private readonly PacienteService _pacienteService;

    private ModernTextBox _txtDni = null!;
    private ModernTextBox _txtNombre = null!;
    private ModernTextBox _txtApellido = null!;
    private ModernTextBox _txtEmail = null!;
    private ModernTextBox _txtTelefono = null!;
    private DateTimePicker _dtpFechaNacimiento = null!;
    private ModernButton _btnGuardar = null!;
    private Button _btnCancelar = null!;
    private ErrorLabel _lblError = null!;
    private Label _lblTitle = null!;

    public CrearPacienteForm(PacienteService pacienteService)
    {
        _pacienteService = pacienteService;
        InitializeComponentCustom();
        ApplyTheme();
    }

    private void InitializeComponentCustom()
    {
        this.Text = "Crear Nuevo Paciente";
        this.Size = new Size(500, 550);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Font = new Font("Segoe UI", 10);
        _lblTitle = new Label
        {
            Text = "Crear Nuevo Paciente",
            Location = new Point(30, 20),
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            AutoSize = true
        };
        this.Controls.Add(_lblTitle);

        int y = 60;
        const int spacing = 55;
        var lblDni = new Label { Text = "DNI:", Location = new Point(30, y), AutoSize = true };
        this.Controls.Add(lblDni);
        _txtDni = new ModernTextBox { Location = new Point(30, y + 25), Width = 440 };
        _txtDni.AddTooltip("Número de documento (7-9 dígitos)");
        this.Controls.Add(_txtDni);
        y += spacing;
        var lblNombre = new Label { Text = "Nombre:", Location = new Point(30, y), AutoSize = true };
        this.Controls.Add(lblNombre);
        _txtNombre = new ModernTextBox { Location = new Point(30, y + 25), Width = 440 };
        _txtNombre.AddTooltip("Nombre del paciente");
        this.Controls.Add(_txtNombre);
        y += spacing;
        var lblApellido = new Label { Text = "Apellido:", Location = new Point(30, y), AutoSize = true };
        this.Controls.Add(lblApellido);
        _txtApellido = new ModernTextBox { Location = new Point(30, y + 25), Width = 440 };
        _txtApellido.AddTooltip("Apellido del paciente");
        this.Controls.Add(_txtApellido);
        y += spacing;
        var lblEmail = new Label { Text = "Email:", Location = new Point(30, y), AutoSize = true };
        this.Controls.Add(lblEmail);
        _txtEmail = new ModernTextBox { Location = new Point(30, y + 25), Width = 440 };
        _txtEmail.AddTooltip("Correo electrónico");
        this.Controls.Add(_txtEmail);
        y += spacing;
        var lblTelefono = new Label { Text = "Teléfono (Opcional):", Location = new Point(30, y), AutoSize = true };
        this.Controls.Add(lblTelefono);
        _txtTelefono = new ModernTextBox { Location = new Point(30, y + 25), Width = 440 };
        _txtTelefono.AddTooltip("Número de teléfono");
        this.Controls.Add(_txtTelefono);
        y += spacing;
        var lblFecha = new Label { Text = "Fecha de Nacimiento:", Location = new Point(30, y), AutoSize = true };
        this.Controls.Add(lblFecha);
        _dtpFechaNacimiento = new DateTimePicker
        {
            Location = new Point(30, y + 25),
            Width = 440,
            Format = DateTimePickerFormat.Short,
            Value = DateTime.Now.AddYears(-30)
        };
        this.Controls.Add(_dtpFechaNacimiento);
        y += spacing + 10;
        _btnGuardar = new ModernButton { Text = "Guardar", Location = new Point(150, y), Width = 140 };
        _btnGuardar.Click += BtnGuardar_Click;
        this.Controls.Add(_btnGuardar);

        _btnCancelar = new Button
        {
            Text = "Cancelar",
            Location = new Point(320, y),
            Width = 140,
            Height = 40,
            FlatStyle = FlatStyle.Flat
        };
        _btnCancelar.Click += (s, e) => this.Close();
        this.Controls.Add(_btnCancelar);

        y += 50;
        _lblError = new ErrorLabel { Location = new Point(30, y) };
        this.Controls.Add(_lblError);
        this.KeyPreview = true;
        this.KeyDown += (s, e) =>
        {
            if (e.KeyCode == Keys.Escape) this.Close();
            if (e.KeyCode == Keys.Enter && _btnGuardar.Focused) BtnGuardar_Click(null, null!);
        };

        ThemeManager.ThemeChanged += (s, e) => ApplyTheme();
    }

    private void ApplyTheme()
    {
        var colors = ThemeManager.CurrentColorScheme;
        this.BackColor = colors.BackgroundColor;
        this.ForeColor = colors.TextPrimary;
        _lblTitle.ForeColor = colors.TextPrimary;
        ThemeApplier.ApplyThemeToForm(this);
    }

    private async void BtnGuardar_Click(object? sender, EventArgs e)
    {
        // Validaciones
        if (!ValidationHelper.ValidateDNI(_txtDni, _lblError)) return;
        if (!ValidationHelper.ValidateRequired(_txtNombre, _lblError, "Nombre")) return;
        if (!ValidationHelper.ValidateRequired(_txtApellido, _lblError, "Apellido")) return;
        if (!ValidationHelper.ValidateRequired(_txtEmail, _lblError, "Email")) return;
        if (!ValidationHelper.ValidateEmail(_txtEmail, _lblError)) return;

        _lblError.Visible = false;
        _btnGuardar.ShowLoading();

        try
        {
            var request = new CreatePacienteRequest
            {
                DNI = _txtDni.Text.Trim(),
                Nombre = _txtNombre.Text.Trim(),
                Apellido = _txtApellido.Text.Trim(),
                Email = _txtEmail.Text.Trim(),
                Telefono = _txtTelefono.Text.Trim(),
                FechaNacimiento = _dtpFechaNacimiento.Value,
                Genero = "O" // Por defecto (Otro) hasta que se implemente el combo de Género en la UI
            };

            await _pacienteService.CrearAsync(request);

            NotificationManager.ShowSuccess(
                "Éxito",
                "Paciente creado correctamente.",
                this
            );

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        catch (Exception ex)
        {
            _lblError.Text = "⚠️ Ocurrió un error (ver detalle en la ventana emergente)";
            _lblError.Visible = true;
            NotificationManager.ShowError("Error al guardar", ex.Message, this);
        }
        finally
        {
            _btnGuardar.HideLoading();
        }
    }
}


