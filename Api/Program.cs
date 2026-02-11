// Permite usar autenticación JWT en ASP.NET
using Microsoft.AspNetCore.Authentication.JwtBearer;
// Permite trabajar correctamente cuando la app está detrás de proxies como Render
using Microsoft.AspNetCore.HttpOverrides;
// Permite acceder a configuración del appsettings
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
// Permite validar tokens JWT
using Microsoft.IdentityModel.Tokens;
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
    // Dirección metadata OpenID Supabase
    options.MetadataAddress =
        $"{supabaseIssuer}/.well-known/openid-configuration";

    // Fuerza uso HTTPS
    options.RequireHttpsMetadata = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = supabaseIssuer,

        ValidateAudience = true,
        ValidAudience = supabaseAudience,

        ValidateLifetime = true,

        // Permite validar firma con JWKS descargadas
        ValidateIssuerSigningKey = true,

        ClockSkew = TimeSpan.FromSeconds(30)
    };

    options.MapInboundClaims = false;
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
