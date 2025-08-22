create database PagoServiciosDB;

use PagoServiciosDB;

-- Tabla de clientes
CREATE TABLE Clientes (
    ClienteID INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Telefono VARCHAR(20) NOT NULL
);

-- Tabla de paquetes disponibles
CREATE TABLE Paquetes (
    PaqueteID INT AUTO_INCREMENT PRIMARY KEY,
    NombrePaquete VARCHAR(100) NOT NULL,
    Descripcion TEXT,
    Precio DECIMAL(10, 2) NOT NULL
);

-- Tabla de conceptos de pago (ej: internet, cable, mora)
CREATE TABLE ConceptosPago (
    ConceptoID INT AUTO_INCREMENT PRIMARY KEY,
    NombreConcepto VARCHAR(50) NOT NULL
);

-- Tabla de pagos (una "factura")
CREATE TABLE Pagos (
    PagoID INT AUTO_INCREMENT PRIMARY KEY,
    ClienteID INT NOT NULL,
    PaqueteID INT NOT NULL,
    TotalPagar DECIMAL(10, 2) NOT NULL,
    FechaVencimiento DATE NOT NULL,
    Estado ENUM('pendiente', 'pagado', 'anulada') DEFAULT 'pendiente',
    FOREIGN KEY (ClienteID) REFERENCES Clientes(ClienteID),
    FOREIGN KEY (PaqueteID) REFERENCES Paquetes(PaqueteID)
);

-- Tabla detalle de conceptos de pago en un pago
CREATE TABLE PagoDetalle (
    PagoDetalleID INT AUTO_INCREMENT PRIMARY KEY,
    PagoID INT NOT NULL,
    ConceptoID INT NOT NULL,
    Monto DECIMAL(10, 2) NOT NULL,
    FOREIGN KEY (PagoID) REFERENCES Pagos(PagoID),
    FOREIGN KEY (ConceptoID) REFERENCES ConceptosPago(ConceptoID)
);


CREATE TABLE Usuarios (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    NombreUsuario VARCHAR(100) NOT NULL,
    Clave VARCHAR(255) NOT NULL,
    Role VARCHAR(20) NOT NULL
);

