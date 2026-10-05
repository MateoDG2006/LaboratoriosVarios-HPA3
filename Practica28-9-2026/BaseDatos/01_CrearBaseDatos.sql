/* =====================================================================
   01_CrearBaseDatos.sql
   Crea desde cero la base de datos practica28_productosdb y su esquema.
   - Si la base ya existe, NO la toca (usa 02_ReiniciarBaseDatos.sql
     para borrarla y recrearla).
   Ejecucion:
     SSMS : abrir el archivo y pulsar Execute (F5)
     CLI  : sqlcmd -S Mateo -E -C -i BaseDatos\01_CrearBaseDatos.sql
   ===================================================================== */

USE master;
GO

IF DB_ID(N'practica28_productosdb') IS NULL
BEGIN
    PRINT N'Creando base de datos practica28_productosdb...';
    CREATE DATABASE practica28_productosdb;
END
ELSE
    PRINT N'La base de datos practica28_productosdb ya existe; se omite su creacion.';
GO

USE practica28_productosdb;
GO

/* ---------- Tabla productos ---------- */
IF OBJECT_ID(N'dbo.productos', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.productos
    (
        id        INT IDENTITY(1,1) NOT NULL,
        nombre    NVARCHAR(100)     NOT NULL,
        precio    DECIMAL(10,2)     NOT NULL,
        cantidad  INT               NOT NULL CONSTRAINT DF_productos_cantidad DEFAULT (0),
        imagen    VARBINARY(MAX)    NULL,
        CONSTRAINT PK_productos          PRIMARY KEY (id),
        CONSTRAINT CK_productos_precio   CHECK (precio >= 0),
        CONSTRAINT CK_productos_cantidad CHECK (cantidad >= 0)
    );
    PRINT N'Tabla dbo.productos creada.';
END
ELSE
    PRINT N'La tabla dbo.productos ya existe; se omite su creacion.';
GO

/* ---------- Datos semilla (para probar Inyeccion SQL) ---------- */
IF NOT EXISTS (SELECT 1 FROM dbo.productos)
BEGIN
    INSERT INTO dbo.productos (nombre, precio, cantidad, imagen) VALUES
        (N'Teclado',  25.00, 10, NULL),
        (N'Mouse',    12.50, 25, NULL),
        (N'Monitor', 180.00,  4, NULL);
    PRINT N'Datos semilla insertados (3 productos).';
END
GO

PRINT N'Base de datos practica28_productosdb lista.';
GO
