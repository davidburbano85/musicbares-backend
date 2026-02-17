// Permite usar autenticación JWT en ASP.NET
using Microsoft.AspNetCore.Authentication.JwtBearer;
// Permite trabajar correctamente cuando la app está detrás de proxies como Render
using Microsoft.AspNetCore.HttpOverrides;
// Permite acceder a configuración del appsettings
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
// 🔥 Permite ver errores internos reales de JWT

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
using System.IdentityModel.Tokens.Jwt;

// 🔥 Permite ver errores internos reales de JWT
IdentityModelEventSource.ShowPII = true;


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

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);
/*
=========================================
CONFIGURACIÓN AUTENTICACIÓN JWT SUPABASE
=========================================
*/
builder.Services
.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    Console.WriteLine("===== CONFIGURANDO JWT =====");

    options.Authority = supabaseIssuer;
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;

    Console.WriteLine($"Authority configurado: {options.Authority}");

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = supabaseIssuer,

        ValidateAudience = true,
        ValidAudience = supabaseAudience,

        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ClockSkew = TimeSpan.FromSeconds(30)
    };

    options.Events = new JwtBearerEvents
    {

        // ==========================================
        // 🔥 CUANDO LLEGA EL TOKEN
        // ==========================================
        OnMessageReceived = context =>
        {
            Console.WriteLine("\n===== JWT MESSAGE RECEIVED =====");

            var header = context.Request.Headers["Authorization"].ToString();

            Console.WriteLine($"Authorization RAW: {header}");

            if (!string.IsNullOrEmpty(header))
            {
                var token = header.Replace("Bearer ", "");

                Console.WriteLine($"Token Length: {token.Length}");
                Console.WriteLine($"Dot Count: {token.Count(c => c == '.')}");

                // 🔥 Intentar leer token SIN validar
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwt = handler.ReadJwtToken(token);

                    Console.WriteLine("----- TOKEN HEADER -----");
                    Console.WriteLine($"Algoritmo: {jwt.Header.Alg}");
                    Console.WriteLine($"Kid: {jwt.Header.Kid}");

                    Console.WriteLine("----- TOKEN CLAIMS -----");
                    foreach (var claim in jwt.Claims)
                    {
                        Console.WriteLine($"{claim.Type} = {claim.Value}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("❌ Error leyendo JWT sin validar:");
                    Console.WriteLine(ex);
                }
            }

            Console.WriteLine("===== FIN MESSAGE RECEIVED =====\n");

            return Task.CompletedTask;
        },

        // ==========================================
        // 🔥 CUANDO .NET DESCARGA METADATA OPENID
        // ==========================================
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("\n❌ AUTHENTICATION FAILED");
            Console.WriteLine("Exception completa:");

            Console.WriteLine(context.Exception.ToString());

            if (context.Exception.InnerException != null)
            {
                Console.WriteLine("\n---- INNER EXCEPTION ----");
                Console.WriteLine(context.Exception.InnerException);
            }

            Console.WriteLine("===== FIN AUTH FAILED =====\n");

            return Task.CompletedTask;
        },

        // ==========================================
        // 🔥 TOKEN VALIDADO
        // ==========================================
        OnTokenValidated = context =>
        {
            Console.WriteLine("\n✅ TOKEN VALIDADO CORRECTAMENTE");

            var jwt = context.SecurityToken as JwtSecurityToken;

            if (jwt != null)
            {
                Console.WriteLine($"Algoritmo validado: {jwt.Header.Alg}");
                Console.WriteLine($"Kid validado: {jwt.Header.Kid}");
                Console.WriteLine($"Issuer token: {jwt.Issuer}");
                Console.WriteLine($"Audience token: {string.Join(",", jwt.Audiences)}");
            }

            Console.WriteLine("----- CLAIMS POST VALIDACIÓN -----");

            foreach (var claim in context.Principal.Claims)
            {
                Console.WriteLine($"{claim.Type} = {claim.Value}");
            }

            Console.WriteLine("===== FIN TOKEN VALIDADO =====\n");

            return Task.CompletedTask;
        },

        // ==========================================
        // 🔥 CUANDO FALLA AUTORIZACIÓN
        // ==========================================
        OnChallenge = context =>
        {
            Console.WriteLine("\n⚠️ CHALLENGE ACTIVADO");
            Console.WriteLine($"Error: {context.Error}");
            Console.WriteLine($"Description: {context.ErrorDescription}");
            Console.WriteLine($"Uri: {context.ErrorUri}");

            Console.WriteLine("===== FIN CHALLENGE =====\n");

            return Task.CompletedTask;
        },


        


        // ==========================================
        // 🔥 CUANDO USER NO TIENE PERMISOS
        // ==========================================
        OnForbidden = context =>
        {
            Console.WriteLine("\n🚫 FORBIDDEN EVENT ACTIVADO");

            return Task.CompletedTask;
        }
    };

    Console.WriteLine("===== FIN CONFIG JWT =====");
});







// ===========================
// AUTORIZACIÓN
// ===========================
builder.Services.AddAuthorization();

// ===========================
// CONFIGURACIÓN CORS
// Permite que el frontend Angular
// pueda llamar a esta API desde otro dominio
// ===========================
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy
            // 🔹 Permite peticiones desde tu frontend en Render
            .WithOrigins(
                "http://localhost:4200",              // desarrollo local
                "https://music-bares.onrender.com"    // frontend en producción
            )

            // 🔹 Permite cualquier header (Authorization, Content-Type, etc.)
            .AllowAnyHeader()

            // 🔹 Permite GET, POST, PUT, DELETE, etc.
            .AllowAnyMethod();
    });
});


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
//app.UseHttpsRedirection();


// Activa middleware de autenticación
app.UseAuthentication();


// Activa middleware de autorización
app.UseAuthorization();


// Mapea controladores
app.MapControllers();


// Ejecuta la aplicación
app.Run();
