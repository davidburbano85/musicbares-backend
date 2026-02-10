// Permite trabajar con encabezados cuando el backend está detrás de proxies (Render usa proxy)
// Librería que permite autenticar usuarios mediante JWT
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
// Contiene clases para validar tokens (firma, expiración, issuer, etc.)
using Microsoft.IdentityModel.Tokens;
using MusicBares.Application.Interfaces.Context;
// Importa interfaces de repositorios
using MusicBares.Application.Interfaces.Repositories;
// Importa interfaces de servicios
using MusicBares.Application.Interfaces.Servicios;
// Importa implementaciones de servicios
using MusicBares.Application.Servicios;
// Importa la fábrica que crea conexiones a PostgreSQL
using MusicBares.Infrastructure.Conexion;
using MusicBares.Infrastructure.Context;
// Importa los repositorios Dapper
using MusicBares.Infrastructure.Repositories;
// Necesario para convertir el JWT Secret a bytes
using System.Text;



// Crea el builder principal de la aplicación ASP.NET
var builder = WebApplication.CreateBuilder(args);

// Obtiene el puerto desde Render (si existe) o usa 8080 local
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";

// Configura la aplicación para escuchar en ese puerto
builder.WebHost.UseUrls($"http://*:{port}");


// Registra FabricaConexion como Singleton
// Esto significa que habrá una sola instancia durante toda la app
builder.Services.AddSingleton<FabricaConexion>();


// Inyección de dependencias para BAR
builder.Services.AddScoped<IBarRepositorio, BarRepositorioDapper>();
builder.Services.AddScoped<IBarServicio, BarServicio>();


// Inyección de dependencias para USUARIO
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorioDapper>();
builder.Services.AddScoped<IUsuarioServicio, UsuarioServicio>();


// Inyección de dependencias para MESA
builder.Services.AddScoped<IMesaRepositorio, MesaRepositorioDapper>();
builder.Services.AddScoped<IMesaServicio, MesaServicio>();


// Inyección de dependencias para VIDEO MESA
builder.Services.AddScoped<IVideoMesaRepositorio, VideoMesaRepositorioDapper>();
builder.Services.AddScoped<IVideoMesaServicio, VideoMesaServicio>();

//inyeccionde  dependencias para  el usuario context
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUsuarioContext, UsuarioContext>();

// ======================================================
// Servicio que permite obtener información del usuario autenticado
// desde el token JWT de Supabase
// ======================================================

// IHttpContextAccessor permite acceder al HttpContext actual
// Es necesario para poder leer el token JWT del usuario logueado
builder.Services.AddHttpContextAccessor();

// Registramos el servicio que obtiene el usuario actual
// Scoped significa que se crea una instancia por cada request HTTP
builder.Services.AddScoped<IUsuarioActualServicio, UsuarioActualServicio>();


/*
=====================================================
CONFIGURACIÓN DE AUTENTICACIÓN JWT CON SUPABASE
=====================================================
*/


// Obtiene el JWT Secret desde variables de entorno en Render
// Este secret es usado para verificar que el token es válido
var jwtSecret = Environment.GetEnvironmentVariable("SUPABASE_JWT_SECRET");


// Obtiene la URL del proyecto Supabase desde appsettings.json
// El issuer es quien emitió el token
var issuer = builder.Configuration["Supabase:Issuer"];


// Agrega el sistema de autenticación al contenedor de servicios
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme) // Define que usaremos JWT como método de autenticación
    .AddJwtBearer(options =>
    {
        // Obliga a que los tokens solo se acepten vía HTTPS
        options.RequireHttpsMetadata = true;

        // Guarda el token dentro del contexto HTTP
        options.SaveToken = true;

        // Configuración de validación del token
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Verifica que el token provenga del issuer correcto
            ValidateIssuer = true,

            // Issuer válido esperado (tu proyecto Supabase)
            ValidIssuer = issuer,

            // No validamos audiencia porque Supabase no la usa en este caso
            ValidateAudience = false,

            // Verifica que el token no esté expirado
            ValidateLifetime = true,

            // Verifica la firma del token usando el secret
            ValidateIssuerSigningKey = true,

            // Convierte el JWT Secret en clave de validación
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSecret!)
            )
        };
    });


// Habilita el sistema de autorización basado en roles/permisos
builder.Services.AddAuthorization();



/*
=====================================================
SERVICIOS BÁSICOS DE ASP.NET
=====================================================
*/


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
// Aquí el backend valida tokens enviados en Authorization: Bearer
app.UseAuthentication();


// Activa autorización basada en atributos como [Authorize]
app.UseAuthorization();


// Mapea los controllers a endpoints HTTP
app.MapControllers();


// Inicia la aplicación
app.Run();
