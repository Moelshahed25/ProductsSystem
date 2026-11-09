using Microsoft.EntityFrameworkCore;
using ProductsApi.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// الـ MVC بيكلم الـ API من السيرفر (server-to-server) فمش محتاجين CORS.

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // JSON مواصفات الـ API على: /openapi/v1.json
    app.MapOpenApi();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
