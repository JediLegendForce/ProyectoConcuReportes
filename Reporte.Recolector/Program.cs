using Newtonsoft.Json;
using Reporte.Recolector;
using Reporte.Recolector.Dtos;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHostedService<RecolectorBackgroundService>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();


//leer todos los archivos del folder sales y enviar datos de 50 en 50 y al terminar eliminar los datos
// usar paralelismo

