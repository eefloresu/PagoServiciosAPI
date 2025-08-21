# PagoServiciosAPI

API RESTful para la gestión de pagos de servicios de una compañía de cable e internet.

## Puntos importantes

- **Autenticación JWT:** La API requiere tokens JWT para acceder a los endpoints protegidos.
- **Configuración por variables de entorno:** Las cadenas de conexión y la clave secreta JWT se obtienen desde variables de entorno, mejorando la seguridad.
- **Swagger:** Incluye documentación interactiva y pruebas de endpoints mediante Swagger UI.
- **Logs:** El sistema de logging utiliza Serilog para registrar eventos y errores en archivos diarios.
- **Entity Framework Core:** El acceso a la base de datos se realiza mediante EF Core y el contexto `PagosServiciosDbContext`.
- **Estructura modular:** El proyecto está organizado para facilitar la escalabilidad y el mantenimiento.

## Requisitos

- .NET 6 o superior
- MySQL
- Variables de entorno configuradas:
  - `Connection_PagoServiciosAPI`
  - `JWT_SECRET_KEY`
