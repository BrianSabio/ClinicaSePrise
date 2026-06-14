using SePrise.WinForms.Models;
using SePrise.WinForms.Services;
using SePrise.WinForms.UX;
using System.ComponentModel;

namespace SePrise.WinForms.Forms.Atencion;

public partial class AtencionForm : Form
{
    private readonly AtencionService _atencionService;

    private ModernComboBox _cmbEstado = null!;
    private ModernButton _btnBuscar = null!;
    private DataGridView _gridAtenciones = null!;
    private ModernButton _btnIniciar = null!;
    private ModernButton _btnFinalizar = null!;
    private Button _btnCancelar = null!;
    private ModernButton _btnRefrescar = null!;

    private ModernPanel _pnlDetalles = null!;
    private Label _lblDetalleInfo = null!;
    private ModernTextBox _txtNotas = null!;
    private Label _lblTitle = null!;

    public AtencionForm(AtencionService atencionService)
    {
        _atencionService = atencionService;
        InitializeComponentCustom();
        ApplyTheme();
    }

    private void InitializeComponentCustom()
    {
        this.Text = "Gestión de Atenciones - Flujo Médico";
        this.ClientSize = new Size(1280, 680);
        this.StartPosition = FormStartPosition.CenterParent;
        this.Font = new Font("Segoe UI", 10);
        _lblTitle = new Label
        {
            Text = "Gestión de Atenciones - Flujo Médico",
            Location = new Point(20, 20),
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            AutoSize = true
        };
        this.Controls.Add(_lblTitle);
        var lblFiltro = new Label { Text = "Filtro Estado:", Location = new Point(20, 65), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
        this.Controls.Add(lblFiltro);

        _cmbEstado = new ModernComboBox { Location = new Point(160, 62), Width = 180 };
        _cmbEstado.Items.AddRange(new[] { "Todos", "Acreditada", "EnProgreso", "Finalizada", "Cancelada" });
        _cmbEstado.SelectedIndex = 0;
        _cmbEstado.AddTooltip("Filtrar atenciones por estado");
        this.Controls.Add(_cmbEstado);

        _btnBuscar = new ModernButton { Text = "🔍 Buscar", Location = new Point(350, 62), Width = 100 };
        _btnBuscar.Click += BtnBuscar_Click;
        _btnBuscar.AddTooltip("Buscar atenciones con el filtro seleccionado");
        this.Controls.Add(_btnBuscar);

        _btnRefrescar = new ModernButton { Text = "🔄 Refrescar", Location = new Point(460, 62), Width = 110 };
        _btnRefrescar.Click += BtnRefrescar_Click;
        _btnRefrescar.AddTooltip("Actualizar lista de atenciones");
        this.Controls.Add(_btnRefrescar);
        _gridAtenciones = new DataGridView
        {
            Location = new Point(20, 110),
            Size = new Size(840, 550),
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            BorderStyle = BorderStyle.FixedSingle,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
            AutoGenerateColumns = false
        };
        
        _gridAtenciones.Columns.AddRange(new DataGridViewColumn[] {
            new DataGridViewTextBoxColumn { DataPropertyName = "IdAtencion", HeaderText = "ID", Width = 50 },
            new DataGridViewTextBoxColumn { DataPropertyName = "FechaHoraAcreditacion", HeaderText = "Fecha", Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Format = "g" } },
            new DataGridViewTextBoxColumn { DataPropertyName = "PacienteNombre", HeaderText = "Paciente", Width = 150 },
            new DataGridViewTextBoxColumn { DataPropertyName = "MedicoNombre", HeaderText = "Médico", Width = 150 },
            new DataGridViewTextBoxColumn { DataPropertyName = "EspecialidadNombre", HeaderText = "Especialidad", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells },
            new DataGridViewTextBoxColumn { DataPropertyName = "ModalidadPago", HeaderText = "Modalidad", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells },
            new DataGridViewTextBoxColumn { DataPropertyName = "Estado", HeaderText = "Estado", Width = 100 }
        });

        _gridAtenciones.SelectionChanged += GridAtenciones_SelectionChanged;
        _gridAtenciones.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        this.Controls.Add(_gridAtenciones);
        _pnlDetalles = new ModernPanel { Location = new Point(880, 110), Size = new Size(380, 550), Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right };

        var lblDetalles = new Label { Text = "Detalles de Atención", Location = new Point(15, 15), Font = new Font("Segoe UI", 11, FontStyle.Bold), AutoSize = true };
        _pnlDetalles.Controls.Add(lblDetalles);

        _lblDetalleInfo = new Label { Location = new Point(15, 45), Size = new Size(350, 100), Text = "Seleccione una atención en la tabla...", AutoSize = false };
        _pnlDetalles.Controls.Add(_lblDetalleInfo);

        _pnlDetalles.Controls.Add(new Label { Text = "Notas Clínicas:", Location = new Point(15, 150), Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true });
        _txtNotas = new ModernTextBox { Location = new Point(15, 175), Width = 350, Height = 100 };
        _pnlDetalles.Controls.Add(_txtNotas);
        _btnIniciar = new ModernButton { Text = "▶ Iniciar Atención", Location = new Point(15, 290), Width = 350, Enabled = false };
        _btnIniciar.Click += BtnIniciar_Click;
        _pnlDetalles.Controls.Add(_btnIniciar);

        _btnFinalizar = new ModernButton { Text = "✓ Finalizar Atención", Location = new Point(15, 340), Width = 170, Enabled = false };
        _btnFinalizar.Click += BtnFinalizar_Click;
        _pnlDetalles.Controls.Add(_btnFinalizar);

        _btnCancelar = new Button
        {
            Text = "✕ Cancelar",
            Location = new Point(195, 340),
            Width = 170,
            Height = 40,
            Enabled = false,
            FlatStyle = FlatStyle.Flat
        };
        _btnCancelar.Click += BtnCancelar_Click;
        _pnlDetalles.Controls.Add(_btnCancelar);

        this.Controls.Add(_pnlDetalles);

        this.Load += AtencionForm_Load;
        ThemeManager.ThemeChanged += (s, e) => ApplyTheme();
    }

