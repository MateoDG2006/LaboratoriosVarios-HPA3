# Práctica 28-09-2026

Aplicación de consola en **C# / .NET 10** que reúne los ejemplos de la práctica del 28-09-2026 en un único menú interactivo: sobrecarga de métodos, recursividad, consultas preparadas e inyección SQL contra **SQL Server**, diccionarios y listas, y frecuencias.

## Contenido del repositorio

```
Practica28-9-2026/
├── Practica28-9-2026.slnx            # Solución (formato .slnx)
├── setup.ps1                         # Setup automático + verificación de la base y la app
└── Practica28-9-2026/
    ├── Practica28-9-2026.csproj      # Proyecto (net10.0 + Microsoft.Data.SqlClient)
    ├── Main2.cs                      # Punto de entrada: menú principal
    ├── MetodosSobrecargados.cs       # Opción 1: sobrecarga, factorial y Fibonacci recursivos
    ├── Fibonacci.cs                  #   └─ Fibonacci recursivo
    ├── ConsultasPreparadas.cs        # Opción 2: INSERT con SqlCommand.Prepare()
    ├── InyeccionSQL.cs               # Opción 3: consulta vulnerable vs. parametrizada
    ├── DiccionariosListas.cs         # Opción 4: Dictionary, List, string.Join y SQL dinámico
    ├── Frecuencias.cs                # Opción 5: frecuencia de 6000 tiradas de un dado
    ├── ConexionBD.cs                 # Servidor, nombre de la base y cadena de conexión
    └── BaseDatos/
        ├── 01_CrearBaseDatos.sql     # Crea la base desde cero (no destructivo)
        └── 02_ReiniciarBaseDatos.sql # Borra la base por completo y la recrea
```

### Opciones del menú

| # | Ejemplo | Usa base de datos |
|---|---------|:-:|
| 1 | Métodos sobrecargados y recursividad (cuadrado `int`/`double`, factorial, Fibonacci) | No |
| 2 | Consultas preparadas: inserta N productos reutilizando una sola sentencia `Prepare()` | **Sí** |
| 3 | Inyección SQL (OWASP A05): compara una búsqueda concatenada con una parametrizada | **Sí** |
| 4 | Diccionarios y listas: búsqueda con `TryGetValue` y armado de un `UPDATE` dinámico | No |
| 5 | Frecuencias: simula 6000 tiradas de un dado y cuenta cada cara | No |

## Requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- **SQL Server** (Express, Developer o LocalDB) con autenticación de Windows
- Para abrir la solución `.slnx`: Visual Studio 2022 (17.13 o superior) o Visual Studio 2026. También se puede usar solo la CLI de `dotnet`.
- Para ejecutar los scripts: **SSMS** o **sqlcmd**

## Setup rápido (automático)

Desde la raíz del repositorio, en PowerShell:

```powershell
powershell -ExecutionPolicy Bypass -File .\setup.ps1              # crea la base si no existe
powershell -ExecutionPolicy Bypass -File .\setup.ps1 -Reiniciar   # la borra y la recrea
powershell -ExecutionPolicy Bypass -File .\setup.ps1 -Servidor ".\SQLEXPRESS"
```

El script:

1. Comprueba que `sqlcmd` y `dotnet` estén instalados.
2. Ejecuta `01_CrearBaseDatos.sql` (o `02_ReiniciarBaseDatos.sql` con `-Reiniciar`).
3. Verifica que `dbo.productos` tenga las columnas correctas y los 3 productos semilla.
4. Compila el proyecto.
5. Hace una prueba de humo: ejecuta el menú con entradas automáticas, inserta 2 productos (opción 2) y prueba el payload `' OR '1'='1` (opción 3). Comprueba que la forma vulnerable devuelve todo y que la parametrizada no devuelve nada.

Al final muestra `[OK]` o `[FALLO]` por cada comprobación. Si algo falla, imprime la salida completa de la aplicación.

Si prefieres hacerlo a mano, sigue los pasos de la siguiente sección.

## Configuración

### 1. Configurar la conexión

