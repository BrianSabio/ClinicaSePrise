using SePrise.WinForms.Models;
using SePrise.WinForms.Services;
using SePrise.WinForms.UX;

namespace SePrise.WinForms.Forms.Pacientes;

public partial class EditarPacienteForm : Form
{
    private readonly PacienteService _pacienteService;
    private readonly PacienteDTO _pacienteOriginal;

    private ModernTextBox _txtDni = null!;
    private ModernTextBox _txtNombre = null!;
    private ModernTextBox _txtApellido = null!;
    private ModernTextBox _txtEmail = null!;
    private ModernTextBox _txtTelefono = null!;
    private DateTimePicker _dtpFechaNacimiento = null!;
    private ModernButton _btnGuardar = null!;
    private ModernButton _btnCancelar = null!;
    private ErrorLabel _lblError = null!;
    private Label _lblTitle = null!;

    public EditarPacienteForm(PacienteService pacienteService, PacienteDTO paciente)
    {
        _pacienteService = pacienteService;
        _pacienteOriginal = paciente;
        InitializeComponentCustom();
        ApplyTheme();
        CargarDatos();
    }

    private void InitializeComponentCustom()
    {
        this.Text = "Editar Paciente";
        this.Size = new Size(500, 600);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Font = new Font("Segoe UI", 10);
        _lblTitle = new Label
        {
            Text = $"Editar Paciente - {_pacienteOriginal.Nombre}",
            Location = new Point(30, 20),
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            AutoSize = true
        };
        this.Controls.Add(_lblTitle);

        int y = 70;
        const int spacing = 65;
        this.Controls.Add(new Label { Text = "DNI:", Location = new Point(30, y), AutoSize = true, Font = new Font("Segoe UI", 10) });
        _txtDni = new ModernTextBox { Location = new Point(30, y + 25), Width = 440, Height = 35 };
        _txtDni.ReadOnly = true;
        _txtDni.Enabled = false;
        this.Controls.Add(_txtDni);
        y += spacing;
        this.Controls.Add(new Label { Text = "Nombre:", Location = new Point(30, y), AutoSize = true, Font = new Font("Segoe UI", 10) });
        _txtNombre = new ModernTextBox { Location = new Point(30, y + 25), Width = 440, Height = 35 };
        _txtNombre.AddTooltip("Ingrese el nombre del paciente");
        this.Controls.Add(_txtNombre);
        y += spacing;
        this.Controls.Add(new Label { Text = "Apellido:", Location = new Point(30, y), AutoSize = true, Font = new Font("Segoe UI", 10) });
        _txtApellido = new ModernTextBox { Location = new Point(30, y + 25), Width = 440, Height = 35 };
        _txtApellido.AddTooltip("Ingrese el apellido del paciente");
        this.Controls.Add(_txtApellido);
        y += spacing;
        this.Controls.Add(new Label { Text = "Email:", Location = new Point(30, y), AutoSize = true, Font = new Font("Segoe UI", 10) });
        _txtEmail = new ModernTextBox { Location = new Point(30, y + 25), Width = 440, Height = 35 };
        _txtEmail.AddTooltip("Ingrese el correo electrónico");
        this.Controls.Add(_txtEmail);
        y += spacing;
        this.Controls.Add(new Label { Text = "Teléfono:", Location = new Point(30, y), AutoSize = true, Font = new Font("Segoe UI", 10) });
        _txtTelefono = new ModernTextBox { Location = new Point(30, y + 25), Width = 440, Height = 35 };
        _txtTelefono.AddTooltip("Ingrese el número de teléfono");
        this.Controls.Add(_txtTelefono);
        y += spacing;
        this.Controls.Add(new Label { Text = "Fecha de Nacimiento:", Location = new Point(30, y), AutoSize = true, Font = new Font("Segoe UI", 10) });
        _dtpFechaNacimiento = new DateTimePicker 
        { 
            Location = new Point(30, y + 25), 
            Width = 440, 
            Height = 35,
            Format = DateTimePickerFormat.Short
        };
        this.Controls.Add(_dtpFechaNacimiento);
        y += spacing;
        _lblError = new ErrorLabel
        {
            Location = new Point(30, y),
            Width = 440,
            AutoSize = false,
            Visible = false
        };
        this.Controls.Add(_lblError);
        y += 50;
        var pnlBotones = new Panel
        {
            Location = new Point(30, y),
            Size = new Size(440, 50)
        };

        _btnGuardar = new ModernButton 
        { 
            Text = "💾 Guardar", 
            Location = new Point(0, 0), 
            Width = 210,
            Height = 40
        };
        _btnGuardar.Click += BtnGuardar_Click;
        _btnGuardar.AddTooltip("Guardar cambios");
        pnlBotones.Controls.Add(_btnGuardar);

        _btnCancelar = new ModernButton 
        { 
            Text = "❌ Cancelar", 
            Location = new Point(220, 0), 
            Width = 210,
            Height = 40
        };
        _btnCancelar.Click += (s, e) => this.Close();
        _btnCancelar.AddTooltip("Cancelar edición");
        pnlBotones.Controls.Add(_btnCancelar);

        this.Controls.Add(pnlBotones);
        ThemeManager.ThemeChanged += (s, e) => ApplyTheme();
    }

    private void ApplyTheme()
    {
        var colors = ThemeManager.CurrentColorScheme;
        this.BackColor = colors.BackgroundColor;
        this.ForeColor = colors.TextPrimary;
        ThemeApplier.ApplyThemeToForm(this);
    }

    private void CargarDatos()
    {
        _txtDni.Text = _pacienteOriginal.DNI;
        _txtNombre.Text = _pacienteOriginal.Nombre;
        _txtApellido.Text = _pacienteOriginal.Apellido;
        _txtEmail.Text = _pacienteOriginal.Email;
        _txtTelefono.Text = _pacienteOriginal.Telefono;
        _dtpFechaNacimiento.Value = _pacienteOriginal.FechaNacimiento;
    }

    private async void BtnGuardar_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_txtDni.Text) || 
            string.IsNullOrWhiteSpace(_txtNombre.Text) || 
            string.IsNullOrWhiteSpace(_txtApellido.Text))
        {
            MostrarError("DNI, Nombre y Apellido son obligatorios.");
            return;
        }

        _lblError.Visible = false;
        DeshabilitarBotones();

        try
        {
            var request = new UpdatePacienteRequest
            {
                DNI = _txtDni.Text.Trim(),
                Nombre = _txtNombre.Text.Trim(),
                Apellido = _txtApellido.Text.Trim(),
                Email = _txtEmail.Text.Trim(),
                Telefono = _txtTelefono.Text.Trim(),
                FechaNacimiento = _dtpFechaNacimiento.Value,
                Genero = "O" // Por defecto (Otro) hasta que se implemente el combo de Género en la UI
            };

            await _pacienteService.ActualizarAsync(_pacienteOriginal.IdPaciente, request);
            MessageBox.Show("Paciente actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        catch (Exception ex)
        {
            MostrarError(ex.Message);
        }
        finally
        {
            HabilitarBotones();
        }
    }

    private void MostrarError(string mensaje)
    {
        _lblError.Text = "⚠️ Ocurrió un error (ver detalle en la ventana emergente)";
        _lblError.Visible = true;
        NotificationManager.ShowError("Error de validación", mensaje, this);
    }

    private void DeshabilitarBotones()
    {
        _btnGuardar.Enabled = false;
        _btnCancelar.Enabled = false;
    }

    private void HabilitarBotones()
    {
        _btnGuardar.Enabled = true;
        _btnCancelar.Enabled = true;
    }
}


