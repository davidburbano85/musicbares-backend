// Permite usar autenticación JWT en ASP.NET
using Microsoft.AspNetCore.Authentication.JwtBearer;

// Permite trabajar correctamente cuando la app está detrás de proxies como Render
using Microsoft.AspNetCore.HttpOverrides;

// Permite validar tokens JWT
using Microsoft.IdentityModel.Tokens;

// Permite acceder a configuración del appsettings
using Microsoft.Extensions.Configuration;

// Interfaces del proyecto
using MusicBares.Application.Interfaces.Context;
using MusicBares.Application.Interfaces.Repositories;
using MusicBares.Application.Interfaces.Servicios;

// Implementaciones del proyecto
using MusicBares.Application.Servicios;
using MusicBares.Infrastructure.Conexion;
using MusicBares.Infrastructure.Context;
using MusicBares.Infrastructure.Repositories;


// ===========================
// CREACIÓN DEL BUILDER
// ===========================
var builder = WebApplication.CreateBuilder(args);


// Obtiene el puerto que Render asigna dinámicamente
// Si no existe (ejecución local) usa 8080
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";

// Permite que la API escuche en cualquier IP en ese puerto
builder.WebHost.UseUrls($"http://*:{port}");


// ===========================
// OBTENER CONFIGURACIÓN SUPABASE
// ===========================

// Obtiene el issuer desde appsettings.json
// Este valor define quién generó el token JWT
var supabaseIssuer = builder.Configuration["Supabase:Issuer"];

// Obtiene audience desde appsettings
// Supabase usa "authenticated" para usuarios logueados
var supabaseAudience = builder.Configuration["Supabase:Audience"];


// ===========================
// INYECCIÓN DEPENDENCIAS
// ===========================

// Fábrica de conexión PostgreSQL
builder.Services.AddSingleton<FabricaConexion>();


// ================= BAR =================
builder.Services.AddScoped<IBarRepositorio, BarRepositorioDapper>();
builder.Services.AddScoped<IBarServicio, BarServicio>();


// ================= USUARIO =================
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorioDapper>();
builder.Services.AddScoped<IUsuarioServicio, UsuarioServicio>();


// ================= MESA =================
builder.Services.AddScoped<IMesaRepositorio, MesaRepositorioDapper>();
builder.Services.AddScoped<IMesaServicio, MesaServicio>();


// ================= VIDEO MESA =================
builder.Services.AddScoped<IVideoMesaRepositorio, VideoMesaRepositorioDapper>();
builder.Services.AddScoped<IVideoMesaServicio, VideoMesaServicio>();


// Permite acceder al HttpContext actual
builder.Services.AddHttpContextAccessor();


// Contexto del usuario autenticado
builder.Services.AddScoped<IUsuarioContext, UsuarioContext>();


// Servicio que obtiene usuario actual desde JWT
builder.Services.AddScoped<IUsuarioActualServicio, UsuarioActualServicio>();


/*
=========================================
CONFIGURACIÓN AUTENTICACIÓN JWT SUPABASE
=========================================
*/
builder.Services
.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    // Define el proveedor de autenticación
    // ASP.NET descargará automáticamente las claves públicas de Supabase
    options.Authority = supabaseIssuer;

    // Parámetros que definen cómo validar el token
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // Verifica que el token venga del issuer correcto
        ValidateIssuer = true,
        ValidIssuer = supabaseIssuer,

        // Verifica que el token sea para usuarios autenticados
        ValidateAudience = true,
        ValidAudience = supabaseAudience,

        // Rechaza tokens expirados
        ValidateLifetime = true,

        // Reduce margen de tolerancia de expiración (mejor seguridad)
        ClockSkew = TimeSpan.FromSeconds(30)
    };

    // Evita que .NET renombre automáticamente los claims
    // Supabase usa nombres estándar que queremos conservar
    options.MapInboundClaims = false;

    // ==========================
    // EVENTOS DE DEBUG JWT
    // ==========================
    options.Events = new JwtBearerEvents
    {
        // Se ejecuta cuando llega el token
        OnMessageReceived = context =>
        {
            // Obtiene el header Authorization
            var authHeader = context.Request.Headers["Authorization"].ToString();

            // Log útil para debugging
            if (string.IsNullOrWhiteSpace(authHeader))
            {
                Console.WriteLine("⚠ Authorization header vacío");
            }

            return Task.CompletedTask;
        },

        // Se ejecuta cuando falla la autenticación
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("❌ Error validando JWT:");
            Console.WriteLine(context.Exception.Message);

            return Task.CompletedTask;
        },

        // Se ejecuta cuando el token es válido
        OnTokenValidated = context =>
        {
            Console.WriteLine("✅ Token JWT validado correctamente");

            return Task.CompletedTask;
        }
    };
});


// ===========================
// AUTORIZACIÓN
// ===========================
builder.Services.AddAuthorization();


// Controllers
builder.Services.AddControllers();


// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// ===========================
// CONSTRUCCIÓN APP
// ===========================
var app = builder.Build();


// Swagger solo en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// Permite que Render pase correctamente IP y protocolo original
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto
});


// Redirecciona HTTP → HTTPS
app.UseHttpsRedirection();


// Activa middleware de autenticación
app.UseAuthentication();


// Activa middleware de autorización
app.UseAuthorization();


// Mapea controladores
app.MapControllers();


// Ejecuta la aplicación
app.Run();
