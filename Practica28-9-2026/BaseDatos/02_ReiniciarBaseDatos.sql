/* =====================================================================
   02_ReiniciarBaseDatos.sql
   LIMPIA POR COMPLETO practica28_productosdb (la elimina, con todos
   sus datos) y la vuelve a crear desde cero con su esquema y semilla.
   !! Operacion destructiva: se pierden todos los registros. !!
   Ejecucion:
     SSMS : abrir el archivo y pulsar Execute (F5)
     CLI  : sqlcmd -S Mateo -E -C -i BaseDatos\02_ReiniciarBaseDatos.sql
   ===================================================================== */

USE master;
GO

/* ---------- 1. Eliminar la base existente ---------- */
IF DB_ID(N'practica28_productosdb') IS NOT NULL
BEGIN
    PRINT N'Cerrando conexiones activas y eliminando practica28_productosdb...';
    -- Expulsa cualquier sesion abierta (SSMS, la app, etc.) y revierte sus transacciones.
    ALTER DATABASE practica28_productosdb SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE practica28_productosdb;
    PRINT N'Base de datos eliminada.';
END
ELSE
    PRINT N'practica28_productosdb no existia; se creara desde cero.';
GO

/* ---------- 2. Crear la base de nuevo ---------- */
CREATE DATABASE practica28_productosdb;
GO

USE practica28_productosdb;
GO

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
GO

INSERT INTO dbo.productos (nombre, precio, cantidad, imagen) VALUES
    (N'Teclado',  25.00, 10, NULL),
    (N'Mouse',    12.50, 25, NULL),
    (N'Monitor', 180.00,  4, NULL);
GO

PRINT N'practica28_productosdb reiniciada: esquema recreado y 3 productos semilla insertados.';
GO
