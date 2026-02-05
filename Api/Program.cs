using Microsoft.AspNetCore.HttpOverrides;
using MusicBares.Infrastructure.Conexion;
using MusicBares.Infrastructure.Repositorios;



var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<FabricaConexion>();
builder.Services.AddScoped<PruebaConexionRepositorio>();


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
