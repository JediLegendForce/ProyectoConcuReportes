using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Options;
using Reporte.Sucursal;
using Reporte.Sucursal.Services;

var builder = WebApplication.CreateBuilder(args);
var DB = new Database();
//EmployeeService x = new EmployeeService(DB.Employees);
builder.Services.AddControllers();
builder.Services.AddScoped<IEmployeeService, EmployeeService>(
    serviceProvider => new EmployeeService(DB.Employees)
    ) ;
builder.Services.AddScoped<ICarService, CarService>(
    serviceProvider => new CarService(DB.Cars));

builder.Services.AddScoped<ISucursalesService,SucursalesService>( 
    serviceProvider => new SucursalesService(DB.Sucursales) );

//builder.Services.AddScoped<IEmployeeService, EmployeeService>();
var app = builder.Build();


app.MapGet("/", () => "Hello World!");
app.MapControllers();

app.Run();
