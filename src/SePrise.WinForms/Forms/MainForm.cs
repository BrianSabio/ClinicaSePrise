using SePrise.WinForms.Services;
using SePrise.WinForms.Forms.Pacientes;
using SePrise.WinForms.UX;

namespace SePrise.WinForms.Forms;

public partial class MainForm : Form
{
    private readonly AuthService _authService;
    private Panel _sidebarPanel = null!;
    private Panel _contentPanel = null!;
    private Label _lblUsuario = null!;
    private Label _lblEstado = null!;
    private Button _btnToggleTheme = null!;

    public MainForm(AuthService authService)
    {
        _authService = authService;
        InitializeComponentCustom();
        ApplyTheme();
    }

    private void InitializeComponentCustom()
    {
        this.Text = "SePrise - Sistema de Gestión Clínica";
        this.Size = new Size(1100, 700);
        this.StartPosition = FormStartPosition.CenterScreen;
        // SIN IsMdiContainer — el panel Fill bloqueaba todos los subformularios
        this.Font = new Font("Segoe UI", 10);
        this.MinimumSize = new Size(900, 600);
        _sidebarPanel = new Panel
        {
            Dock = DockStyle.Left,
            Width = 260,
            AutoScroll = true
        };

        // Logo / título en sidebar
        var pnlLogoArea = new Panel
        {
            Dock = DockStyle.Top,
            Height = 90,
            Padding = new Padding(15, 15, 15, 5)
        };

        var lblClinica = new Label
        {
            Text = "🏥 CLÍNICA SEPRISE",
            Location = new Point(15, 18),
            Font = new Font("Segoe UI", 13, FontStyle.Bold),
            AutoSize = true
        };
        pnlLogoArea.Controls.Add(lblClinica);

        var lblSistema = new Label
        {
            Text = "Sistema de Gestión",
            Location = new Point(15, 48),
            Font = new Font("Segoe UI", 9, FontStyle.Regular),
            AutoSize = true
        };
        pnlLogoArea.Controls.Add(lblSistema);

        _sidebarPanel.Controls.Add(pnlLogoArea);

        // Separador
        var sep1 = new Panel { Dock = DockStyle.Top, Height = 2, Margin = new Padding(10, 0, 10, 0) };
        _sidebarPanel.Controls.Add(sep1);

        // Etiqueta MENÚ
        var lblMenuTitulo = new Label
        {
            Text = "  NAVEGACIÓN",
            Dock = DockStyle.Top,
            Height = 35,
            Font = new Font("Segoe UI", 8, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft
        };
        _sidebarPanel.Controls.Add(lblMenuTitulo);
        // Se agregan en orden inverso porque Dock=Top los apila de abajo hacia arriba
        
        // Botón Atención
        var btnAtencion = CrearBotonNavegacion("👨‍⚕️  Atención Médica", "Gestionar el flujo de atención");
        btnAtencion.Click += (s, e) => AbrirFormSeguro(() => OpenAtencionForm());
        _sidebarPanel.Controls.Add(btnAtencion);

        // Botón: Reportes (debe ir ANTES de Cerrar sesión en el código, pero ARRIBA en la UI)
        var btnReportes = CrearBotonNavegacion("📊  Reportes", "Generar reportes del sistema");
        btnReportes.Click += (s, e) => AbrirFormSeguro(() => OpenGenerarReporteForm());
        _sidebarPanel.Controls.Add(btnReportes);

        // Botón Acreditación
        var btnAcreditacion = CrearBotonNavegacion("✅  Acreditación", "Acreditar pacientes en recepción");
        btnAcreditacion.Click += (s, e) => AbrirFormSeguro(() => OpenAcreditacionForm());
        _sidebarPanel.Controls.Add(btnAcreditacion);

        // Botón Turnos
        var btnTurnos = CrearBotonNavegacion("📅  Turnos", "Agendar y gestionar turnos");
        btnTurnos.Click += (s, e) => AbrirFormSeguro(() => OpenTurnosForm());
        _sidebarPanel.Controls.Add(btnTurnos);

        // Botón Pacientes (último en lista pero primero visualmente)
        var btnPacientes = CrearBotonNavegacion("👥  Pacientes", "Crear, editar y buscar pacientes");
        btnPacientes.Click += (s, e) => AbrirFormSeguro(() => OpenPacientesForm());
        _sidebarPanel.Controls.Add(btnPacientes);

        // Separador inferior
        var sep2 = new Panel { Dock = DockStyle.Bottom, Height = 2 };
        _sidebarPanel.Controls.Add(sep2);

        // Botón Cerrar Sesión
        var btnLogout = new Button
        {
            Text = "🔒  Cerrar Sesión",
            Dock = DockStyle.Bottom,
            Height = 45,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10),
            Cursor = Cursors.Hand,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(15, 0, 0, 0)
        };
        btnLogout.Click += (s, e) => CerrarSesion();
        _sidebarPanel.Controls.Add(btnLogout);
        var statusPanel = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 45,
        };

