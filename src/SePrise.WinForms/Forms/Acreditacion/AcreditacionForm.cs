using SePrise.WinForms.Models;
using SePrise.WinForms.Services;
using SePrise.WinForms.UX;

namespace SePrise.WinForms.Forms.Acreditacion;

public partial class AcreditacionForm : Form
{
    private readonly AtencionService _atencionService;
    private readonly TurnoService _turnoService;
    private readonly PacienteService _pacienteService;
    private readonly EspecialidadService _especialidadService;
    private readonly MedicoService _medicoService;

    private RadioButton _rbPorTurno = null!;
    private RadioButton _rbEspontanea = null!;

    private ModernPanel _pnlPorTurno = null!;
    private ModernComboBox _cmbTurno = null!;
    private Label _lblTurnoInfo = null!;

    private ModernPanel _pnlEspontanea = null!;
    private ModernComboBox _cmbPaciente = null!;
    private ModernComboBox _cmbEspecialidad = null!;
    private ModernComboBox _cmbMedico = null!;

    private ModernComboBox _cmbModalidadPago = null!;
    private ModernButton _btnAcreditar = null!;
    private Button _btnLimpiar = null!;
    private ErrorLabel _lblMensaje = null!;
    private Label _lblTitle = null!;

    private List<MedicoDTO> _todosLosMedicos = new();

    public AcreditacionForm(AtencionService atencionService, TurnoService turnoService, PacienteService pacienteService, EspecialidadService especialidadService, MedicoService medicoService)
    {
        _atencionService = atencionService;
        _turnoService = turnoService;
        _pacienteService = pacienteService;
        _especialidadService = especialidadService;
        _medicoService = medicoService;

        InitializeComponentCustom();
        ApplyTheme();
    }

