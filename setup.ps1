<#
.SYNOPSIS
  Configura la base de datos de la practica y verifica que todo funcione.

.DESCRIPTION
  1. Comprueba que sqlcmd y dotnet esten instalados.
  2. Crea la base (01_CrearBaseDatos.sql) o la reinicia (-Reiniciar).
  3. Verifica la base: existencia, columnas de dbo.productos y datos semilla.
  4. Compila el proyecto.
  5. Prueba de humo: ejecuta el menu con entradas automaticas (opciones 2 y 3)
     y comprueba que se insertaron filas y que la demo de inyeccion responde.

.EXAMPLE
  .\setup.ps1
  .\setup.ps1 -Reiniciar
  .\setup.ps1 -Servidor ".\SQLEXPRESS"
#>
param(
    [string]$Servidor = "Mateo",
    [switch]$Reiniciar
)

$ErrorActionPreference = "Stop"
$raiz      = $PSScriptRoot
$proyecto  = Join-Path $raiz "Practica28-9-2026"
$scripts   = Join-Path $proyecto "BaseDatos"
$baseDatos = "practica28_productosdb"
$fallos    = 0

function Paso($texto) { Write-Host "`n==> $texto" -ForegroundColor Cyan }
function Ok($texto)   { Write-Host "  [OK]    $texto" -ForegroundColor Green }
function Falla($texto){ Write-Host "  [FALLO] $texto" -ForegroundColor Red; $script:fallos++ }

# Ejecuta una consulta y devuelve el primer valor (sin encabezados).
function Escalar($consulta) {
    $r = & sqlcmd -S $Servidor -E -C -b -h -1 -W -d $baseDatos -Q "SET NOCOUNT ON; $consulta"
    if ($LASTEXITCODE -ne 0) { throw "sqlcmd fallo con la consulta: $consulta" }
    return ($r | Where-Object { $_ -ne "" } | Select-Object -First 1).Trim()
}

# ---------------------------------------------------------------- 1
Paso "Comprobando herramientas"
foreach ($cmd in "sqlcmd", "dotnet") {
    if (Get-Command $cmd -ErrorAction SilentlyContinue) { Ok "$cmd encontrado" }
    else { Falla "$cmd no esta instalado o no esta en el PATH"; exit 1 }
}

$constante = Select-String -Path (Join-Path $proyecto "ConexionBD.cs") -Pattern 'Servidor = "(.+)"' |
             ForEach-Object { $_.Matches[0].Groups[1].Value.Replace('\\', '\') }
if ($constante -ne $Servidor) {
    Write-Host "  [AVISO] ConexionBD.Servidor es '$constante' pero se uso -Servidor '$Servidor'." -ForegroundColor Yellow
    Write-Host "          La aplicacion se conectara a '$constante'. Ajusta ConexionBD.cs si no es lo que quieres." -ForegroundColor Yellow
}

# ---------------------------------------------------------------- 2
$archivo = if ($Reiniciar) { "02_ReiniciarBaseDatos.sql" } else { "01_CrearBaseDatos.sql" }
Paso "Ejecutando $archivo en $Servidor"
& sqlcmd -S $Servidor -E -C -b -i (Join-Path $scripts $archivo)
if ($LASTEXITCODE -ne 0) { Falla "El script $archivo termino con errores"; exit 1 }
Ok "$archivo ejecutado"

# ---------------------------------------------------------------- 3
Paso "Verificando la base de datos"
$columnas = Escalar "SELECT STRING_AGG(COLUMN_NAME + ':' + DATA_TYPE, ',') WITHIN GROUP (ORDER BY ORDINAL_POSITION) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'productos'"
$esperado = "id:int,nombre:nvarchar,precio:decimal,cantidad:int,imagen:varbinary"
if ($columnas -eq $esperado) { Ok "dbo.productos tiene las columnas esperadas" }
else { Falla "Columnas inesperadas: '$columnas' (se esperaba '$esperado')" }

$semilla = [int](Escalar "SELECT COUNT(*) FROM dbo.productos WHERE nombre IN (N'Teclado', N'Mouse', N'Monitor')")
if ($semilla -eq 3) { Ok "Los 3 productos semilla estan presentes" }
else { Falla "Se encontraron $semilla de 3 productos semilla" }

$antes = [int](Escalar "SELECT COUNT(*) FROM dbo.productos")
Ok "Filas actuales en dbo.productos: $antes"

# ---------------------------------------------------------------- 4
Paso "Compilando el proyecto"
& dotnet build $proyecto -c Debug --nologo -v q
if ($LASTEXITCODE -ne 0) { Falla "La compilacion fallo"; exit 1 }
Ok "Compilacion correcta"

# ---------------------------------------------------------------- 5
Paso "Prueba de humo de la aplicacion"
# Opcion 2 -> insertar 2 productos -> ENTER
# Opcion 3 -> buscar el payload ' OR '1'='1 -> ENTER
# Opcion 0 -> salir
$entrada = @("2", "2", "", "3", "' OR '1'='1", "", "0") -join "`n"
$salida  = ($entrada | & dotnet run --project $proyecto --no-build 2>&1) -join "`n"

if ($salida -match "2 producto\(s\) insertado\(s\)") { Ok "Opcion 2: inserto 2 productos con la sentencia preparada" }
else { Falla "Opcion 2 no reporto la insercion" }

$despues = [int](Escalar "SELECT COUNT(*) FROM dbo.productos")
if ($despues -eq $antes + 2) { Ok "La tabla paso de $antes a $despues filas" }
else { Falla "Se esperaban $($antes + 2) filas y hay $despues" }

if ($salida -match "la inyeccion tuvo exito") { Ok "Opcion 3: la consulta vulnerable devolvio todas las filas" }
else { Falla "Opcion 3: la forma vulnerable no mostro la inyeccion" }

if ($salida -match "Sin coincidencias") { Ok "Opcion 3: la consulta parametrizada bloqueo el payload" }
else { Falla "Opcion 3: la forma segura no respondio como se esperaba" }

if ($salida -match "Error \d+ de SQL Server") {
    Falla "La aplicacion reporto un error de SQL Server:"
    ($salida -split "`n" | Select-String "Error \d+ de SQL Server") | ForEach-Object { Write-Host "          $_" }
}

# ---------------------------------------------------------------- Resumen
Write-Host ""
if ($fallos -eq 0) {
    Write-Host "Todo funciona. Base '$baseDatos' lista en '$Servidor'." -ForegroundColor Green
    Write-Host "Las 2 filas de prueba quedaron insertadas; usa .\setup.ps1 -Reiniciar para volver al estado inicial."
} else {
    Write-Host "$fallos comprobacion(es) fallaron. Revisa los mensajes de arriba." -ForegroundColor Red
    Write-Host "`n----- Salida completa de la aplicacion -----"
    Write-Host $salida
    exit 1
}
