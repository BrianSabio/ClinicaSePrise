using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClosedXML.Excel;
using SePrise.WinForms.Models;
using SePrise.WinForms.Services;
using SePrise.WinForms.UX;

namespace SePrise.WinForms.Forms.Reportes;

public partial class GenerarReporteForm : Form
{
    private readonly ReportesService _reportesService;
    private readonly MedicoService _medicoService;
    private readonly EspecialidadService _especialidadService;

    private Label _lblTitle = null!;
    private DateTimePicker _dtpFechaDesde = null!;
    private DateTimePicker _dtpFechaHasta = null!;
    private ModernComboBox _cmbMedico = null!;
    private ModernComboBox _cmbEspecialidad = null!;
    private ModernButton _btnVistaPrevia = null!;
    private Button _btnLimpiarFiltros = null!;
    
    private Panel _pnlResumen = null!;
    private Label _lblResumenContenido = null!;
    
    private DataGridView _gridReporte = null!;
    
    private ModernButton _btnDescargarExcel = null!;
    private ModernButton _btnDescargarCsv = null!;

    public GenerarReporteForm(
        ReportesService reportesService,
        MedicoService medicoService,
        EspecialidadService especialidadService)
    {
        _reportesService = reportesService ?? throw new ArgumentNullException(nameof(reportesService));
        _medicoService = medicoService ?? throw new ArgumentNullException(nameof(medicoService));
        _especialidadService = especialidadService ?? throw new ArgumentNullException(nameof(especialidadService));
        
        InitializeComponentCustom();
        ApplyTheme();
        
        this.Load += GenerarReporteForm_Load;
    }

