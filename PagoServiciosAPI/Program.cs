using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PagoServiciosAPI.Data;
using Serilog;
using Serilog.Formatting.Json;

var builder = WebApplication.CreateBuilder(args);

// Ejecutar como servicio de Windows
builder.Host.UseWindowsService();

// Configura Serilog para escribir en archivo
Log.Logger = new LoggerConfiguration()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Obtener la cadena de conexión desde la variable de entorno
var connectionString = Environment.GetEnvironmentVariable("Connection_PagoServiciosAPI");

// Registrar el contexto de base de datos usando la cadena de conexión
builder.Services.AddDbContext<PagosServiciosDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Add services to the container.
// Configuración de servicios para la API
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuración de CORS
var allowedOrigins = Environment.GetEnvironmentVariable("PAGOSERVICIOSAPI_ALLOWED_ORIGINS") ?? "";
var origins = allowedOrigins.Split(';', StringSplitOptions.RemoveEmptyEntries);

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins(origins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Configuración de Swagger con soporte para JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Pago de Servicios API",
        Version = "v1",
        Description = "API para gestión de pagos de servicios de internet, telefonía y otros servicios.",
    });

    // Configuración para JWT en Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando el esquema Bearer. Ejemplo: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Configurar autenticación JWT
var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
if (string.IsNullOrWhiteSpace(jwtKey))
    throw new ArgumentException("La clave secreta 'JWT_SECRET_KEY' no está definida como variable de entorno.");

//Console.WriteLine($"JWT Key en configuracion de programa: {jwtKey}");
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "tu_issuer",
            ValidAudience = "tu_audience",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)),
            RoleClaimType = ClaimTypes.Role
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("❌ Error de autenticación: " + context.Exception.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("✅ Token validado correctamente.");
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                Console.WriteLine("⚠️ Token rechazado o no enviado.");
                return Task.CompletedTask;
            }
        };
    });

// Define URL donde correrá el servicio
builder.WebHost.UseUrls("http://localhost:5201"); //5201 para desarrollo, 5200 para producción

// Mueve esta línea después de configurar todos los servicios
var app = builder.Build();

// Configure the HTTP request pipeline.
/*if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
} */
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors(builder =>
    builder.WithOrigins("*") // o "*" para todos los orígenes
           .AllowAnyHeader()
           .AllowAnyMethod()
);

app.MapControllers();

// Log de arranque
Directory.CreateDirectory("C:\\Deploy\\PagoServiciosAPI\\");
File.AppendAllText("C:\\Deploy\\PagoServiciosAPI\\startup.log", $"Iniciando API: {DateTime.Now}\n");

app.Run();
