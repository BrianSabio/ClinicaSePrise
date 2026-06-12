using SePrise.WinForms.Models;
using SePrise.WinForms.Services;
using SePrise.WinForms.UX;

namespace SePrise.WinForms.Forms.Pacientes;

public partial class PacientesForm : Form
{
    private readonly PacienteService _pacienteService;

    private ModernTextBox _txtBuscar = null!;
    private ModernButton _btnBuscar = null!;
    private DataGridView _gridPacientes = null!;
    private ModernButton _btnNuevo = null!;
    private ModernButton _btnEditar = null!;
    private ModernButton _btnEliminar = null!;
    private ModernButton _btnRefrescar = null!;
    private Label _lblTotal = null!;

    public PacientesForm(PacienteService pacienteService)
    {
        _pacienteService = pacienteService;
        InitializeComponentCustom();
        ApplyTheme();
    }

    private void InitializeComponentCustom()
    {
        this.Text = "Gestión de Pacientes";
        this.Size = new Size(1000, 650);
        this.Font = new Font("Segoe UI", 10);

        // ========== SEARCH PANEL ==========
        var pnlSearch = new Panel
        {
            Dock = DockStyle.Top,
            Height = 70,
            Padding = new Padding(20)
        };

        var lblBuscar = new Label 
        { 
            Text = "🔍 Buscar por DNI:", 
            Location = new Point(20, 15), 
            AutoSize = true,
            Font = new Font("Segoe UI", 10, FontStyle.Regular)
        };
        pnlSearch.Controls.Add(lblBuscar);

        _txtBuscar = new ModernTextBox 
        { 
            Location = new Point(210, 12), // Movido a la derecha para no pisar el label
            Width = 200,
            Height = 35
        };
        _txtBuscar.AddTooltip("Ingrese un DNI para buscar (Enter para buscar)");
        _txtBuscar.KeyPress += (s, e) =>
        {
            if (e.KeyChar == '\r')
            {
                e.Handled = true;
                BtnBuscar_Click(null, EventArgs.Empty);
            }
        };
        pnlSearch.Controls.Add(_txtBuscar);

        _btnBuscar = new ModernButton 
        { 
            Text = "Buscar", 
            Location = new Point(420, 12), // Movido a la derecha
            Width = 80,
            Height = 35
        };
        _btnBuscar.Click += BtnBuscar_Click;
        _btnBuscar.AddTooltip("Buscar paciente por DNI");
        pnlSearch.Controls.Add(_btnBuscar);

        _btnRefrescar = new ModernButton 
        { 
            Text = "🔄 Actualizar", 
            Location = new Point(510, 12), // Movido a la derecha
            Width = 100,
            Height = 35
        };
        _btnRefrescar.Click += BtnRefrescar_Click;
        _btnRefrescar.AddTooltip("Cargar todos los pacientes");
        pnlSearch.Controls.Add(_btnRefrescar);

        _lblTotal = new Label
        {
            Text = "Total: 0 pacientes",
            Location = new Point(630, 20), // Movido a la derecha
            AutoSize = true,
            Font = new Font("Segoe UI", 9, FontStyle.Regular)
        };
        pnlSearch.Controls.Add(_lblTotal);

        // ========== GRID ==========
        _gridPacientes = new DataGridView
        {
            Dock = DockStyle.Fill,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            ReadOnly = true,
            AllowUserToAddRows = false,
            RowHeadersVisible = false,
            AutoGenerateColumns = false, // <-- EVITA QUE SE DUPLIQUEN LAS COLUMNAS AL ASIGNAR DATASOURCE
            BorderStyle = BorderStyle.FixedSingle,
            Margin = new Padding(20)
        };
        _gridPacientes.SelectionChanged += GridPacientes_SelectionChanged;
        ConfigurarGrid();

        // ========== BUTTON PANEL ==========
        var pnlBotones = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 70,
            Padding = new Padding(20)
        };

        _btnNuevo = new ModernButton 
        { 
            Text = "➕ Nuevo Paciente", 
            Location = new Point(20, 15), 
            Width = 140,
            Height = 40
        };
        _btnNuevo.Click += BtnNuevo_Click;
        _btnNuevo.AddTooltip("Crear un nuevo paciente");
        pnlBotones.Controls.Add(_btnNuevo);

        _btnEditar = new ModernButton 
        { 
            Text = "✏️ Editar", 
            Location = new Point(170, 15), 
            Width = 100,
            Height = 40,
            Enabled = false
        };
        _btnEditar.Click += BtnEditar_Click;
        _btnEditar.AddTooltip("Editar el paciente seleccionado");
        pnlBotones.Controls.Add(_btnEditar);

        _btnEliminar = new ModernButton 
        { 
            Text = "🗑️ Eliminar", 
            Location = new Point(280, 15), 
            Width = 100,
            Height = 40,
            Enabled = false
        };
        _btnEliminar.Click += BtnEliminar_Click;
        _btnEliminar.AddTooltip("Eliminar el paciente seleccionado");
        pnlBotones.Controls.Add(_btnEliminar);

        this.Controls.Add(_gridPacientes);
        this.Controls.Add(pnlBotones);
        this.Controls.Add(pnlSearch);

        this.Load += PacientesForm_Load;

