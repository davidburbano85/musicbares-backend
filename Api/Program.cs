using Microsoft.AspNetCore.HttpOverrides;
using MusicBares.Infrastructure.Conexion;
using MusicBares.Infrastructure.Repositories;
using MusicBares.Application.Interfaces.Servicios;
using MusicBares.Application.Interfaces.Repositories;
using MusicBares.Application.Servicios;




var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://*:{port}");
builder.Services.AddSingleton<FabricaConexion>();

builder.Services.AddScoped<IBarRepositorio, BarRepositorioDapper>();
builder.Services.AddScoped<IBarServicio, BarServicio>();

builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorioDapper>();
builder.Services.AddScoped<IUsuarioServicio, UsuarioServicio>();



// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
