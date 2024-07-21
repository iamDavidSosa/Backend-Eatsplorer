
--SCRIPT BASE DE DATOS PARA EATSPLORER--

--USUARIOS--
CREATE TABLE Usuarios (
	id_usuario int identity (1,1) primary key,
	correo VARCHAR(50) NOT NULL,
    usuario VARCHAR(50) NOT NULL,
    clave VARCHAR(200) NOT NULL,
    id_rol INT NOT NULL,
	fecha_creacion DATETIME NOT NULL,
	url_foto_perfil nvarchar(2048),
	descripcion varchar(256),
	url_foto_portada nvarchar(2048),
	cant_recetas_guardadas int default 0
);

--ROLES--
CREATE TABLE Roles (
    id_rol int identity (1,1) primary key,
    nombre_rol VARCHAR(50) NOT NULL
);

--USUARIOS-ROLES--
CREATE TABLE Usuario_roles (
    id_usuario INT,
    id_rol INT
);

--RECETAS--
CREATE TABLE recetas (
	id int identity (1,1) primary key,
    titulo VARCHAR(100) NOT NULL,
    descripcion TEXT,
    instrucciones TEXT NOT NULL,
    foto_receta VARCHAR(255),
    usuario_id INT,
    fecha_creacion DATETIME
);


-- INGREDIENTES
CREATE TABLE Ingredientes (
    id_ingrediente int identity (1,1) primary key,
    nombre VARCHAR(100) NOT NULL
);

-- RECETAS-INGREDIENTES
CREATE TABLE Receta_ingrediente (
    id_receta INT,
    id_ingrediente INT
);

--RECETAS GUARDADAS--
CREATE TABLE recetas_guardadas (
	id int identity (1,1) primary key,
    usuario_id INT NOT NULL,
    receta_id INT NOT NULL,
    fecha_guardado DATETIME
);

--RECETAS RECIENTES--
CREATE TABLE recetas_accedidas_recientemente (
	id int identity (1,1) primary key,
    usuario_id INT NOT NULL,
    receta_id INT NOT NULL,
    fecha_acceso DATETIME NOT NULL
);

--PREGUNTAS CONTRASEÑA
CREATE TABLE Preguntas_Contrasena (
    IdPreguntasContrasena int identity (1,1) primary key,
    pregunta VARCHAR(255) NOT NULL
);


--RECUPERAR CONTRASEÑA
CREATE TABLE Recuperar_Contrasena (
    IdRecuperarContrasena int identity (1,1) primary key,
    IdUsuario INT NOT NULL,
    id_pregunta int NOT NULL,
    respuesta VARCHAR(255) NOT NULL

);
