using Newtonsoft.Json;
using Reporte.Recolector.Dtos;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
/*
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
*/



/*foreach(var file in new DirectoryInfo(".\\Sales").GetFiles())
{ 
    file.Delete();
}*/

app.MapGet("/", () => "Hello World!");

app.Run();


//leer todos los archivos del folder sales y enviar datos de 50 en 50 y al terminar eliminar los datos
// usar paralelismo

