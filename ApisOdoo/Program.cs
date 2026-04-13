using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using OdooCls.API.Attributes;
using OdooCls.Core.Entities;
using OdooCls.Core.Interfaces;
using OdooCls.Infrastucture.Repositorys;



var builder = WebApplication.CreateBuilder(args);

if (string.IsNullOrWhiteSpace(builder.Configuration["Authentication:Library"]))
{
    var envPath = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "..", ".env"));
    if (File.Exists(envPath))
    {
        foreach (var line in File.ReadAllLines(envPath))
        {
            var trimmed = line.Trim();
            if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith("#") || !trimmed.Contains('='))
                continue;

            var idx = trimmed.IndexOf('=');
            var key = trimmed[..idx].Trim();
            var value = trimmed[(idx + 1)..].Trim().Trim('"');

            if (key.Equals("LIBRERIA", StringComparison.OrdinalIgnoreCase) ||
                key.Equals("Authentication__Library", StringComparison.OrdinalIgnoreCase))
            {
                builder.Configuration["Authentication:Library"] = value;
            }
        }
    }
}

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IRegistroVentasRepository, RegistroVentasRepository>();
builder.Services.AddTransient<IRegistroComprasRepository, RegistrocomprasRepository>();
builder.Services.AddTransient<IRegistroClientesRepository, RegistroClientesRepository>();
builder.Services.AddTransient<IRegistroProveedoresRepository, RegistroProveedoresRepository>();
builder.Services.AddTransient<IRegistroArticulosRepository, RegistroArticulosRepository>();
builder.Services.AddTransient<IRegistroAlmacenesRepository, RegistroAlmacenesRepository>();
builder.Services.AddTransient<IRegistroMovimientosRepository, RegistroMovimientosRepository>();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<BearerAuthAttribute>(); // Aplica a toda la API
});

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errores = context.ModelState
            .Where(e => e.Value?.Errors.Count > 0)
            .Select(e => $"{e.Key}: {e.Value!.Errors.First().ErrorMessage}");

        var mensaje = errores.Any()
            ? $"Datos inválidos. {string.Join(" | ", errores)}"
            : "Datos inválidos en la solicitud";

        var response = new ApiResponse<object>(400, 1009, mensaje, null);
        return new BadRequestObjectResult(response);
    };
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT", // Aunque no uses JWT, esto es est�tico
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Introduce tu token como: Bearer {tu_token}"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
var app = builder.Build();

// Configurar puerto dinámicamente
var port = Environment.GetEnvironmentVariable("ASPNETCORE_PORT") ?? "8095";
app.Urls.Add($"http://0.0.0.0:{port}");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
