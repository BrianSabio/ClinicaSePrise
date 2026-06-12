$utf8NoBom = New-Object System.Text.UTF8Encoding $false

$p1 = "c:\Users\BSUb2\AndroidStudioProjects\SePrise\src\SePrise.WinForms\Forms\Turnos\EditarEstadoTurnoForm.cs"
$l1 = [System.IO.File]::ReadAllLines($p1)
$l1_new = $l1[0..193] + "    }" + $l1[198..($l1.Length - 1)]
[System.IO.File]::WriteAllLines($p1, $l1_new, $utf8NoBom)

$p2 = "c:\Users\BSUb2\AndroidStudioProjects\SePrise\src\SePrise.WinForms\Forms\Acreditacion\AcreditacionForm.cs"
$l2 = [System.IO.File]::ReadAllLines($p2)
$l2[183] = "        ThemeApplier.ApplyThemeToForm(this);" + [Environment]::NewLine + "    }"
$l2_new = $l2[0..339]
[System.IO.File]::WriteAllLines($p2, $l2_new, $utf8NoBom)

$p3 = "c:\Users\BSUb2\AndroidStudioProjects\SePrise\src\SePrise.WinForms\Forms\Atencion\AtencionForm.cs"
$l3 = [System.IO.File]::ReadAllLines($p3)
$l3[134] = "        ThemeApplier.ApplyThemeToForm(this);" + [Environment]::NewLine + "    }"
$l3_new = $l3[0..305] + "}"
[System.IO.File]::WriteAllLines($p3, $l3_new, $utf8NoBom)

$p4 = "c:\Users\BSUb2\AndroidStudioProjects\SePrise\src\SePrise.WinForms\Forms\Pacientes\EditarPacienteForm.cs"
$l4 = [System.IO.File]::ReadAllLines($p4)
$l4_new = $l4[0..154] + "    }" + $l4[158..($l4.Length - 1)]
[System.IO.File]::WriteAllLines($p4, $l4_new, $utf8NoBom)

$p5 = "c:\Users\BSUb2\AndroidStudioProjects\SePrise\src\SePrise.WinForms\Forms\Turnos\TurnosForm.cs"
$l5 = [System.IO.File]::ReadAllLines($p5)
$l5_new = $l5[0..129] + "    }" + $l5[182..($l5.Length - 1)]
[System.IO.File]::WriteAllLines($p5, $l5_new, $utf8NoBom)
