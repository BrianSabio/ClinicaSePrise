namespace SePrise.WinForms.UX;
public class ColorScheme
{
    public Color PrimaryColor { get; set; }
    public Color PrimaryDark { get; set; }
    public Color PrimaryLight { get; set; }
    public Color SecondaryColor { get; set; }
    public Color SecondaryDark { get; set; }
    public Color BackgroundColor { get; set; }
    public Color BackgroundAlt { get; set; }
    public Color SurfaceColor { get; set; }
    public Color TextPrimary { get; set; }
    public Color TextSecondary { get; set; }
    public Color TextDisabled { get; set; }
    public Color SuccessColor { get; set; }
    public Color ErrorColor { get; set; }
    public Color WarningColor { get; set; }
    public Color InfoColor { get; set; }
    public Color BorderColor { get; set; }
    public Color BorderLight { get; set; }
    public Color BorderDark { get; set; }
    public Color HoverColor { get; set; }
    public Color FocusColor { get; set; }
    public Color DisabledColor { get; set; }
    public Color PlaceholderColor { get; set; }
    public static ColorScheme CreateLightTheme()
    {
        return new ColorScheme
        {
            // Primary - Azul profesional
            PrimaryColor = Color.FromArgb(0, 102, 204),      // #0066CC
            PrimaryDark = Color.FromArgb(0, 82, 164),        // #0052A4
            PrimaryLight = Color.FromArgb(230, 241, 255),    // #E6F1FF

            // Secondary - Verde de éxito
            SecondaryColor = Color.FromArgb(76, 175, 80),    // #4CAF50
            SecondaryDark = Color.FromArgb(56, 142, 60),     // #388E3C

            // Fondos
            BackgroundColor = Color.White,                    // #FFFFFF
            BackgroundAlt = Color.FromArgb(248, 248, 250),   // #F8F8FA
            SurfaceColor = Color.FromArgb(245, 245, 247),    // #F5F5F7

            // Texto
            TextPrimary = Color.FromArgb(33, 33, 33),        // #212121
            TextSecondary = Color.FromArgb(117, 117, 117),   // #757575
            TextDisabled = Color.FromArgb(189, 189, 189),    // #BDBDBD

            // Estados
            SuccessColor = Color.FromArgb(76, 175, 80),      // #4CAF50
            ErrorColor = Color.FromArgb(244, 67, 54),        // #F44336
            WarningColor = Color.FromArgb(255, 152, 0),      // #FF9800
            InfoColor = Color.FromArgb(33, 150, 243),        // #2196F3

            // Bordes
            BorderColor = Color.FromArgb(224, 224, 224),     // #E0E0E0
            BorderLight = Color.FromArgb(238, 238, 238),     // #EEEEEE
            BorderDark = Color.FromArgb(189, 189, 189),      // #BDBDBD

            // Especiales
            HoverColor = Color.FromArgb(245, 245, 245),      // #F5F5F5
            FocusColor = Color.FromArgb(0, 102, 204),        // #0066CC
            DisabledColor = Color.FromArgb(224, 224, 224),   // #E0E0E0
            PlaceholderColor = Color.FromArgb(158, 158, 158) // #9E9E9E
        };
    }
    public static ColorScheme CreateDarkTheme()
    {
        return new ColorScheme
        {
            // Primary - Azul brillante para dark mode
            PrimaryColor = Color.FromArgb(100, 181, 246),     // #64B5F6
            PrimaryDark = Color.FromArgb(66, 165, 245),       // #42A5F5
            PrimaryLight = Color.FromArgb(25, 80, 150),       // #195096

            // Secondary - Verde ajustado
            SecondaryColor = Color.FromArgb(165, 214, 167),   // #A5D6A7
            SecondaryDark = Color.FromArgb(129, 199, 132),    // #81C784

            // Fondos
            BackgroundColor = Color.FromArgb(33, 33, 33),     // #212121
            BackgroundAlt = Color.FromArgb(48, 48, 48),       // #303030
            SurfaceColor = Color.FromArgb(66, 66, 66),        // #424242

            // Texto
            TextPrimary = Color.FromArgb(229, 229, 229),      // #E5E5E5
            TextSecondary = Color.FromArgb(189, 189, 189),    // #BDBDBD
            TextDisabled = Color.FromArgb(97, 97, 97),        // #616161

            // Estados
            SuccessColor = Color.FromArgb(165, 214, 167),     // #A5D6A7
            ErrorColor = Color.FromArgb(239, 154, 154),       // #EF9A9A
            WarningColor = Color.FromArgb(255, 204, 129),     // #FFCC81
            InfoColor = Color.FromArgb(144, 202, 249),        // #90CAF9

            // Bordes
            BorderColor = Color.FromArgb(66, 66, 66),         // #424242
            BorderLight = Color.FromArgb(97, 97, 97),         // #616161
            BorderDark = Color.FromArgb(48, 48, 48),          // #303030

            // Especiales
            HoverColor = Color.FromArgb(66, 66, 66),          // #424242
            FocusColor = Color.FromArgb(100, 181, 246),       // #64B5F6
            DisabledColor = Color.FromArgb(48, 48, 48),       // #303030
            PlaceholderColor = Color.FromArgb(117, 117, 117)  // #757575
        };
    }
}


