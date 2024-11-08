using Microsoft.EntityFrameworkCore;
using Iglesia_Bautista_Centro_Familiar_NET.CORE;
using Iglesia_Bautista_Centro_Familiar_NET.CORE.Models;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configura CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTodo", builder =>
    {
        builder.AllowAnyOrigin() // Permite cualquier origen
               .AllowAnyMethod() // Permite cualquier método (GET, POST, etc.)
               .AllowAnyHeader(); // Permite cualquier cabecera
    });
});

// Agregar contexto de base de datos
builder.Services.AddDbContext<dbIglesia_Bautista_Centro_FamiliarContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("ConcexionSQLSERVER")));


// Configurar opciones de JSON para ignorar ciclos de referencia
builder.Services.AddControllers().AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Habilitar CORS
app.UseCors("PermitirTodo");

app.UseAuthorization();

app.MapControllers();

app.Run();
