// Permite autenticar usuarios usando JWT
using Microsoft.AspNetCore.Authentication.JwtBearer;

// Permite manejar encabezados cuando la app está detrás de proxies (Render usa proxy)
using Microsoft.AspNetCore.HttpOverrides;

// Contiene clases para validar tokens (issuer, expiración, etc.)
using Microsoft.IdentityModel.Tokens;

// Interfaces del contexto del usuario autenticado
using MusicBares.Application.Interfaces.Context;

// Interfaces de repositorios
using MusicBares.Application.Interfaces.Repositories;

// Interfaces de servicios
using MusicBares.Application.Interfaces.Servicios;

// Implementaciones de servicios
using MusicBares.Application.Servicios;

// Fábrica que crea conexiones a PostgreSQL
using MusicBares.Infrastructure.Conexion;

// Implementación del contexto del usuario autenticado
using MusicBares.Infrastructure.Context;

// Repositorios usando Dapper
using MusicBares.Infrastructure.Repositories;


// Crea el builder principal de la aplicación ASP.NET
var builder = WebApplication.CreateBuilder(args);


// Obtiene el puerto desde Render o usa 8080 en local
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";


// Configura la aplicación para escuchar en ese puerto
builder.WebHost.UseUrls($"http://*:{port}");


// Registra FabricaConexion como Singleton (una sola instancia en toda la app)
builder.Services.AddSingleton<FabricaConexion>();


// ===========================
// INYECCIÓN DE DEPENDENCIAS
// ===========================


// Repositorio y servicio de Bar
builder.Services.AddScoped<IBarRepositorio, BarRepositorioDapper>();
builder.Services.AddScoped<IBarServicio, BarServicio>();


// Repositorio y servicio de Usuario
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorioDapper>();
builder.Services.AddScoped<IUsuarioServicio, UsuarioServicio>();


// Repositorio y servicio de Mesa
builder.Services.AddScoped<IMesaRepositorio, MesaRepositorioDapper>();
builder.Services.AddScoped<IMesaServicio, MesaServicio>();


// Repositorio y servicio de VideoMesa
builder.Services.AddScoped<IVideoMesaRepositorio, VideoMesaRepositorioDapper>();
builder.Services.AddScoped<IVideoMesaServicio, VideoMesaServicio>();


// Permite acceder al HttpContext actual (necesario para leer el JWT)
builder.Services.AddHttpContextAccessor();


// Contexto que obtiene datos del usuario autenticado desde el JWT
builder.Services.AddScoped<IUsuarioContext, UsuarioContext>();


// Servicio que obtiene el usuario actual desde el token
builder.Services.AddScoped<IUsuarioActualServicio, UsuarioActualServicio>();


// ===========================
// CONFIGURACIÓN JWT SUPABASE
// ===========================


// Obtiene el issuer desde appsettings.json
var issuer = builder.Configuration["Supabase:Issuer"];


// Configura autenticación usando JWT
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Supabase expone las claves públicas automáticamente aquí
        options.Authority = issuer;

        // Parámetros de validación del token
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Verifica que el token venga del issuer correcto
            ValidateIssuer = true,

           

            // Supabase no requiere validar audiencia
            ValidateAudience = false,

            // Verifica que el token no esté expirado
            ValidateLifetime = true
        };
    });


// Habilita autorización con atributos como [Authorize]
builder.Services.AddAuthorization();


// ===========================
// SERVICIOS BÁSICOS ASP.NET
// ===========================


// Permite usar Controllers
builder.Services.AddControllers();


// Permite generar endpoints para Swagger
builder.Services.AddEndpointsApiExplorer();


// Habilita Swagger para documentar la API
builder.Services.AddSwaggerGen();


// Construye la aplicación
var app = builder.Build();


// Si estamos en ambiente de desarrollo se habilita Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// Permite que Render pase correctamente IP y protocolo al backend
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});


// Redirige automáticamente HTTP → HTTPS
app.UseHttpsRedirection();


// Activa autenticación JWT
app.UseAuthentication();


// Activa autorización basada en atributos [Authorize]
app.UseAuthorization();


// Mapea los controllers a endpoints HTTP
app.MapControllers();


// Inicia la aplicación
app.Run();
