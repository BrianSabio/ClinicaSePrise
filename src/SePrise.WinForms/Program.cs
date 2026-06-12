using SePrise.WinForms.Forms;
using SePrise.WinForms.Services;
using SePrise.WinForms.UX;

namespace SePrise.WinForms;

static class Program
{
    public static ApiClient ApiClient { get; private set; } = null!;
    public static AuthService AuthService { get; private set; } = null!;
    public static PacienteService PacienteService { get; private set; } = null!;
    public static TurnoService TurnoService { get; private set; } = null!;
    public static MedicoService MedicoService { get; private set; } = null!;
    public static EspecialidadService EspecialidadService { get; private set; } = null!;
    public static SalaService SalaService { get; private set; } = null!;
    public static AtencionService AtencionService { get; private set; } = null!;

    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        // ========== INITIALIZE THEME & ACCESSIBILITY ==========
        // Cargar preferencias guardadas de tema y accesibilidad
        try
        {
            ThemeManager.LoadThemePreference();
            AccessibilityManager.LoadAccessibilityPreference();
        }
        catch
        {
            // Si falla, usar valores por defecto
            ThemeManager.SetTheme(ThemeMode.Light);
            AccessibilityManager.SetAccessibilityLevel(AccessibilityLevel.Normal);
        }

        // ========== INITIALIZE SERVICES ==========
        ApiClient = new ApiClient();
        AuthService = new AuthService(ApiClient);
        PacienteService = new PacienteService(ApiClient);
        TurnoService = new TurnoService(ApiClient);
        MedicoService = new MedicoService(ApiClient);
        EspecialidadService = new EspecialidadService(ApiClient);
        SalaService = new SalaService(ApiClient);
        AtencionService = new AtencionService(ApiClient);

        // ========== RUN APPLICATION ==========
        Application.Run(new LoginForm(AuthService));
    }
}
