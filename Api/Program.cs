// Permite autenticar usuarios usando JWT
using Microsoft.AspNetCore.Authentication.JwtBearer;

// Permite manejar encabezados cuando la app está detrás de proxies (Render usa proxy)
using Microsoft.AspNetCore.HttpOverrides;

// Permite validar tokens JWT
using Microsoft.IdentityModel.Tokens;

using MusicBares.Application.Interfaces.Context;
using MusicBares.Application.Interfaces.Repositories;
using MusicBares.Application.Interfaces.Servicios;
using MusicBares.Application.Servicios;
using MusicBares.Infrastructure.Conexion;
using MusicBares.Infrastructure.Context;
using MusicBares.Infrastructure.Repositories;


// ===========================
// CREACIÓN DEL BUILDER
// ===========================
var builder = WebApplication.CreateBuilder(args);


// Obtiene puerto desde Render o usa 8080 local
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";

// Configura el puerto
builder.WebHost.UseUrls($"http://*:{port}");


// ===========================
// INYECCIÓN DEPENDENCIAS
// ===========================

// Fábrica conexión BD
builder.Services.AddSingleton<FabricaConexion>();


// BAR
builder.Services.AddScoped<IBarRepositorio, BarRepositorioDapper>();
builder.Services.AddScoped<IBarServicio, BarServicio>();


// USUARIO
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorioDapper>();
builder.Services.AddScoped<IUsuarioServicio, UsuarioServicio>();


// MESA
builder.Services.AddScoped<IMesaRepositorio, MesaRepositorioDapper>();
builder.Services.AddScoped<IMesaServicio, MesaServicio>();


// VIDEO MESA
builder.Services.AddScoped<IVideoMesaRepositorio, VideoMesaRepositorioDapper>();
builder.Services.AddScoped<IVideoMesaServicio, VideoMesaServicio>();


// Acceso al HttpContext
builder.Services.AddHttpContextAccessor();

// Contexto usuario autenticado
builder.Services.AddScoped<IUsuarioContext, UsuarioContext>();

// Servicio usuario actual
builder.Services.AddScoped<IUsuarioActualServicio, UsuarioActualServicio>();


/*
=========================================
CONFIGURACIÓN JWT SUPABASE (FORMA MODERNA)
=========================================
*/

// Obtiene issuer desde appsettings
var issuer = builder.Configuration["Supabase:Issuer"];

builder.Services
.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.Authority = issuer;
    options.RequireHttpsMetadata = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateAudience = false,
        ValidateLifetime = true
    };

    // 🔥 DEBUG JWT
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("TOKEN INVALIDO: " + context.Exception.Message);
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("TOKEN VALIDO");
            return Task.CompletedTask;
        }
    };
});





// Autorización
builder.Services.AddAuthorization();


// Controllers
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// ===========================
// CONSTRUCCIÓN APP
// ===========================
var app = builder.Build();


// Swagger solo desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// Permite trabajar detrás del proxy Render
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto
});


app.UseHttpsRedirection();


// Activa autenticación
app.UseAuthentication();

// Activa autorización
app.UseAuthorization();


// Map Controllers
app.MapControllers();

app.Run();