En `ConexionBD.cs`, ajusta el servidor a tu instancia:

```csharp
public const string Servidor = "Mateo";                         // p. ej. "localhost", ".\\SQLEXPRESS", "(localdb)\\MSSQLLocalDB"
public const string NombreBaseDatos = "practica28_productosdb"; // base exclusiva de esta práctica
```

La conexión usa seguridad integrada (`Integrated Security=True`) y `TrustServerCertificate=True`, así que no necesita usuario ni contraseña.

### 2. Crear la base de datos

Ejecuta `BaseDatos/01_CrearBaseDatos.sql`:

- **SSMS:** abre el archivo y pulsa **Execute (F5)**.
- **CLI** (desde la carpeta del proyecto):

  ```
  sqlcmd -S Mateo -E -C -i BaseDatos\01_CrearBaseDatos.sql
  ```

  `-E` = autenticación de Windows · `-C` = confiar en el certificado del servidor. Si cambiaste el servidor en el paso 1, usa el mismo valor en `-S`.

El script crea:

- La base `practica28_productosdb`
- La tabla `dbo.productos`:

  | Columna | Tipo | Notas |
  |---|---|---|
  | `id` | `INT IDENTITY` | Clave primaria |
  | `nombre` | `NVARCHAR(100)` | Obligatorio |
  | `precio` | `DECIMAL(10,2)` | ≥ 0 |
  | `cantidad` | `INT` | ≥ 0, valor por defecto 0 |
  | `imagen` | `VARBINARY(MAX)` | Opcional |

- 3 productos de ejemplo (`Teclado`, `Mouse`, `Monitor`), para que la opción 3 tenga datos que devolver.

Puedes ejecutar el script varias veces sin problema: si la base o la tabla ya existen, no las modifica.

### 3. Compilar y ejecutar

```
dotnet run --project Practica28-9-2026
```

O abre `Practica28-9-2026.slnx` en Visual Studio y pulsa **F5**. NuGet restaura `Microsoft.Data.SqlClient` automáticamente.

## Reiniciar la base de datos

Para dejar la base como recién creada (por ejemplo, después de muchas inserciones con la opción 2):

```
sqlcmd -S Mateo -E -C -i BaseDatos\02_ReiniciarBaseDatos.sql
```

> ⚠️ **Operación destructiva:** elimina `practica28_productosdb` con todos sus datos y la vuelve a crear con el esquema y los 3 productos de ejemplo. Antes de borrarla cierra las conexiones abiertas (`SET SINGLE_USER WITH ROLLBACK IMMEDIATE`), así que no falla aunque SSMS o la aplicación estén conectados.

## Probar la inyección SQL (opción 3)

1. Busca un nombre real, por ejemplo `Mouse`: las dos formas devuelven 1 fila.
2. Busca el payload `' OR '1'='1`:
   - **Forma vulnerable:** la consulta queda como `... WHERE nombre = '' OR '1'='1'` y devuelve **todos** los productos.
   - **Forma segura:** `@nombre` se trata como texto literal y devuelve 0 filas.

## Solución de problemas

| Síntoma | Causa probable / solución |
|---|---|
| `Error 4060: Cannot open database "practica28_productosdb"` | La base no existe todavía. Ejecuta `01_CrearBaseDatos.sql`. |
| `Error 208: Invalid object name 'productos'` | Falta la tabla. Ejecuta `01_CrearBaseDatos.sql` o `02_ReiniciarBaseDatos.sql`. |
| `Error 2` / `-1` / `53`: no se encuentra el servidor | El valor de `Servidor` en `ConexionBD.cs` no coincide con tu instancia, o el servicio SQL Server está detenido. |
| `Error 18456: Login failed` | Tu usuario de Windows no tiene un login en la instancia. Agrégalo desde SSMS → Security → Logins. |
| `sqlcmd` no se reconoce | Instala las [herramientas sqlcmd](https://learn.microsoft.com/sql/tools/sqlcmd/sqlcmd-utility) o ejecuta los scripts desde SSMS. |