        _lblUsuario = new Label
        {
            Text = $"👤 Usuario: {_authService.UsuarioDNI}",
            Location = new Point(15, 14),
            AutoSize = true,
            Font = new Font("Segoe UI", 9, FontStyle.Regular)
        };
        statusPanel.Controls.Add(_lblUsuario);

        var _pnlEstadoDot = new Panel
        {
            Location = new Point(220, 18),
            Width = 12,
            Height = 12
        };
        _pnlEstadoDot.Paint += (s, e) =>
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using var brush = new SolidBrush(Color.MediumSeaGreen); // Verde para conectado
            e.Graphics.FillEllipse(brush, 0, 0, 10, 10);
        };
        statusPanel.Controls.Add(_pnlEstadoDot);

        _lblEstado = new Label
        {
            Text = "Conectado",
            Location = new Point(235, 14),
            AutoSize = true,
            Font = new Font("Segoe UI", 9, FontStyle.Regular)
        };
        statusPanel.Controls.Add(_lblEstado);

        _btnToggleTheme = new Button
        {
            Text = "🌓 Cambiar Tema",
            Location = new Point(400, 8),
            Width = 130,
            Height = 30,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9),
            Cursor = Cursors.Hand
        };
        _btnToggleTheme.Click += (s, e) =>
        {
            ThemeManager.ToggleTheme();
            ThemeManager.SaveThemePreference();
            ApplyTheme();
        };
        statusPanel.Controls.Add(_btnToggleTheme);
        _contentPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(40),
            AutoScroll = true
        };
        var pnlHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 100
        };

        var lblSubtitulo = new Label
        {
            Text = "Seleccione una opción del menú lateral para comenzar.",
            Font = new Font("Segoe UI", 11, FontStyle.Regular),
            TextAlign = ContentAlignment.TopCenter,
            Dock = DockStyle.Top,
            Height = 40
        };

        var lblBienvenida = new Label
        {
            Text = "Bienvenido al Sistema de Gestión Clínica",
            Font = new Font("Segoe UI", 20, FontStyle.Bold),
            TextAlign = ContentAlignment.BottomCenter,
            Dock = DockStyle.Top,
            Height = 60,
            Padding = new Padding(0, 0, 0, 5)
        };

        // En WinForms, el último Dock=Top agregado se empuja hacia abajo.
        // Añadimos primero el subtítulo, y luego el título (este quedará más arriba).
        pnlHeader.Controls.Add(lblSubtitulo);
        pnlHeader.Controls.Add(lblBienvenida);

        _contentPanel.Controls.Add(pnlHeader);

        // Panel de accesos rápidos
        var pnlAccesos = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            Padding = new Padding(20),
            WrapContents = true,
            AutoScroll = true
        };

        pnlAccesos.Controls.Add(CrearTarjetaAcceso("👥 Pacientes", "Gestionar pacientes del sistema",
            () => AbrirFormSeguro(() => OpenPacientesForm())));
        pnlAccesos.Controls.Add(CrearTarjetaAcceso("📅 Turnos", "Agendar y administrar turnos",
            () => AbrirFormSeguro(() => OpenTurnosForm())));
        pnlAccesos.Controls.Add(CrearTarjetaAcceso("✅ Acreditación", "Acreditar pacientes en recepción",
            () => AbrirFormSeguro(() => OpenAcreditacionForm())));
        pnlAccesos.Controls.Add(CrearTarjetaAcceso("👨‍⚕️ Atención", "Gestionar flujo de atención médica",
            () => AbrirFormSeguro(() => OpenAtencionForm())));
        pnlAccesos.Controls.Add(CrearTarjetaAcceso("📊 Reportes", "Generar reportes del sistema",
            () => AbrirFormSeguro(() => OpenGenerarReporteForm())));

        _contentPanel.Controls.Add(pnlAccesos);
        pnlAccesos.BringToFront(); // Garantiza que pnlHeader (Top) dockee ANTES que pnlAccesos (Fill)

        // Orden de agregado: primero status (bottom), luego sidebar (left), luego content (fill)
        this.Controls.Add(_contentPanel);
        this.Controls.Add(_sidebarPanel);
        this.Controls.Add(statusPanel);

        ThemeManager.ThemeChanged += (s, e) => ApplyTheme();
    }
    private static Button CrearBotonNavegacion(string texto, string tooltip)
    {
        var btn = new Button
        {
            Text = texto,
            Dock = DockStyle.Top,
            Height = 50,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Regular),
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(15, 0, 0, 0),
            Cursor = Cursors.Hand
        };
        btn.FlatAppearance.BorderSize = 0;
        btn.AddTooltip(tooltip);
        return btn;
    }
    private static Panel CrearTarjetaAcceso(string titulo, string descripcion, Action onClick)
    {
        var tarjeta = new Panel
        {
            Size = new Size(220, 130),
            Margin = new Padding(15),
            Cursor = Cursors.Hand,
            BorderStyle = BorderStyle.FixedSingle
        };

        var lblIconoTitulo = new Label
        {
            Text = titulo,
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            Location = new Point(15, 20),
            Size = new Size(190, 30),
            TextAlign = ContentAlignment.MiddleLeft
        };

        var lblDesc = new Label
        {
            Text = descripcion,
            Font = new Font("Segoe UI", 8, FontStyle.Regular),
            Location = new Point(15, 55),
            Size = new Size(190, 40),
            TextAlign = ContentAlignment.TopLeft
        };

        tarjeta.Controls.Add(lblIconoTitulo);
        tarjeta.Controls.Add(lblDesc);

        // Hacer que el click en la tarjeta y en sus labels funcionen
        tarjeta.Click += (s, e) => onClick();
        lblIconoTitulo.Click += (s, e) => onClick();
        lblDesc.Click += (s, e) => onClick();

        return tarjeta;
    }
    private void AbrirFormSeguro(Action abrirForm)
    {
        try
        {
            abrirForm();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"No se pudo abrir el módulo:\n\n{ex.Message}",
                "Error al abrir módulo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void ApplyTheme()
    {
        var colors = ThemeManager.CurrentColorScheme;

        this.BackColor = colors.BackgroundColor;
        this.ForeColor = colors.TextPrimary;

        _sidebarPanel.BackColor = colors.SurfaceColor;
        _contentPanel.BackColor = colors.BackgroundColor;

        // Aplicar colores a botones de navegación en sidebar
        foreach (Control ctrl in _sidebarPanel.Controls)
        {
            if (ctrl is Button btn)
            {
                btn.BackColor = colors.SurfaceColor;
                btn.ForeColor = colors.TextPrimary;
                btn.FlatAppearance.MouseOverBackColor = colors.HoverColor;
                btn.FlatAppearance.MouseDownBackColor = colors.PrimaryLight;
                btn.FlatAppearance.BorderColor = colors.SurfaceColor;
            }
            else if (ctrl is Label lbl)
            {
                lbl.BackColor = colors.SurfaceColor;
                lbl.ForeColor = colors.TextSecondary;
            }
            else if (ctrl is Panel sep)
            {
                sep.BackColor = colors.BorderColor;
            }
        }

        // Tarjetas del área central
        foreach (Control ctrl in _contentPanel.Controls)
        {
            if (ctrl is Label lbl)
            {
                lbl.BackColor = colors.BackgroundColor;
                lbl.ForeColor = colors.TextPrimary;
            }
            else if (ctrl is FlowLayoutPanel flow)
            {
                flow.BackColor = colors.BackgroundColor;
                foreach (Control tarjeta in flow.Controls)
                {
                    tarjeta.BackColor = colors.SurfaceColor;
                    tarjeta.ForeColor = colors.TextPrimary;
                    foreach (Control inner in tarjeta.Controls)
                    {
                        inner.BackColor = colors.SurfaceColor;
                        inner.ForeColor = inner is Label ? colors.TextSecondary : colors.TextPrimary;
                    }
                }
            }
            else if (ctrl is Panel pnlHeader)
            {
                pnlHeader.BackColor = colors.BackgroundColor;
                foreach (Control inner in pnlHeader.Controls)
                {
                    if (inner is Label lblHeader)
                    {
                        lblHeader.BackColor = colors.BackgroundColor;
                        lblHeader.ForeColor = colors.TextPrimary;
                    }
                }
            }
        }

        _lblEstado.ForeColor = colors.SuccessColor;
        ThemeApplier.ApplyThemeToForm(this);
    }

    private void CerrarSesion()
    {
        var resultado = MessageBox.Show(
            "¿Desea cerrar sesión?",
            "Cerrar Sesión",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (resultado == DialogResult.Yes)
        {
            _authService.Logout();
            this.Close();
        }
    }

    private void OpenPacientesForm()
    {
        using var form = new PacientesForm(Program.PacienteService);
        form.StartPosition = FormStartPosition.CenterParent;
        form.ShowDialog(this);
    }

    private void OpenTurnosForm()
    {
        using var form = new SePrise.WinForms.Forms.Turnos.TurnosForm(
            Program.TurnoService, Program.PacienteService, Program.MedicoService,
            Program.EspecialidadService, Program.SalaService);
        form.StartPosition = FormStartPosition.CenterParent;
        form.ShowDialog(this);
    }

    private void OpenAcreditacionForm()
    {
        using var form = new SePrise.WinForms.Forms.Acreditacion.AcreditacionForm(
            Program.AtencionService, Program.TurnoService, Program.PacienteService,
            Program.EspecialidadService, Program.MedicoService);
        form.StartPosition = FormStartPosition.CenterParent;
        form.ShowDialog(this);
    }

    private void OpenAtencionForm()
    {
        using var form = new SePrise.WinForms.Forms.Atencion.AtencionForm(Program.AtencionService);
        form.StartPosition = FormStartPosition.CenterParent;
        form.ShowDialog(this);
    }

    private void OpenGenerarReporteForm()
    {
        using var form = new SePrise.WinForms.Forms.Reportes.GenerarReporteForm(
            Program.ReportesService, Program.MedicoService, Program.EspecialidadService);
        form.StartPosition = FormStartPosition.CenterParent;
        form.ShowDialog(this);
    }
}