    private void InitializeComponentCustom()
    {
        this.Text = "Acreditación de Paciente - Recepción";
        this.Size = new Size(600, 680);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Font = new Font("Segoe UI", 10);

        // ========== TITLE ==========
        _lblTitle = new Label
        {
            Text = "Acreditación de Paciente",
            Location = new Point(30, 20),
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            AutoSize = true
        };
        this.Controls.Add(_lblTitle);

        int y = 60;

        // ========== RADIO BUTTONS - TIPO DE ACREDITACION ==========
        var lblTipo = new Label { Text = "Tipo de Acreditación:", Location = new Point(30, y), Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true };
        this.Controls.Add(lblTipo);
        y += 35;

        _rbPorTurno = new RadioButton { Text = "✓ Por Turno Previo", Location = new Point(30, y), Width = 180, Checked = true, Font = new Font("Segoe UI", 10) };
        _rbPorTurno.CheckedChanged += TipoAcreditacion_Changed;
        _rbPorTurno.AddTooltip("Acreditar paciente que tiene turno agendado");
        this.Controls.Add(_rbPorTurno);

        _rbEspontanea = new RadioButton { Text = "★ Demanda Espontánea", Location = new Point(250, y), Width = 200, Font = new Font("Segoe UI", 10) };
        _rbEspontanea.CheckedChanged += TipoAcreditacion_Changed;
        _rbEspontanea.AddTooltip("Acreditar paciente sin turno previo");
        this.Controls.Add(_rbEspontanea);

        y += 50;

        // ========== PANEL POR TURNO ==========
        _pnlPorTurno = new ModernPanel { Location = new Point(30, y), Size = new Size(540, 160), AutoSize = false };

        var lblTurnoId = new Label { Text = "Turno:", Location = new Point(15, 20), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
        _pnlPorTurno.Controls.Add(lblTurnoId);

        _cmbTurno = new ModernComboBox { Location = new Point(15, 45), Width = 510 };
        _cmbTurno.SelectedIndexChanged += CmbTurno_SelectedIndexChanged;
        _cmbTurno.AddTooltip("Seleccione el turno del paciente");
        _pnlPorTurno.Controls.Add(_cmbTurno);

        _lblTurnoInfo = new InfoLabel 
        { 
            Location = new Point(15, 80), 
            Size = new Size(510, 70), 
            Text = "Seleccione un turno para ver los detalles...",
            AutoSize = false
        };
        _pnlPorTurno.Controls.Add(_lblTurnoInfo);

        this.Controls.Add(_pnlPorTurno);

        // ========== PANEL ESPONTANEA ==========
        _pnlEspontanea = new ModernPanel { Location = new Point(30, y), Size = new Size(540, 220), AutoSize = false, Visible = false };

        var lblPaciente = new Label { Text = "Paciente:", Location = new Point(15, 20), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
        _pnlEspontanea.Controls.Add(lblPaciente);
        _cmbPaciente = new ModernComboBox { Location = new Point(15, 45), Width = 510 };
        _pnlEspontanea.Controls.Add(_cmbPaciente);

        var lblEspec = new Label { Text = "Especialidad:", Location = new Point(15, 80), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
        _pnlEspontanea.Controls.Add(lblEspec);
        _cmbEspecialidad = new ModernComboBox { Location = new Point(15, 105), Width = 510 };
        _cmbEspecialidad.SelectedIndexChanged += CmbEspecialidad_SelectedIndexChanged;
        _pnlEspontanea.Controls.Add(_cmbEspecialidad);

        var lblMedico = new Label { Text = "Médico:", Location = new Point(15, 140), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
        _pnlEspontanea.Controls.Add(lblMedico);
        _cmbMedico = new ModernComboBox { Location = new Point(15, 165), Width = 510 };
        _pnlEspontanea.Controls.Add(_cmbMedico);

        this.Controls.Add(_pnlEspontanea);

        y += 240;

        // ========== MODALIDAD PAGO ==========
        var lblModalidad = new Label { Text = "Modalidad de Pago:", Location = new Point(30, y), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
        this.Controls.Add(lblModalidad);

        _cmbModalidadPago = new ModernComboBox { Location = new Point(30, y + 30), Width = 540 };
        _cmbModalidadPago.Items.AddRange(new[] { "Obra Social", "Particular" });
        _cmbModalidadPago.SelectedIndex = 0;
        _cmbModalidadPago.AddTooltip("Seleccione la modalidad de pago");
        this.Controls.Add(_cmbModalidadPago);

        y += 70;

        // ========== BUTTONS ==========
        _btnAcreditar = new ModernButton { Text = "Acreditar Paciente", Location = new Point(150, y), Width = 150 };
        _btnAcreditar.Click += BtnAcreditar_Click;
        this.Controls.Add(_btnAcreditar);

        _btnLimpiar = new Button
        {
            Text = "Limpiar",
            Location = new Point(330, y),
            Width = 150,
            Height = 40,
            FlatStyle = FlatStyle.Flat
        };
        _btnLimpiar.Click += (s, e) => LimpiarFormulario();
        this.Controls.Add(_btnLimpiar);

        y += 50;

        // ========== ERROR/SUCCESS MESSAGE ==========
        _lblMensaje = new ErrorLabel { Location = new Point(30, y) };
        this.Controls.Add(_lblMensaje);

        // ========== KEYBOARD SHORTCUTS ==========
        this.KeyPreview = true;
        this.KeyDown += (s, e) =>
        {
            if (e.KeyCode == Keys.Escape) this.Close();
            if (e.KeyCode == Keys.Enter && _btnAcreditar.Focused) BtnAcreditar_Click(null, null!);
        };

        this.Load += AcreditacionForm_Load;
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
    private void TipoAcreditacion_Changed(object? sender, EventArgs e)
    {
        _pnlPorTurno.Visible = _rbPorTurno.Checked;
        _pnlEspontanea.Visible = _rbEspontanea.Checked;
        _lblMensaje.Text = string.Empty;
    }

    private async void AcreditacionForm_Load(object? sender, EventArgs e)
    {
        try
        {
            _btnAcreditar.DisableWithFeedback();
            await CargarDatosIniciales();
        }
        catch (Exception ex)
        {
            NotificationManager.ShowError("Error de Carga", "Error al cargar datos: " + ex.Message, this);
        }
        finally
        {
            _btnAcreditar.EnableWithFeedback();
        }
    }

    private async Task CargarDatosIniciales()
    {
        var turnos = await _turnoService.ObtenerTodosAsync();
        var turnosDisponibles = turnos.Where(t => t.Estado == "Reservado").ToList();
        _cmbTurno.DisplayMember = "DisplayInfo";
        _cmbTurno.ValueMember = "IdTurno";
        _cmbTurno.DataSource = turnosDisponibles.Select(t => new { t.IdTurno, DisplayInfo = $"ID {t.IdTurno} - {t.PacienteNombre} ({t.FechaHoraInicio:HH:mm})" }).ToList();

        var pacientes = await _pacienteService.ObtenerTodosAsync();
        _cmbPaciente.DisplayMember = "DNI";
        _cmbPaciente.ValueMember = "IdPaciente";
        _cmbPaciente.DataSource = pacientes.Select(p => new { p.IdPaciente, DNI = $"{p.DNI} - {p.Nombre} {p.Apellido}" }).ToList();

        var especialidades = await _especialidadService.ObtenerTodosAsync();
        _cmbEspecialidad.DisplayMember = "Nombre";
        _cmbEspecialidad.ValueMember = "IdEspecialidad";
        _cmbEspecialidad.DataSource = especialidades;

        _todosLosMedicos = await _medicoService.ObtenerTodosAsync();
        CmbEspecialidad_SelectedIndexChanged(null, EventArgs.Empty);
    }

    private void CmbTurno_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_cmbTurno.SelectedValue != null && _cmbTurno.SelectedItem != null)
        {
            dynamic item = _cmbTurno.SelectedItem;
            _lblTurnoInfo.Text = $"📋 ID Turno: {item.IdTurno}\n" +
                                $"👤 Paciente: {item.DisplayInfo.Split('-')[1]?.Trim()}\n" +
                                $"📅 Detalles: Ver en la selección\n\n" +
                                $"✓ Listo para acreditar";
        }
        else
        {
            _lblTurnoInfo.Text = "📌 Seleccione un turno de la lista...";
        }
    }

    private void CmbEspecialidad_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_cmbEspecialidad.SelectedItem != null && _cmbEspecialidad.SelectedValue != null)
        {
            int idEspecialidad = (int)_cmbEspecialidad.SelectedValue!;
            var medicosFiltrados = _todosLosMedicos.Where(m => 
                m.Especialidades?.Any(e => e.IdEspecialidad == idEspecialidad) == true).ToList();

            _cmbMedico.DisplayMember = "NombreCompleto";
            _cmbMedico.ValueMember = "IdMedico";
            _cmbMedico.DataSource = medicosFiltrados.Select(m => new { m.IdMedico, NombreCompleto = $"Dr. {m.Nombre} {m.Apellido}" }).ToList();
        }
    }

    private async void BtnAcreditar_Click(object? sender, EventArgs e)
    {
        _lblMensaje.Text = string.Empty;
        _btnAcreditar.ShowLoading();

        try
        {
            if (_rbPorTurno.Checked)
            {
                if (!ValidationHelper.ValidateSelection(_cmbTurno, _lblMensaje, "Turno")) 
                {
                    _btnAcreditar.HideLoading();
                    return;
                }

                var req = new AcreditarPacienteRequest
                {
                    IdTurno = (int)_cmbTurno.SelectedValue!,
                    ModalidadPago = _cmbModalidadPago.SelectedItem?.ToString() ?? "ObraSocial"
                };

                var atencion = await _atencionService.AcreditarPacienteAsync(req);
                NotificationManager.ShowSuccess(
                    "Paciente Acreditado",
                    $"Paciente acreditado exitosamente.\nAtención ID: {atencion?.IdAtencion}",
                    this
                );
            }
            else
            {
                if (!ValidationHelper.ValidateSelection(_cmbPaciente, _lblMensaje, "Paciente")) 
                {
                    _btnAcreditar.HideLoading();
                    return;
                }
                if (!ValidationHelper.ValidateSelection(_cmbMedico, _lblMensaje, "Médico")) 
                {
                    _btnAcreditar.HideLoading();
                    return;
                }

                var req = new CrearDemandaEspontaneaRequest
                {
                    IdPaciente = (int)_cmbPaciente.SelectedValue!,
                    IdMedico = (int)_cmbMedico.SelectedValue!,
                    ModalidadPago = _cmbModalidadPago.SelectedItem?.ToString() ?? "ObraSocial"
                };

                var atencion = await _atencionService.CrearDemandaEspontaneaAsync(req);
                NotificationManager.ShowSuccess(
                    "Demanda Registrada",
                    $"Demanda espontánea registrada exitosamente.\nAtención ID: {atencion?.IdAtencion}",
                    this
                );
            }

            LimpiarFormulario();
        }
        catch (Exception ex)
        {
            _lblMensaje.Text = $"❌ Error: {ex.Message}";
            _lblMensaje.Visible = true;
        }
        finally
        {
            _btnAcreditar.HideLoading();
        }
    }

    private void LimpiarFormulario()
    {
        _cmbTurno.SelectedIndex = -1;
        _cmbPaciente.SelectedIndex = -1;
        _cmbEspecialidad.SelectedIndex = -1;
        _cmbMedico.SelectedIndex = -1;
        _cmbModalidadPago.SelectedIndex = 0;
        _lblMensaje.Text = string.Empty;
        _lblTurnoInfo.Text = "Seleccione un turno...";
    }
}