    private void InitializeComponentCustom()
    {
        this.Text = "Generar Reporte";
        this.Size = new Size(900, 700);
        this.StartPosition = FormStartPosition.CenterParent;
        this.Font = new Font("Segoe UI", 10);
        
        _lblTitle = new Label
        {
            Text = "📊 Generar Reporte",
            Location = new Point(20, 20),
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            AutoSize = true
        };
        this.Controls.Add(_lblTitle);

        // Panel Filtros
        var pnlFiltros = new Panel
        {
            Location = new Point(20, 60),
            Size = new Size(840, 190),
            BorderStyle = BorderStyle.FixedSingle,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        var lblFiltros = new Label { Text = "Filtros:", Location = new Point(10, 10), Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true };
        pnlFiltros.Controls.Add(lblFiltros);

        // Fila 1: Fechas (Y=40)
        var lblFechaDesde = new Label { Text = "Desde:", Location = new Point(10, 40), AutoSize = true };
        pnlFiltros.Controls.Add(lblFechaDesde);
        
        _dtpFechaDesde = new DateTimePicker { Location = new Point(90, 38), Width = 130, Format = DateTimePickerFormat.Short };
        _dtpFechaDesde.Value = DateTime.Today.AddDays(-30);
        pnlFiltros.Controls.Add(_dtpFechaDesde);

        var lblFechaHasta = new Label { Text = "Hasta:", Location = new Point(250, 40), AutoSize = true };
        pnlFiltros.Controls.Add(lblFechaHasta);

        _dtpFechaHasta = new DateTimePicker { Location = new Point(320, 38), Width = 130, Format = DateTimePickerFormat.Short };
        _dtpFechaHasta.Value = DateTime.Today;
        pnlFiltros.Controls.Add(_dtpFechaHasta);

        // Fila 2: Listas Desplegables (Y=90)
        var lblMedico = new Label { Text = "Médico:", Location = new Point(10, 90), AutoSize = true };
        pnlFiltros.Controls.Add(lblMedico);

        _cmbMedico = new ModernComboBox { Location = new Point(90, 88), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
        pnlFiltros.Controls.Add(_cmbMedico);

        var lblEspecialidad = new Label { Text = "Especialidad:", Location = new Point(320, 90), AutoSize = true };
        pnlFiltros.Controls.Add(lblEspecialidad);

        _cmbEspecialidad = new ModernComboBox { Location = new Point(470, 88), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
        pnlFiltros.Controls.Add(_cmbEspecialidad);

        // Fila 3: Botones (Y=140)
        _btnVistaPrevia = new ModernButton { Text = "🔍 Vista Previa", Location = new Point(10, 140), Width = 160 };
        _btnVistaPrevia.Click += BtnVistaPrevia_Click;
        pnlFiltros.Controls.Add(_btnVistaPrevia);

        _btnLimpiarFiltros = new Button { Text = "Limpiar Filtros", Location = new Point(180, 140), Width = 120, Height = 35, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
        _btnLimpiarFiltros.Click += BtnLimpiarFiltros_Click;
        pnlFiltros.Controls.Add(_btnLimpiarFiltros);

        this.Controls.Add(pnlFiltros);

        // Panel Resumen
        _pnlResumen = new Panel
        {
            Location = new Point(20, 260),
            Size = new Size(840, 40),
            BackColor = SystemColors.ControlLight,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            Visible = false
        };

        _lblResumenContenido = new Label
        {
            Text = "",
            Location = new Point(10, 10),
            AutoSize = true,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        _pnlResumen.Controls.Add(_lblResumenContenido);
        this.Controls.Add(_pnlResumen);

        // Grid
        _gridReporte = new DataGridView
        {
            Location = new Point(20, 310),
            Size = new Size(840, 270),
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
            AllowUserToAddRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = Color.White,
            AutoGenerateColumns = false
        };
        _gridReporte.Columns.AddRange(new DataGridViewColumn[] {
            new DataGridViewTextBoxColumn { DataPropertyName = "IdAtencion", HeaderText = "ID", Width = 50 },
            new DataGridViewTextBoxColumn { DataPropertyName = "FechaHoraAcreditacion", HeaderText = "Fecha", Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Format = "g" } },
            new DataGridViewTextBoxColumn { DataPropertyName = "PacienteNombre", HeaderText = "Paciente", Width = 150 },
            new DataGridViewTextBoxColumn { DataPropertyName = "MedicoNombre", HeaderText = "Médico", Width = 150 },
            new DataGridViewTextBoxColumn { DataPropertyName = "EspecialidadNombre", HeaderText = "Especialidad", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells },
            new DataGridViewTextBoxColumn { DataPropertyName = "ModalidadPago", HeaderText = "Modalidad", Width = 100 },
            new DataGridViewTextBoxColumn { DataPropertyName = "Estado", HeaderText = "Estado", Width = 100 }
        });
        this.Controls.Add(_gridReporte);

        // Botones Descarga
        _btnDescargarExcel = new ModernButton { Text = "📊 Exportar Excel (.xlsx)", Location = new Point(20, 600), Width = 200, Anchor = AnchorStyles.Bottom | AnchorStyles.Left, Enabled = false };
        _btnDescargarExcel.Click += BtnDescargarExcel_Click;
        this.Controls.Add(_btnDescargarExcel);

        _btnDescargarCsv = new ModernButton { Text = "📝 Exportar CSV (.csv)", Location = new Point(235, 600), Width = 200, Anchor = AnchorStyles.Bottom | AnchorStyles.Left, Enabled = false };
        _btnDescargarCsv.Click += BtnDescargarCsv_Click;
        this.Controls.Add(_btnDescargarCsv);
    }

    private void ApplyTheme()
    {
        var colors = ThemeManager.CurrentColorScheme;
        this.BackColor = colors.BackgroundColor;
        this.ForeColor = colors.TextPrimary;
        _lblTitle.ForeColor = colors.TextPrimary;
        
        ThemeApplier.ApplyThemeToForm(this);
        
        // El ThemeApplier pisa todos los botones, así que restauramos el botón secundario "Limpiar" después
        _btnLimpiarFiltros.BackColor = colors.SurfaceColor;
        _btnLimpiarFiltros.ForeColor = colors.TextPrimary;
        _btnLimpiarFiltros.FlatAppearance.BorderColor = colors.BorderColor;
    }

    private async void GenerarReporteForm_Load(object? sender, EventArgs e)
    {
        await CargarComboBoxesAsync();
    }

    private async Task CargarComboBoxesAsync()
    {
        try
        {
            var medicos = await _medicoService.ObtenerTodosAsync();
            var medicosList = medicos.ToList();
            medicosList.Insert(0, new MedicoDTO { IdMedico = 0, Nombre = "Todos", Apellido = "" });
            
            _cmbMedico.DisplayMember = "NombreCompleto";
            _cmbMedico.ValueMember = "IdMedico";
            _cmbMedico.DataSource = medicosList.Select(m => new { IdMedico = m.IdMedico, NombreCompleto = m.IdMedico == 0 ? "Todos" : $"{m.Nombre} {m.Apellido}" }).ToList();
            
            var especialidades = await _especialidadService.ObtenerTodosAsync();
            var espList = especialidades.ToList();
            espList.Insert(0, new EspecialidadDTO { IdEspecialidad = 0, Nombre = "Todas" });
            
            _cmbEspecialidad.DisplayMember = "Nombre";
            _cmbEspecialidad.ValueMember = "IdEspecialidad";
            _cmbEspecialidad.DataSource = espList;
        }
        catch (Exception ex)
        {
            NotificationManager.ShowError("Error de Carga", $"Error al cargar listas: {ex.Message}", this);
        }
    }

    private void BtnLimpiarFiltros_Click(object? sender, EventArgs e)
    {
        _dtpFechaDesde.Value = DateTime.Today.AddDays(-30);
        _dtpFechaHasta.Value = DateTime.Today;
        _cmbMedico.SelectedIndex = 0;
        _cmbEspecialidad.SelectedIndex = 0;
        _gridReporte.DataSource = null;
        _pnlResumen.Visible = false;
        ActualizarEstadoBotones();
    }

    private async void BtnVistaPrevia_Click(object? sender, EventArgs e)
    {
        await CargarDatosAsync();
    }

    private async Task CargarDatosAsync()
    {
        if (_dtpFechaDesde.Value.Date > _dtpFechaHasta.Value.Date)
        {
            NotificationManager.ShowWarning("Fechas inválidas", "La fecha 'Desde' no puede ser mayor que 'Hasta'.", this);
            return;
        }

        _btnVistaPrevia.DisableWithFeedback();
        try
        {
            DateTime fechaDesde = _dtpFechaDesde.Value.Date;
            DateTime fechaHasta = _dtpFechaHasta.Value.Date;
            int idMedico = (int)(_cmbMedico.SelectedValue ?? 0);
            int idEspecialidad = (int)(_cmbEspecialidad.SelectedValue ?? 0);

            // Resumen
            var resumen = await _reportesService.ObtenerResumenAsync(fechaDesde, fechaHasta);
            _lblResumenContenido.Text = $"Total Atenciones: {resumen.TotalAtenciones} | Obra Social: {resumen.TotalObraSocial} | Particular: {resumen.TotalParticular} | Tiempo Promedio: {resumen.TiempoPromedioMinutos} min";
            _pnlResumen.Visible = true;

            // Datos
            IEnumerable<AtencionDTO> datos = new List<AtencionDTO>();

            if (idMedico > 0)
            {
                datos = await _reportesService.ObtenerPorMedicoAsync(idMedico, fechaDesde, fechaHasta);
                if (idEspecialidad > 0)
                {
                    datos = datos.Where(a => a.EspecialidadNombre == _cmbEspecialidad.Text).ToList();
                }
            }
            else if (idEspecialidad > 0)
            {
                datos = await _reportesService.ObtenerPorEspecialidadAsync(idEspecialidad, fechaDesde, fechaHasta);
            }
            else
            {
                datos = await _reportesService.ObtenerPorFechaAsync(fechaDesde, fechaHasta);
            }

            var bindingList = new BindingList<AtencionDTO>(datos.ToList());
            _gridReporte.DataSource = bindingList;

            if (!datos.Any())
            {
                NotificationManager.ShowInfo("Sin Datos", "No se encontraron atenciones para los filtros seleccionados.", this);
            }
        }
        catch (Exception ex)
        {
            NotificationManager.ShowError("Error", $"Error al cargar vista previa: {ex.Message}", this);
        }
        finally
        {
            _btnVistaPrevia.EnableWithFeedback();
            ActualizarEstadoBotones();
        }
    }

    private void ActualizarEstadoBotones()
    {
        bool tieneDatos = _gridReporte.Rows.Count > 0;
        _btnDescargarExcel.Enabled = tieneDatos;
        _btnDescargarCsv.Enabled = tieneDatos;
    }

    private void BtnDescargarExcel_Click(object? sender, EventArgs e)
    {
        if (_gridReporte.Rows.Count == 0) return;

        using SaveFileDialog sfd = new SaveFileDialog();
        sfd.Filter = "Excel Workbook|*.xlsx";
        sfd.Title = "Guardar Reporte Excel";
        sfd.FileName = $"Reporte_Atenciones_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";

        if (sfd.ShowDialog() == DialogResult.OK)
        {
            try
            {
                using var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Reporte");

                // Headers
                for (int i = 0; i < _gridReporte.Columns.Count; i++)
                {
                    ws.Cell(1, i + 1).Value = _gridReporte.Columns[i].HeaderText;
                    ws.Cell(1, i + 1).Style.Font.Bold = true;
                    ws.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
                }

                // Data
                for (int i = 0; i < _gridReporte.Rows.Count; i++)
                {
                    for (int j = 0; j < _gridReporte.Columns.Count; j++)
                    {
                        var val = _gridReporte.Rows[i].Cells[j].Value?.ToString() ?? "";
                        ws.Cell(i + 2, j + 1).Value = val;
                    }
                }

                // Resumen
                int resumenRow = _gridReporte.Rows.Count + 3;
                ws.Cell(resumenRow, 1).Value = "RESUMEN:";
                ws.Cell(resumenRow, 1).Style.Font.Bold = true;
                ws.Cell(resumenRow, 2).Value = _lblResumenContenido.Text;
                
                ws.Columns().AdjustToContents();

                wb.SaveAs(sfd.FileName);
                NotificationManager.ShowSuccess("Éxito", "Reporte descargado exitosamente.", this);
            }
            catch (Exception ex)
            {
                NotificationManager.ShowError("Error", $"Error al guardar Excel: {ex.Message}", this);
            }
        }
    }

    private void BtnDescargarCsv_Click(object? sender, EventArgs e)
    {
        if (_gridReporte.Rows.Count == 0) return;

        using SaveFileDialog sfd = new SaveFileDialog();
        sfd.Filter = "CSV File|*.csv";
        sfd.Title = "Guardar Reporte CSV";
        sfd.FileName = $"Reporte_Atenciones_{DateTime.Now:yyyyMMdd_HHmm}.csv";

        if (sfd.ShowDialog() == DialogResult.OK)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                // Headers
                var headers = _gridReporte.Columns.Cast<DataGridViewColumn>().Select(c => $"\"{c.HeaderText}\"");
                sb.AppendLine(string.Join(",", headers));

                // Data
                foreach (DataGridViewRow row in _gridReporte.Rows)
                {
                    var cells = row.Cells.Cast<DataGridViewCell>().Select(c => $"\"{c.Value?.ToString()?.Replace("\"", "\"\"")}\"");
                    sb.AppendLine(string.Join(",", cells));
                }

                // Resumen
                sb.AppendLine();
                sb.AppendLine($"\"RESUMEN:\",\"{_lblResumenContenido.Text.Replace("\"", "\"\"")}\"");

                File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                NotificationManager.ShowSuccess("Éxito", "Reporte descargado exitosamente.", this);
            }
            catch (Exception ex)
            {
                NotificationManager.ShowError("Error", $"Error al guardar CSV: {ex.Message}", this);
            }
        }
    }
}
