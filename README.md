# Laboratorio 4 - HPA3

## Información del laboratorio

| Dato | Información |
|---|---|
| Curso | Herramientas de la Programación Aplicada III (.NET) |
| Laboratorio | #4 |
| Estudiante | Cristell Peters |
| Grupo | 1IL133 |
| Carrera | Ingeniería en Sistemas y Computación |
| Año | 2026 |
| Fecha | 14/09/2026 |
| Instructor | Ing. Irina Fong |

## Objetivos

🩷 Crear una interfaz gráfica en C# Windows Forms que permita ingresar información de productos y sus imágenes.

🩷 Convertir imágenes a arreglos de bytes utilizando `MemoryStream` para poder almacenarlas en una base de datos.

🩷 Conectar la aplicación de C# con MySQL para insertar y cargar registros en un `DataGridView`.

## Contenido del laboratorio

En este laboratorio se desarrolló una aplicación en C# Windows Forms conectada a una base de datos MySQL. El programa permite registrar productos con información como nombre, precio, cantidad e imagen. Durante el desarrollo se trabajó con la conexión entre C# y MySQL, el manejo de datos y la conversión de imágenes para poder almacenarlas en la base de datos.

## Diseño de la aplicación

Primero se diseñó el formulario de la aplicación utilizando Windows Forms. Se agregaron los campos necesarios para ingresar la información de cada producto y controles como el `DataGridView` para mostrar los registros y el `PictureBox` para trabajar con las imágenes.

También se utilizó un `ImageList` para colocar imágenes en algunos de los botones de la aplicación, como guardar, modificar y limpiar.

![Diseño de la aplicación](./imagenes/lab4iamagen1.png)

## Base de datos MySQL

La información de los productos se almacena en una base de datos MySQL. Para este laboratorio se creó una tabla llamada `productos`, que contiene los datos principales de cada producto.

Los campos utilizados son `id`, `nombre`, `precio`, `cantidad` e `imagen`. El campo `imagen` utiliza el tipo `LONGBLOB`, que permite almacenar los datos de la imagen en formato binario.

En C#, este tipo de información se maneja mediante un arreglo de bytes (`byte[]`), lo que permite convertir la imagen para guardarla en la base de datos.

![Base de datos MySQL](./imagenes/lab4iamagen2.png)

## Conexión entre C# y MySQL

Para conectar la aplicación con MySQL se creó una clase encargada de establecer la conexión con la base de datos. Para esto se utilizó `MySqlConnection` junto con el paquete `MySql.Data`, instalado mediante NuGet.

A partir de esta conexión se pueden realizar consultas y obtener los registros almacenados en la tabla `productos`. También se utilizan `MySqlCommand` para ejecutar instrucciones SQL y `MySqlDataReader` para leer los datos obtenidos.

## Manejo de productos

Los productos se manejan mediante una lista de objetos `Producto`. Los registros obtenidos desde la base de datos se cargan en esta lista y posteriormente se muestran en el `DataGridView`.

Para realizar esta parte se utiliza `List<Producto>` y un método encargado de obtener los productos desde MySQL. También se agregó un filtro de búsqueda que permite consultar los registros utilizando los datos escritos en el campo de búsqueda.

El `DataGridView` permite mostrar los datos de los productos y también las imágenes que se obtienen desde la base de datos.

## Manejo de imágenes

Una de las partes principales del laboratorio fue trabajar con las imágenes de los productos. Para poder almacenarlas en MySQL, primero se convierten en un arreglo de bytes.

Para realizar esta conversión se utiliza `MemoryStream`, que permite trabajar temporalmente con los datos de la imagen en la memoria. De esta manera, la imagen que se encuentra en el `PictureBox` puede convertirse en `byte[]` y luego almacenarse en el campo `LONGBLOB` de la base de datos.

Cuando la imagen se obtiene nuevamente desde MySQL, se realiza el proceso contrario. Los bytes se cargan en un `MemoryStream` y luego se utiliza `Bitmap` para reconstruir la imagen y mostrarla en la aplicación.

## Selección y carga de imágenes

Para seleccionar una imagen desde la computadora se utilizó `OpenFileDialog`. Este componente permite abrir el explorador de archivos de Windows y seleccionar una imagen.

Después de seleccionar el archivo, la imagen se carga en el `PictureBox`, donde puede visualizarse antes de guardar el producto.

También se configuró el filtro del `OpenFileDialog` para trabajar con formatos de imagen como `.jpg`, `.png` y `.bmp`.

## Herramientas utilizadas

🩷 C#

🩷 Windows Forms

🩷 .NET

🩷 MySQL

🩷 MySQL Workbench

🩷 Visual Studio

🩷 NuGet / MySql.Data

🩷 DataGridView

🩷 PictureBox

🩷 OpenFileDialog

🩷 MemoryStream

## Estructura del repositorio

```text
Laboratorio-4-HPA3/
│
├── ...
├── imagenes/
│   ├── lab4iamagen1.png
│   └── lab4iamagen2.png
├── .gitignore
└── README.md
