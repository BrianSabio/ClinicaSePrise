namespace SePrise.WinForms.UX;

/// <summary>
/// Control visual de carga/progreso global para operaciones asincrónicas.
/// </summary>
public class LoadingIndicator : Control
{
    private System.Windows.Forms.Timer _animationTimer = null!;
    private int _dotCount = 1;

    public LoadingIndicator()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
        SetStyle(ControlStyles.SupportsTransparentBackColor, true);

        this.Height = 50;
        this.Width = 150;
        this.BackColor = Color.Transparent;
        this.ForeColor = ThemeManager.CurrentColorScheme.PrimaryColor;

        _animationTimer = new System.Windows.Forms.Timer();
        _animationTimer.Interval = 300;
        _animationTimer.Tick += (s, e) =>
        {
            _dotCount = (_dotCount % 3) + 1;
            this.Invalidate();
        };

        ThemeManager.ThemeChanged += (s, e) =>
        {
            ForeColor = e.ColorScheme.PrimaryColor;
            Invalidate();
        };
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        var text = "Cargando" + new string('.', _dotCount);
        using (var brush = new SolidBrush(ForeColor))
        using (var font = new Font("Segoe UI", 10, FontStyle.Regular))
        {
            e.Graphics.DrawString(text, font, brush, 0, 15);
        }
    }

    public void Start()
    {
        _animationTimer.Start();
        this.Visible = true;
    }

    public void Stop()
    {
        _animationTimer.Stop();
        this.Visible = false;
        _dotCount = 1;
    }

    protected override void Dispose(bool disposing)
    {
        _animationTimer?.Dispose();
        base.Dispose(disposing);
    }
}

/// <summary>
/// Panel semi-transparente que bloquea interacción durante carga.
/// </summary>
public class LoadingOverlay : Panel
{
    private Label _lblMessage = null!;

    public LoadingOverlay()
    {
        this.Dock = DockStyle.Fill;
        this.BackColor = Color.Black;

        _lblMessage = new Label
        {
            Text = "Procesando...",
            AutoSize = true,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            BackColor = Color.Transparent
        };

        this.Controls.Add(_lblMessage);
        this.Visible = false;
    }

    public void Show(string message = "Procesando...")
    {
        _lblMessage.Text = message;
        this.Visible = true;
        this.BringToFront();
    }

    public new void Hide()
    {
        this.Visible = false;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        // Dibujar spinner simple
        var centerX = this.Width / 2;
        var centerY = this.Height / 2;

        using (var pen = new Pen(Color.White, 3))
        {
            e.Graphics.DrawEllipse(pen, centerX - 30, centerY - 30, 60, 60);
        }

        // Centrar el mensaje
        _lblMessage.Location = new Point(centerX - _lblMessage.Width / 2, centerY + 50);
    }
}

/// <summary>
/// Gestor centralizado de indicadores de carga globales.
/// </summary>
public static class LoadingManager
{
    private static LoadingOverlay? _globalOverlay;
    private static int _loadingCount = 0;
    private static readonly object _lockObject = new();

    /// <summary>
    /// Inicializa el gestor de carga para un formulario específico.
    /// </summary>
    public static void Initialize(Form form)
    {
        if (_globalOverlay == null || _globalOverlay.Parent != form)
        {
            _globalOverlay = new LoadingOverlay { Parent = form };
            form.Controls.Add(_globalOverlay);
        }
    }

    /// <summary>
    /// Muestra un indicador de carga global.
    /// </summary>
    public static void Show(string message = "Procesando...")
    {
        lock (_lockObject)
        {
            _loadingCount++;
            if (_globalOverlay != null)
            {
                _globalOverlay.Show(message);
            }
        }
    }

    /// <summary>
    /// Oculta el indicador de carga global (solo si no hay más operaciones pendientes).
    /// </summary>
    public static void Hide()
    {
        lock (_lockObject)
        {
            _loadingCount = Math.Max(0, _loadingCount - 1);
            if (_loadingCount == 0 && _globalOverlay != null)
            {
                _globalOverlay.Hide();
            }
        }
    }

    /// <summary>
    /// Reinicia el contador (para casos de error).
    /// </summary>
    public static void Reset()
    {
        lock (_lockObject)
        {
            _loadingCount = 0;
            _globalOverlay?.Hide();
        }
    }
}

/// <summary>
/// Indicador de progreso en barra para operaciones largas.
/// </summary>
public class ProgressBar2 : Control
{
    private int _progress = 0;
    private string _text = "0%";

    public int Progress
    {
        get => _progress;
        set
        {
            _progress = Math.Max(0, Math.Min(100, value));
            _text = $"{_progress}%";
            this.Invalidate();
        }
    }

    public string? CustomText { get; set; }

