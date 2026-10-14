
-- ==========================================
-- LABORATORIO 4 - HPA III
-- BASE DE DATOS: cristell
-- ==========================================

CREATE DATABASE IF NOT EXISTS cristell;

USE cristell;


-- ==========================================
-- TABLA USUARIOS
-- ==========================================

CREATE TABLE IF NOT EXISTS usuarios (
    id_usuario INT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    usuario VARCHAR(50) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    activo TINYINT(1) NOT NULL DEFAULT 1,
    fecha_registro DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);


-- ==========================================
-- TABLA PRODUCTOS
-- ==========================================

CREATE TABLE IF NOT EXISTS productos (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    precio DECIMAL(10,2) NOT NULL,
    cantidad INT NOT NULL,
    imagen LONGBLOB,

    -- Eliminación lógica
    anulado TINYINT(1) NOT NULL DEFAULT 0,

    -- Fechas
    fecha_creacion DATETIME DEFAULT CURRENT_TIMESTAMP,
    fecha_modificacion DATETIME DEFAULT NULL,

    -- Usuarios
    usuario_creacion INT DEFAULT NULL,
    usuario_modificacion INT DEFAULT NULL,

    CONSTRAINT fk_producto_usuario_creacion
        FOREIGN KEY (usuario_creacion)
        REFERENCES usuarios(id_usuario),

    CONSTRAINT fk_producto_usuario_modificacion
        FOREIGN KEY (usuario_modificacion)
        REFERENCES usuarios(id_usuario)
);


-- ==========================================
-- TABLA AUDITORIA
-- ==========================================

CREATE TABLE IF NOT EXISTS auditoria (
    id_auditoria INT AUTO_INCREMENT PRIMARY KEY,
    id_usuario INT NOT NULL,
    id_producto INT NOT NULL,
    accion VARCHAR(20) NOT NULL,
    descripcion VARCHAR(255),
    fecha_accion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_auditoria_usuario
        FOREIGN KEY (id_usuario)
        REFERENCES usuarios(id_usuario),

    INDEX idx_auditoria_producto (id_producto),
    INDEX idx_auditoria_fecha (fecha_accion)
);


-- =======================
USE cristell;

INSERT INTO usuarios
(nombre, usuario, password_hash, activo)
VALUES
('Cristell Peters', 'cristell', '1234', 1),
('Luis Padilla', 'luis', '5678', 1),
('Administrador', 'admin', 'admin123', 1);



	