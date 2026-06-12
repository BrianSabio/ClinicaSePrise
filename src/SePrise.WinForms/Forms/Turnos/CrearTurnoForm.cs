using SePrise.WinForms.Models;
using SePrise.WinForms.Services;
using SePrise.WinForms.UX;

namespace SePrise.WinForms.Forms.Turnos;

public partial class CrearTurnoForm : Form
{
    private readonly TurnoService _turnoService;
    private readonly PacienteService _pacienteService;
    private readonly MedicoService _medicoService;
    private readonly EspecialidadService _especialidadService;
    private readonly SalaService _salaService;

    private ModernComboBox _cmbPaciente = null!;
    private ModernComboBox _cmbEspecialidad = null!;
    private ModernComboBox _cmbMedico = null!;
    private ModernComboBox _cmbSala = null!;
    private DateTimePicker _dtpFecha = null!;
    private DateTimePicker _dtpHora = null!;
    private NumericUpDown _numDuracion = null!;

    private ModernButton _btnGuardar = null!;
    private Button _btnCancelar = null!;
    private ErrorLabel _lblError = null!;
    private Label _lblTitle = null!;
    private Label _lblMedicoInfo = null!;

    private List<MedicoDTO> _todosLosMedicos = new();

    public CrearTurnoForm(TurnoService turnoService, PacienteService pacienteService, MedicoService medicoService, EspecialidadService especialidadService, SalaService salaService)
    {
        _turnoService = turnoService;
        _pacienteService = pacienteService;
        _medicoService = medicoService;
        _especialidadService = especialidadService;
        _salaService = salaService;

        InitializeComponentCustom();
        ApplyTheme();
    }

    private void InitializeComponentCustom()
    {
        this.Text = "Agendar Nuevo Turno";
        this.Size = new Size(550, 650);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Font = new Font("Segoe UI", 10);

        // ========== TITLE ==========
        _lblTitle = new Label
        {
            Text = "Agendar Nuevo Turno Médico",
            Location = new Point(30, 20),
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            AutoSize = true
        };
        this.Controls.Add(_lblTitle);

        int y = 60;
        const int spacing = 60;

        // ========== PACIENTE ==========
        var lblPaciente = new Label { Text = "Paciente:", Location = new Point(30, y), AutoSize = true };
        this.Controls.Add(lblPaciente);
        _cmbPaciente = new ModernComboBox { Location = new Point(30, y + 25), Width = 480 };
        _cmbPaciente.AddTooltip("Seleccione el paciente para este turno");
        this.Controls.Add(_cmbPaciente);
        y += spacing;

        // ========== ESPECIALIDAD ==========
        var lblEspecialidad = new Label { Text = "Especialidad:", Location = new Point(30, y), AutoSize = true };
        this.Controls.Add(lblEspecialidad);
        _cmbEspecialidad = new ModernComboBox { Location = new Point(30, y + 25), Width = 480 };
        _cmbEspecialidad.SelectedIndexChanged += CmbEspecialidad_SelectedIndexChanged;
        _cmbEspecialidad.AddTooltip("Seleccione la especialidad médica");
        this.Controls.Add(_cmbEspecialidad);
        y += spacing;

        // ========== MÉDICO ==========
        var lblMedico = new Label { Text = "Médico:", Location = new Point(30, y), AutoSize = true };
        this.Controls.Add(lblMedico);
        _cmbMedico = new ModernComboBox { Location = new Point(30, y + 25), Width = 480 };
        _cmbMedico.AddTooltip("Seleccione el médico para el turno");
        this.Controls.Add(_cmbMedico);
        _lblMedicoInfo = new InfoLabel { Location = new Point(30, y + 55), AutoSize = true, Text = "" };
        this.Controls.Add(_lblMedicoInfo);
        y += spacing + 30;

        // ========== SALA ==========
        var lblSala = new Label { Text = "Sala/Consultorio:", Location = new Point(30, y), AutoSize = true };
        this.Controls.Add(lblSala);
        _cmbSala = new ModernComboBox { Location = new Point(30, y + 25), Width = 480 };
        _cmbSala.AddTooltip("Seleccione la sala o consultorio");
        this.Controls.Add(_cmbSala);
        y += spacing;

        // ========== FECHA ==========
        var lblFecha = new Label { Text = "Fecha:", Location = new Point(30, y), AutoSize = true };
        this.Controls.Add(lblFecha);
        _dtpFecha = new DateTimePicker
        {
            Location = new Point(30, y + 25),
            Width = 480,
            Format = DateTimePickerFormat.Short,
            MinDate = DateTime.Today
        };
        this.Controls.Add(_dtpFecha);
        y += spacing;

        // ========== HORA ==========
        var lblHora = new Label { Text = "Hora:", Location = new Point(30, y), AutoSize = true };
        this.Controls.Add(lblHora);
        _dtpHora = new DateTimePicker
        {
            Location = new Point(30, y + 25),
            Width = 480,
            Format = DateTimePickerFormat.Time,
            ShowUpDown = true
        };
        this.Controls.Add(_dtpHora);
        y += spacing;

        // ========== DURACIÓN ==========
        var lblDuracion = new Label { Text = "Duración (minutos):", Location = new Point(30, y), AutoSize = true };
        this.Controls.Add(lblDuracion);
        _numDuracion = new NumericUpDown
        {
            Location = new Point(30, y + 25),
            Width = 480,
            Minimum = 5,
            Maximum = 120,
            Value = 30
        };
        this.Controls.Add(_numDuracion);
        y += spacing;

        // ========== BUTTONS ==========
        _btnGuardar = new ModernButton { Text = "Agendar Turno", Location = new Point(120, y), Width = 170 };
        _btnGuardar.Click += BtnGuardar_Click;
        this.Controls.Add(_btnGuardar);

        _btnCancelar = new Button
        {
            Text = "Cancelar",
            Location = new Point(320, y),
            Width = 170,
            Height = 40,
            FlatStyle = FlatStyle.Flat
        };
        _btnCancelar.Click += (s, e) => this.Close();
        this.Controls.Add(_btnCancelar);

        y += 50;

        // ========== ERROR LABEL ==========
        _lblError = new ErrorLabel { Location = new Point(30, y) };
        this.Controls.Add(_lblError);

        // ========== KEYBOARD SHORTCUTS ==========
        this.KeyPreview = true;
        this.KeyDown += (s, e) =>
        {
            if (e.KeyCode == Keys.Escape) this.Close();
            if (e.KeyCode == Keys.Enter && _btnGuardar.Focused) BtnGuardar_Click(null, null!);
        };

        this.Load += CrearTurnoForm_Load;
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

    private async void CrearTurnoForm_Load(object? sender, EventArgs e)
    {
        try
        {
            _btnGuardar.DisableWithFeedback();

            var pacientes = await _pacienteService.ObtenerTodosAsync();
            _cmbPaciente.DisplayMember = "DNI";
            _cmbPaciente.ValueMember = "IdPaciente";
            _cmbPaciente.DataSource = pacientes.Select(p => new { p.IdPaciente, DNI = $"{p.DNI} - {p.Nombre} {p.Apellido}" }).ToList();

            var especialidades = await _especialidadService.ObtenerTodosAsync();
            _cmbEspecialidad.DisplayMember = "Nombre";
            _cmbEspecialidad.ValueMember = "IdEspecialidad";
            _cmbEspecialidad.DataSource = especialidades;

            _todosLosMedicos = await _medicoService.ObtenerTodosAsync();

            var salas = await _salaService.ObtenerTodosAsync();
            _cmbSala.DisplayMember = "Numero";
            _cmbSala.ValueMember = "IdSala";
            _cmbSala.DataSource = salas.Select(s => new { s.IdSala, Numero = $"Sala {s.Numero} ({s.TipoSala})" }).ToList();

            CmbEspecialidad_SelectedIndexChanged(null, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            NotificationManager.ShowError("Error de Carga", "Error al cargar datos iniciales: " + ex.Message, this);
        }
        finally
        {
            _btnGuardar.EnableWithFeedback();
        }
    }

    private void CmbEspecialidad_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_cmbEspecialidad.SelectedItem is EspecialidadDTO esp)
        {
            _numDuracion.Value = esp.DuracionMinutos > 0 ? esp.DuracionMinutos : 30;

            var medicosFiltrados = _todosLosMedicos
                .Where(m => m.Especialidades?.Any(e => e.IdEspecialidad == esp.IdEspecialidad) == true)
                .ToList();

            _cmbMedico.DisplayMember = "NombreCompleto";
            _cmbMedico.ValueMember = "IdMedico";
            _cmbMedico.DataSource = medicosFiltrados.Select(m => new { m.IdMedico, NombreCompleto = $"Dr. {m.Nombre} {m.Apellido}" }).ToList();

            if (medicosFiltrados.Count == 0)
            {
                _lblMedicoInfo.Text = "⚠️ No hay médicos con esta especialidad";
            }
            else
            {
                _lblMedicoInfo.Text = $"✓ {medicosFiltrados.Count} médico(s) disponible(s)";
            }
        }
    }