    public ProgressBar2()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
        this.Height = 30;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        var colors = ThemeManager.CurrentColorScheme;

        // Fondo
        using (var brush = new SolidBrush(colors.SurfaceColor))
        {
            e.Graphics.FillRectangle(brush, 0, 0, Width, Height);
        }

        // Barra de progreso
        var progressWidth = (Width * _progress) / 100;
        using (var brush = new SolidBrush(colors.SuccessColor))
        {
            e.Graphics.FillRectangle(brush, 0, 0, progressWidth, Height);
        }

        // Borde
        using (var pen = new Pen(colors.BorderColor))
        {
            e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
        }

        // Texto
        var text = CustomText ?? _text;
        using (var brush = new SolidBrush(colors.TextPrimary))
        using (var font = new Font("Segoe UI", 10, FontStyle.Bold))
        {
            var size = e.Graphics.MeasureString(text, font);
            var x = (Width - size.Width) / 2;
            var y = (Height - size.Height) / 2;
            e.Graphics.DrawString(text, font, brush, x, y);
        }
    }
}

/// <summary>
/// Notificación flotante que aparece en la esquina de la pantalla.
/// </summary>
public class ToastNotification : Form
{
    private System.Windows.Forms.Timer _closeTimer = null!;

    public ToastNotification(string title, string message, NotificationType type)
    {
        InitializeStyle();
        BuildUI(title, message, type);
    }

    private void InitializeStyle()
    {
        this.FormBorderStyle = FormBorderStyle.None;
        this.ShowInTaskbar = false;
        this.TopMost = true;
        this.StartPosition = FormStartPosition.Manual;
        this.Size = new Size(350, 100);
        this.Opacity = 0.95;
        this.Padding = new Padding(10);

        var colors = ThemeManager.CurrentColorScheme;
        this.BackColor = colors.SurfaceColor;
        this.ForeColor = colors.TextPrimary;
    }

    private void BuildUI(string title, string message, NotificationType type)
    {
        var colors = ThemeManager.CurrentColorScheme;
        var typeColor = type switch
        {
            NotificationType.Success => colors.SuccessColor,
            NotificationType.Error => colors.ErrorColor,
            NotificationType.Warning => colors.WarningColor,
            NotificationType.Info => colors.InfoColor,
            _ => colors.PrimaryColor
        };

        var pnlColor = new Panel { Dock = DockStyle.Left, Width = 5, BackColor = typeColor };
        this.Controls.Add(pnlColor);

        var pnlContent = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };

        var lblTitle = new Label
        {
            Text = title,
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            ForeColor = colors.TextPrimary,
            AutoSize = true,
            Dock = DockStyle.Top
        };
        pnlContent.Controls.Add(lblTitle);

        var lblMessage = new Label
        {
            Text = message,
            Font = new Font("Segoe UI", 9),
            ForeColor = colors.TextSecondary,
            AutoSize = false,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.TopLeft
        };
        pnlContent.Controls.Add(lblMessage);

        this.Controls.Add(pnlContent);

        _closeTimer = new System.Windows.Forms.Timer();
        _closeTimer.Interval = 4000;
        _closeTimer.Tick += (s, e) =>
        {
            _closeTimer.Stop();
            this.Close();
        };

        this.MouseClick += (s, e) => this.Close();
    }

    public void ShowNotification()
    {
        // Posicionar en esquina inferior derecha
        var screen = Screen.PrimaryScreen;
        if (screen != null)
        {
            this.Location = new Point(screen.WorkingArea.Right - this.Width - 10,
                                       screen.WorkingArea.Bottom - this.Height - 10);
        }

        this.Show();
        _closeTimer.Start();
    }

    protected override void Dispose(bool disposing)
    {
        _closeTimer?.Dispose();
        base.Dispose(disposing);
    }
}

/// <summary>
/// Extensión para mostrar notificaciones flotantes.
/// </summary>
public static class ToastExtensions
{
    /// <summary>
    /// Muestra una notificación flotante en la esquina de la pantalla.
    /// </summary>
    public static void ShowToast(string title, string message, NotificationType type = NotificationType.Info)
    {
        var toast = new ToastNotification(title, message, type);
        toast.ShowNotification();
    }

    /// <summary>
    /// Muestra una notificación flotante de éxito.
    /// </summary>
    public static void ShowSuccessToast(string title, string message)
    {
        ShowToast(title, message, NotificationType.Success);
    }

    /// <summary>
    /// Muestra una notificación flotante de error.
    /// </summary>
    public static void ShowErrorToast(string title, string message)
    {
        ShowToast(title, message, NotificationType.Error);
    }

    /// <summary>
    /// Muestra una notificación flotante de advertencia.
    /// </summary>
    public static void ShowWarningToast(string title, string message)
    {
        ShowToast(title, message, NotificationType.Warning);
    }
}
