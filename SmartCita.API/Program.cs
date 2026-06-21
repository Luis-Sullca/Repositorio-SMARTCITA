using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SmartCita.API.Common.Behaviors;
using SmartCita.API.Common.Endpoints;
using SmartCita.Infrastructure.Persistence;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Configuración de la cadena de conexión para SQL Server
var connectionString = builder.Configuration.GetConnectionString("cadenaConexion");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Seguridad y CORS
// 1. Definimos una constante para el nombre de nuestra política
var angularCorsPolicy = "AllowAngularDev";
// 2. CONFIGURACIÓN DEL SERVICIO
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: angularCorsPolicy, policy =>
        {
            policy.WithOrigins("http://localhost:4200") // Puerto exacto de angular
                .AllowAnyHeader()   // Permite enviar json y tokens
                .AllowAnyMethod();  // Permite usar métodos HTTP como GET, POST, PUT, DELETE
        });
});

// VERTICAL SLICE ARCHITECTURE (CQRS)
var assembly = Assembly.GetExecutingAssembly();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(assembly);
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

builder.Services.AddValidatorsFromAssembly(assembly);

// Manejo global de excepciones
builder.Services.AddExceptionHandler<SmartCita.API.Common.Exceptions.GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Activar CORS debe ir casi al principio, ANTES de la autorización y los endpoints
app.UseCors(angularCorsPolicy);

// Si usas HTTPS redirection, va antes o después de CORS, pero CORS siempre antes de Auth
app.UseHttpsRedirection();

app.UseExceptionHandler();

// Aqui irán las llamadas a los endpoints de tus 'Vertical Slice'

var endpointTypes = assembly.GetTypes()
    .Where(t => t.GetInterfaces().Contains(typeof(IEndpoint)) && !t.IsInterface && !t.IsAbstract);

foreach (var type in endpointTypes)
{
    type.GetMethod(nameof(IEndpoint.MapEndpoint))?.Invoke(null, [app]);
}

app.Run();