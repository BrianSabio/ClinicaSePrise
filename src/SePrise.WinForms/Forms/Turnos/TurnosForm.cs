using SePrise.WinForms.Models;
using SePrise.WinForms.Services;
using SePrise.WinForms.UX;
using System.ComponentModel;

namespace SePrise.WinForms.Forms.Turnos;

public partial class TurnosForm : Form
{
    private readonly TurnoService _turnoService;
    private readonly PacienteService _pacienteService;
    private readonly MedicoService _medicoService;
    private readonly EspecialidadService _especialidadService;
    private readonly SalaService _salaService;

    private ModernComboBox _cmbEstado = null!;
    private ModernButton _btnBuscar = null!;
    private DataGridView _gridTurnos = null!;
    private ModernButton _btnNuevo = null!;
    private ModernButton _btnConfirmar = null!;
    private Button _btnCancelar = null!;
    private ModernButton _btnReprogramar = null!;
    private ModernButton _btnRefrescar = null!;
    private Label _lblTitle = null!;

    public TurnosForm(TurnoService turnoService, PacienteService pacienteService, MedicoService medicoService, EspecialidadService especialidadService, SalaService salaService)
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
        this.Text = "Gestión de Turnos Médicos";
        this.Size = new Size(1050, 650);
        this.StartPosition = FormStartPosition.CenterParent;
        this.Font = new Font("Segoe UI", 10);

        // ========== TITLE ==========
        _lblTitle = new Label
        {
            Text = "Gestión de Turnos Médicos",
            Location = new Point(20, 20),
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            AutoSize = true
        };
        this.Controls.Add(_lblTitle);

        // ========== TOOLBAR ==========
        var lblFiltro = new Label { Text = "Filtro Estado:", Location = new Point(20, 65), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
        this.Controls.Add(lblFiltro);

        _cmbEstado = new ModernComboBox { Location = new Point(160, 62), Width = 180 };
        _cmbEstado.Items.AddRange(new[] { "Todos", "Reservado", "Confirmado", "Atendido", "NoAsistio", "Cancelado", "Reprogramado" });
        _cmbEstado.SelectedIndex = 0;
        _cmbEstado.AddTooltip("Filtrar turnos por estado");
        this.Controls.Add(_cmbEstado);

        _btnBuscar = new ModernButton { Text = "🔍 Buscar", Location = new Point(350, 62), Width = 100 };
        _btnBuscar.Click += BtnBuscar_Click;
        _btnBuscar.AddTooltip("Buscar turnos con el filtro seleccionado");
        this.Controls.Add(_btnBuscar);

        _btnRefrescar = new ModernButton { Text = "🔄 Refrescar", Location = new Point(460, 62), Width = 110 };
        _btnRefrescar.Click += BtnRefrescar_Click;
        _btnRefrescar.AddTooltip("Actualizar lista de turnos");
        this.Controls.Add(_btnRefrescar);

        // ========== GRID DE TURNOS ==========
        _gridTurnos = new DataGridView
        {
            Location = new Point(20, 110),
            Size = new Size(1000, 380),
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            BorderStyle = BorderStyle.FixedSingle,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
        };
        _gridTurnos.SelectionChanged += GridTurnos_SelectionChanged;
        _gridTurnos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        this.Controls.Add(_gridTurnos);

        // ========== ACTION BUTTONS ==========
        _btnNuevo = new ModernButton { Text = "➕ Nuevo Turno", Location = new Point(20, 500), Width = 140, Anchor = AnchorStyles.Bottom | AnchorStyles.Left };
        _btnNuevo.Click += BtnNuevo_Click;
        this.Controls.Add(_btnNuevo);

        _btnConfirmar = new ModernButton { Text = "✓ Confirmar", Location = new Point(170, 500), Width = 140, Enabled = false, Anchor = AnchorStyles.Bottom | AnchorStyles.Left };
        _btnConfirmar.Click += BtnConfirmar_Click;
        this.Controls.Add(_btnConfirmar);

        _btnReprogramar = new ModernButton { Text = "🔁 Reprogramar", Location = new Point(320, 500), Width = 140, Enabled = false, Anchor = AnchorStyles.Bottom | AnchorStyles.Left };
        _btnReprogramar.Click += BtnReprogramar_Click;
        this.Controls.Add(_btnReprogramar);

        _btnCancelar = new Button
        {
            Text = "✕ Cancelar",
            Location = new Point(470, 500),
            Width = 140,
            Height = 40,
            Enabled = false,
            FlatStyle = FlatStyle.Flat,
            Anchor = AnchorStyles.Bottom | AnchorStyles.Left
        };
        _btnCancelar.Click += BtnCancelar_Click;
        this.Controls.Add(_btnCancelar);

        this.Load += TurnosForm_Load;
        ThemeManager.ThemeChanged += (s, e) => ApplyTheme();
    }

    private void ApplyTheme()
    {
        var colors = ThemeManager.CurrentColorScheme;
        this.BackColor = colors.BackgroundColor;
        this.ForeColor = colors.TextPrimary;
        _lblTitle.ForeColor = colors.TextPrimary;
        _gridTurnos.BackgroundColor = colors.BackgroundColor;
        _gridTurnos.GridColor = colors.BorderColor;
        ThemeApplier.ApplyThemeToForm(this);
    }