        // ========== THEME SUPPORT ==========
        ThemeManager.ThemeChanged += (s, e) => ApplyTheme();
    }

    private void ConfigurarGrid()
    {
        _gridPacientes.Columns.Clear();
        
        _gridPacientes.Columns.Add(new DataGridViewTextBoxColumn { Name = "IdPaciente", HeaderText = "ID", DataPropertyName = "IdPaciente", Width = 50 });
        _gridPacientes.Columns.Add(new DataGridViewTextBoxColumn { Name = "DNI", HeaderText = "DNI", DataPropertyName = "DNI", Width = 100 });
        _gridPacientes.Columns.Add(new DataGridViewTextBoxColumn { Name = "Nombre", HeaderText = "Nombre", DataPropertyName = "Nombre", Width = 120 });
        _gridPacientes.Columns.Add(new DataGridViewTextBoxColumn { Name = "Apellido", HeaderText = "Apellido", DataPropertyName = "Apellido", Width = 120 });
        _gridPacientes.Columns.Add(new DataGridViewTextBoxColumn { Name = "Email", HeaderText = "Email", DataPropertyName = "Email", Width = 150 });
        _gridPacientes.Columns.Add(new DataGridViewTextBoxColumn { Name = "Telefono", HeaderText = "Teléfono", DataPropertyName = "Telefono", Width = 100 });
        _gridPacientes.Columns.Add(new DataGridViewTextBoxColumn { Name = "FechaNacimiento", HeaderText = "Fecha Nacimiento", DataPropertyName = "FechaNacimiento", Width = 120 });
    }

    private void ApplyTheme()
    {
        var colors = ThemeManager.CurrentColorScheme;
        this.BackColor = colors.BackgroundColor;
        this.ForeColor = colors.TextPrimary;

        _gridPacientes.BackgroundColor = colors.BackgroundColor;
        _gridPacientes.ForeColor = colors.TextPrimary;
        _gridPacientes.GridColor = colors.BorderColor;

        ThemeApplier.ApplyThemeToForm(this);
    }

    private async void PacientesForm_Load(object? sender, EventArgs e)
    {
        await CargarDatosAsync();
    }

    private async Task CargarDatosAsync(string? dni = null)
    {
        DeshabilitarBotones();
        try
        {
            List<PacienteDTO> pacientes;
            if (string.IsNullOrWhiteSpace(dni))
            {
                pacientes = await _pacienteService.ObtenerTodosAsync();
            }
            else
            {
                pacientes = await _pacienteService.BuscarPorDNIAsync(dni);
            }
            _gridPacientes.DataSource = pacientes;
            
            if (pacientes.Count == 0 && !string.IsNullOrWhiteSpace(dni))
            {
                MessageBox.Show("No se encontraron resultados para la búsqueda.", "Búsqueda", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al cargar datos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            HabilitarBotones();
        }
    }

    private void GridPacientes_SelectionChanged(object? sender, EventArgs e)
    {
        bool hasSelection = _gridPacientes.SelectedRows.Count > 0;
        _btnEditar.Enabled = hasSelection;
        _btnEliminar.Enabled = hasSelection;
    }

    private void DeshabilitarBotones()
    {
        _btnBuscar.Enabled = false;
        _btnNuevo.Enabled = false;
        _btnEditar.Enabled = false;
        _btnEliminar.Enabled = false;
        _btnRefrescar.Enabled = false;
    }

    private void HabilitarBotones()
    {
        _btnBuscar.Enabled = true;
        _btnNuevo.Enabled = true;
        _btnRefrescar.Enabled = true;
        GridPacientes_SelectionChanged(null, EventArgs.Empty);
    }

    private async void BtnBuscar_Click(object? sender, EventArgs e)
    {
        await CargarDatosAsync(_txtBuscar.Text.Trim());
    }

    private async void BtnRefrescar_Click(object? sender, EventArgs e)
    {
        _txtBuscar.Clear();
        await CargarDatosAsync();
    }

    private async void BtnNuevo_Click(object? sender, EventArgs e)
    {
        var form = new CrearPacienteForm(_pacienteService);
        if (form.ShowDialog() == DialogResult.OK)
        {
            await CargarDatosAsync();
        }
    }

    private async void BtnEditar_Click(object? sender, EventArgs e)
    {
        if (_gridPacientes.SelectedRows.Count == 0) return;
        var paciente = (PacienteDTO)_gridPacientes.SelectedRows[0].DataBoundItem;
        
        var form = new EditarPacienteForm(_pacienteService, paciente);
        if (form.ShowDialog() == DialogResult.OK)
        {
            await CargarDatosAsync();
        }
    }

    private async void BtnEliminar_Click(object? sender, EventArgs e)
    {
        if (_gridPacientes.SelectedRows.Count == 0) return;
        var paciente = (PacienteDTO)_gridPacientes.SelectedRows[0].DataBoundItem;

        var result = MessageBox.Show($"¿Está seguro de eliminar a {paciente.Nombre} {paciente.Apellido}?", 
            "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

        if (result == DialogResult.Yes)
        {
            DeshabilitarBotones();
            try
            {
                await _pacienteService.EliminarAsync(paciente.IdPaciente);
                await CargarDatosAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                HabilitarBotones();
            }
        }
    }
}
