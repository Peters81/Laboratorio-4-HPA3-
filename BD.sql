
CREATE DATABASE cristell;
USE cristell;


DROP TABLE IF EXISTS `productos`;
CREATE TABLE IF NOT EXISTS `productos` (
  `id` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `precio` decimal(10,2) NOT NULL,
  `cantidad` int NOT NULL,
  `imagen` longblob NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;


INSERT INTO `productos` (`id`, `nombre`, `precio`, `cantidad`, `imagen`) VALUES
(1, 'Teclado', 20.00, 2, ''),
(2, 'Mouse', 15.70, 2, ''),
(3, 'Portátil', 700.70, 2, '');


SELECT * FROM productos;