    private async void TurnosForm_Load(object? sender, EventArgs e)
    {
        await CargarDatosAsync();
    }

    private async Task CargarDatosAsync()
    {
        _btnBuscar.DisableWithFeedback();
        try
        {
            var turnos = await _turnoService.ObtenerTodosAsync();
            var estadoFiltro = _cmbEstado.SelectedItem?.ToString();

            if (estadoFiltro != "Todos" && !string.IsNullOrEmpty(estadoFiltro))
            {
                turnos = turnos.Where(t => t.Estado == estadoFiltro).ToList();
            }

            var bindingList = new BindingList<TurnoDTO>(turnos);
            _gridTurnos.DataSource = bindingList;
        }
        catch (Exception ex)
        {
            NotificationManager.ShowError("Error de Carga", $"Error al cargar datos: {ex.Message}", this);
        }
        finally
        {
            _btnBuscar.EnableWithFeedback();
        }
    }

    private void GridTurnos_SelectionChanged(object? sender, EventArgs e)
    {
        if (_gridTurnos.SelectedRows.Count > 0)
        {
            var turno = (TurnoDTO)_gridTurnos.SelectedRows[0].DataBoundItem;
            _btnConfirmar.Enabled = turno.Estado == "Reservado";
            _btnCancelar.Enabled = turno.Estado == "Reservado" || turno.Estado == "Confirmado";
            _btnReprogramar.Enabled = turno.Estado == "Reservado" || turno.Estado == "Confirmado";
        }
        else
        {
            _btnConfirmar.Enabled = false;
            _btnCancelar.Enabled = false;
            _btnReprogramar.Enabled = false;
        }
    }

    private async void BtnBuscar_Click(object? sender, EventArgs e)
    {
        await CargarDatosAsync();
    }

    private async void BtnRefrescar_Click(object? sender, EventArgs e)
    {
        _cmbEstado.SelectedIndex = 0;
        await CargarDatosAsync();
    }

    private async void BtnNuevo_Click(object? sender, EventArgs e)
    {
        var form = new CrearTurnoForm(_turnoService, _pacienteService, _medicoService, _especialidadService, _salaService);
        if (form.ShowDialog() == DialogResult.OK)
        {
            await CargarDatosAsync();
        }
    }

    private async void BtnConfirmar_Click(object? sender, EventArgs e)
    {
        if (_gridTurnos.SelectedRows.Count == 0)
        {
            NotificationManager.ShowWarning("Selección Requerida", "Seleccione un turno primero", this);
            return;
        }

        var turno = (TurnoDTO)_gridTurnos.SelectedRows[0].DataBoundItem;

        if (NotificationManager.ShowConfirm("Confirmar Turno", $"¿Confirmar turno ID {turno.IdTurno}?", this) != DialogResult.Yes)
        {
            return;
        }

        _btnConfirmar.ShowLoading();
        try
        {
            await _turnoService.ConfirmarAsync(turno.IdTurno);
            NotificationManager.ShowSuccess("Turno Confirmado", "Turno confirmado exitosamente", this);
            await CargarDatosAsync();
        }
        catch (Exception ex)
        {
            NotificationManager.ShowError("Error", $"Error al confirmar turno: {ex.Message}", this);
        }
        finally
        {
            _btnConfirmar.HideLoading();
        }
    }

    private async void BtnCancelar_Click(object? sender, EventArgs e)
    {
        if (_gridTurnos.SelectedRows.Count == 0)
        {
            NotificationManager.ShowWarning("Selección Requerida", "Seleccione un turno primero", this);
            return;
        }

        var turno = (TurnoDTO)_gridTurnos.SelectedRows[0].DataBoundItem;

        if (NotificationManager.ShowConfirm("Cancelar Turno", $"¿Cancelar turno ID {turno.IdTurno}?", this) != DialogResult.Yes)
        {
            return;
        }

        _btnCancelar.Enabled = false;
        try
        {
            await _turnoService.CancelarAsync(turno.IdTurno);
            NotificationManager.ShowSuccess("Turno Cancelado", "Turno cancelado exitosamente", this);
            await CargarDatosAsync();
        }
        catch (Exception ex)
        {
            NotificationManager.ShowError("Error", $"Error al cancelar turno: {ex.Message}", this);
        }
        finally
        {
            _btnCancelar.Enabled = true;
        }
    }

    private async void BtnReprogramar_Click(object? sender, EventArgs e)
    {
        if (_gridTurnos.SelectedRows.Count == 0)
        {
            NotificationManager.ShowWarning("Selección Requerida", "Seleccione un turno primero", this);
            return;
        }

        var turno = (TurnoDTO)_gridTurnos.SelectedRows[0].DataBoundItem;

        // Abre el formulario de crear turno (adaptado para reprogramación)
        var form = new CrearTurnoForm(_turnoService, _pacienteService, _medicoService, _especialidadService, _salaService);
        if (form.ShowDialog() == DialogResult.OK)
        {
            await CargarDatosAsync();
            NotificationManager.ShowSuccess("Turno Reprogramado", "Turno reprogramado exitosamente", this);
        }
    }
}