    private void ApplyTheme()
    {
        var colors = ThemeManager.CurrentColorScheme;
        this.BackColor = colors.BackgroundColor;
        this.ForeColor = colors.TextPrimary;
        _lblTitle.ForeColor = colors.TextPrimary;
        _gridAtenciones.BackgroundColor = colors.BackgroundColor;
        _gridAtenciones.GridColor = colors.BorderColor;
        ThemeApplier.ApplyThemeToForm(this);
    }
    private async void AtencionForm_Load(object? sender, EventArgs e)
    {
        await CargarDatosAsync();
    }

    private async Task CargarDatosAsync()
    {
        _btnBuscar.DisableWithFeedback();
        try
        {
            var atenciones = await _atencionService.ObtenerTodosAsync();
            var estadoFiltro = _cmbEstado.SelectedItem?.ToString();

            if (estadoFiltro != "Todos" && !string.IsNullOrEmpty(estadoFiltro))
            {
                atenciones = atenciones.Where(a => a.Estado == estadoFiltro).ToList();
            }

            var bindingList = new BindingList<AtencionDTO>(atenciones);
            _gridAtenciones.DataSource = bindingList;
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

    private void GridAtenciones_SelectionChanged(object? sender, EventArgs e)
    {
        if (_gridAtenciones.SelectedRows.Count > 0)
        {
            var atencion = (AtencionDTO)_gridAtenciones.SelectedRows[0].DataBoundItem!;

            _lblDetalleInfo.Text = $"📋 ID: {atencion.IdAtencion}\n" +
                                   $"👤 Paciente: {atencion.PacienteNombre}\n" +
                                   $"💳 Modalidad: {atencion.ModalidadPago}\n" +
                                   $"🔄 Estado: {atencion.Estado}";

            _txtNotas.Text = atencion.Notas;
            _txtNotas.ReadOnly = (atencion.Estado == "Finalizada" || atencion.Estado == "Cancelada");

            _btnIniciar.Enabled = atencion.Estado == "Acreditada";
            _btnFinalizar.Enabled = atencion.Estado == "EnProgreso";
            _btnCancelar.Enabled = atencion.Estado == "Acreditada" || atencion.Estado == "EnProgreso";
        }
        else
        {
            _lblDetalleInfo.Text = "Seleccione una atención de la tabla...";
            _txtNotas.Clear();
            _txtNotas.ReadOnly = true;
            _btnIniciar.Enabled = false;
            _btnFinalizar.Enabled = false;
            _btnCancelar.Enabled = false;
        }
    }

    private async void BtnBuscar_Click(object? sender, EventArgs e)
    {
        await CargarDatosAsync();
    }

    private async void BtnRefrescar_Click(object? sender, EventArgs e)
    {
        await CargarDatosAsync();
    }

    private async void BtnIniciar_Click(object? sender, EventArgs e)
    {
        if (_gridAtenciones.SelectedRows.Count == 0)
        {
            NotificationManager.ShowWarning("Selección Requerida", "Seleccione una atención primero", this);
            return;
        }

        var atencion = (AtencionDTO)_gridAtenciones.SelectedRows[0].DataBoundItem!;
        _btnIniciar.ShowLoading();

        try
        {
            await _atencionService.IniciarAsync(atencion.IdAtencion);
            NotificationManager.ShowSuccess("Atención Iniciada", "La atención ha sido iniciada correctamente", this);
            await CargarDatosAsync();
        }
        catch (Exception ex)
        {
            NotificationManager.ShowError("Error", $"Error al iniciar atención: {ex.Message}", this);
        }
        finally
        {
            _btnIniciar.HideLoading();
        }
    }

    private async void BtnFinalizar_Click(object? sender, EventArgs e)
    {
        if (_gridAtenciones.SelectedRows.Count == 0)
        {
            NotificationManager.ShowWarning("Selección Requerida", "Seleccione una atención primero", this);
            return;
        }

        var atencion = (AtencionDTO)_gridAtenciones.SelectedRows[0].DataBoundItem!;

        if (string.IsNullOrWhiteSpace(_txtNotas.Text))
        {
            NotificationManager.ShowWarning("Campos Requeridos", "Ingrese notas antes de finalizar", this);
            return;
        }

        if (NotificationManager.ShowConfirm("Confirmar Finalización", "¿Finalizar esta atención?", this) != DialogResult.Yes)
        {
            return;
        }

        _btnFinalizar.ShowLoading();

        try
        {
            await _atencionService.FinalizarAsync(atencion.IdAtencion, _txtNotas.Text);

            NotificationManager.ShowSuccess("Atención Finalizada", "La atención ha sido finalizada correctamente", this);
            await CargarDatosAsync();
        }
        catch (Exception ex)
        {
            NotificationManager.ShowError("Error", $"Error al finalizar atención: {ex.Message}", this);
        }
        finally
        {
            _btnFinalizar.HideLoading();
        }
    }

    private async void BtnCancelar_Click(object? sender, EventArgs e)
    {
        if (_gridAtenciones.SelectedRows.Count == 0)
        {
            NotificationManager.ShowWarning("Selección Requerida", "Seleccione una atención primero", this);
            return;
        }

        if (NotificationManager.ShowConfirm("Confirmar Cancelación", "¿Cancelar esta atención?", this) != DialogResult.Yes)
        {
            return;
        }

        var atencion = (AtencionDTO)_gridAtenciones.SelectedRows[0].DataBoundItem!;
        _btnCancelar.Enabled = false;

        try
        {
            await _atencionService.CancelarAsync(atencion.IdAtencion);
            NotificationManager.ShowSuccess("Atención Cancelada", "La atención ha sido cancelada", this);
            await CargarDatosAsync();
        }
        catch (Exception ex)
        {
            NotificationManager.ShowError("Error", $"Error al cancelar atención: {ex.Message}", this);
        }
        finally
        {
            _btnCancelar.Enabled = true;
        }
    }
}