    private async void BtnGuardar_Click(object? sender, EventArgs e)
    {
        _lblError.Visible = false;

        if (!ValidationHelper.ValidateSelection(_cmbPaciente, _lblError, "Paciente")) return;
        if (!ValidationHelper.ValidateSelection(_cmbEspecialidad, _lblError, "Especialidad")) return;
        if (!ValidationHelper.ValidateSelection(_cmbMedico, _lblError, "Médico")) return;
        if (!ValidationHelper.ValidateSelection(_cmbSala, _lblError, "Sala")) return;

        var fechaSeleccionada = _dtpFecha.Value.Date + _dtpHora.Value.TimeOfDay;
        if (fechaSeleccionada <= DateTime.Now)
        {
            _lblError.Text = "La fecha y hora deben ser futuras";
            _lblError.Visible = true;
            return;
        }

        _btnGuardar.ShowLoading();

        try
        {
            var req = new CreateTurnoRequest
            {
                IdPaciente = (int)_cmbPaciente.SelectedValue!,
                IdEspecialidad = (int)_cmbEspecialidad.SelectedValue!,
                IdMedico = (int)_cmbMedico.SelectedValue!,
                IdSala = (int)_cmbSala.SelectedValue!,
                FechaHoraInicio = fechaSeleccionada,
                DuracionMinutos = (int)_numDuracion.Value
            };

            await _turnoService.CrearAsync(req);

            NotificationManager.ShowSuccess(
                "Turno Agendado",
                $"Turno agendado exitosamente para {_dtpFecha.Value:dd/MM/yyyy} a las {_dtpHora.Value:HH:mm}",
                this
            );

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("409") || ex.Message.Contains("overlap") || ex.Message.Contains("Médico"))
            {
                _lblError.Text = "Médico/Sala ocupada en ese horario. Por favor, seleccione otro horario.";
            }
            else if (ex.Message.Contains("especialidad"))
            {
                _lblError.Text = "El médico seleccionado no tiene esta especialidad asignada.";
            }
            else
            {
                _lblError.Text = $"Error: {ex.Message}";
            }
            _lblError.Visible = true;
        }
        finally
        {
            _btnGuardar.HideLoading();
        }
    }
}
