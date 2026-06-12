using SePrise.WinForms.Models;
using SePrise.WinForms.Services;
using SePrise.WinForms.UX;

namespace SePrise.WinForms.Forms.Turnos;

public partial class EditarEstadoTurnoForm : Form
{
    private readonly TurnoService _turnoService;
    private readonly TurnoDTO _turnoOriginal;

    private DateTimePicker _dtpFecha = null!;
    private DateTimePicker _dtpHora = null!;
    private NumericUpDown _numDuracion = null!;

    private ModernButton _btnReprogramar = null!;
    private ModernButton _btnCancelar = null!;
    private ErrorLabel _lblError = null!;
    private Label _lblTitle = null!;

    public EditarEstadoTurnoForm(TurnoService turnoService, TurnoDTO turno)
    {
        _turnoService = turnoService;
        _turnoOriginal = turno;

        InitializeComponentCustom();
        ApplyTheme();
    }

    private void InitializeComponentCustom()
    {
        this.Text = "Reprogramar Turno";
        this.Size = new Size(500, 600);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Font = new Font("Segoe UI", 10);

        // ========== TITLE ==========
        _lblTitle = new Label
        {
            Text = $"Reprogramar Turno #{_turnoOriginal.IdTurno}",
            Location = new Point(30, 20),
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            AutoSize = true
        };
        this.Controls.Add(_lblTitle);

        int y = 70;
        const int spacing = 65;

        // ========== INFO PANEL (READ-ONLY) ==========
        var pnlInfo = new ModernPanel
        {
            Location = new Point(30, y),
            Size = new Size(440, 140),
            BorderStyle = BorderStyle.FixedSingle
        };

        int infoY = 10;
        pnlInfo.Controls.Add(new Label
        {
            Text = $"📋 Turno #{_turnoOriginal.IdTurno}",
            Location = new Point(10, infoY),
            AutoSize = true,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        });
        infoY += 30;

        pnlInfo.Controls.Add(new Label
        {
            Text = $"👤 Paciente: {_turnoOriginal.PacienteNombre}",
            Location = new Point(10, infoY),
            AutoSize = true,
            Font = new Font("Segoe UI", 9)
        });
        infoY += 25;

        pnlInfo.Controls.Add(new Label
        {
            Text = $"👨‍⚕️ Médico: {_turnoOriginal.MedicoNombre}",
            Location = new Point(10, infoY),
            AutoSize = true,
            Font = new Font("Segoe UI", 9)
        });
        infoY += 25;

        pnlInfo.Controls.Add(new Label
        {
            Text = $"🔄 Estado: {_turnoOriginal.Estado}",
            Location = new Point(10, infoY),
            AutoSize = true,
            Font = new Font("Segoe UI", 9, FontStyle.Bold)
        });

        this.Controls.Add(pnlInfo);
        y += 160;

        // ========== NEW DATE ==========
        this.Controls.Add(new Label { Text = "📅 Nueva Fecha:", Location = new Point(30, y), AutoSize = true, Font = new Font("Segoe UI", 10) });
        _dtpFecha = new DateTimePicker
        {
            Location = new Point(30, y + 25),
            Width = 440,
            Height = 35,
            Format = DateTimePickerFormat.Short
        };
        _dtpFecha.MinDate = DateTime.Today;
        _dtpFecha.Value = _turnoOriginal.FechaHoraInicio.Date >= DateTime.Today ? _turnoOriginal.FechaHoraInicio.Date : DateTime.Today;
        this.Controls.Add(_dtpFecha);
        y += spacing;

        // ========== NEW TIME ==========
        this.Controls.Add(new Label { Text = "⏰ Nueva Hora:", Location = new Point(30, y), AutoSize = true, Font = new Font("Segoe UI", 10) });
        _dtpHora = new DateTimePicker
        {
            Location = new Point(30, y + 25),
            Width = 440,
            Height = 35,
            Format = DateTimePickerFormat.Time,
            ShowUpDown = true
        };
        _dtpHora.Value = _turnoOriginal.FechaHoraInicio;
        this.Controls.Add(_dtpHora);
        y += spacing;

        // ========== DURATION ==========
        this.Controls.Add(new Label { Text = "⏱️ Duración (minutos):", Location = new Point(30, y), AutoSize = true, Font = new Font("Segoe UI", 10) });
        _numDuracion = new NumericUpDown
        {
            Location = new Point(30, y + 25),
            Width = 440,
            Height = 35,
            Minimum = 5,
            Maximum = 120,
            Value = _turnoOriginal.DuracionMinutos,
            Font = new Font("Segoe UI", 10)
        };
        this.Controls.Add(_numDuracion);
        y += spacing;

        // ========== ERROR LABEL ==========
        _lblError = new ErrorLabel
        {
            Location = new Point(30, y),
            Width = 440,
            AutoSize = false,
            Visible = false
        };
        this.Controls.Add(_lblError);
        y += 50;

        // ========== BUTTONS ==========
        var pnlBotones = new Panel
        {
            Location = new Point(30, y),
            Size = new Size(440, 50)
        };

        _btnReprogramar = new ModernButton
        {
            Text = "📝 Reprogramar",
            Location = new Point(0, 0),
            Width = 210,
            Height = 40
        };
        _btnReprogramar.Click += BtnReprogramar_Click;
        _btnReprogramar.AddTooltip("Guardar la nueva programación");
        pnlBotones.Controls.Add(_btnReprogramar);

        _btnCancelar = new ModernButton
        {
            Text = "❌ Cancelar",
            Location = new Point(220, 0),
            Width = 210,
            Height = 40
        };
        _btnCancelar.Click += (s, e) => this.Close();
        _btnCancelar.AddTooltip("Cancelar reprogramación");
        pnlBotones.Controls.Add(_btnCancelar);

        this.Controls.Add(pnlBotones);

        // ========== THEME SUPPORT ==========
        ThemeManager.ThemeChanged += (s, e) => ApplyTheme();
    }

    private void ApplyTheme()
    {
        var colors = ThemeManager.CurrentColorScheme;
        this.BackColor = colors.BackgroundColor;
        this.ForeColor = colors.TextPrimary;
        ThemeApplier.ApplyThemeToForm(this);
    }

    private async void BtnReprogramar_Click(object? sender, EventArgs e)
    {
        _lblError.Visible = false;

        var fechaSeleccionada = _dtpFecha.Value.Date + _dtpHora.Value.TimeOfDay;
        if (fechaSeleccionada <= DateTime.Now)
        {
            MostrarError("La fecha y hora deben ser futuras.");
            return;
        }

        DeshabilitarBotones();

        try
        {
            await _turnoService.ReprogramarAsync(_turnoOriginal.IdTurno, fechaSeleccionada, (int)_numDuracion.Value);
            MessageBox.Show("Turno reprogramado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("409"))
            {
                MostrarError("Médico/Sala ocupada o error de estado.");
            }
            else
            {
                MostrarError(ex.Message);
            }
        }
        finally
        {
            HabilitarBotones();
        }
    }

    private void MostrarError(string mensaje)
    {
        _lblError.Text = mensaje;
        _lblError.Visible = true;
    }

    private void DeshabilitarBotones()
    {
        _btnReprogramar.Enabled = false;
        _btnCancelar.Enabled = false;
    }

    private void HabilitarBotones()
    {
        _btnReprogramar.Enabled = true;
        _btnCancelar.Enabled = true;
    }
}
