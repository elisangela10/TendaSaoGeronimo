using CasaDeAxe.Application.Interfaces;
using CasaDeAxe.Application.Service;
using CasaDeAxe.Domain.Interfaces;
using CasaDeAxe.Infrastructure.Data;
using CasaDeAxe.Infrastructure.Repositories;
using CasaDeAxe.Infrastructure.Services;
using CasaDeAxeAPI.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 🔌 Connection String
var defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(defaultConnection))
{
    throw new InvalidOperationException("ConnectionStrings:DefaultConnection não configurada.");
}

// 🔐 JWT
var jwtSecret = builder.Configuration["JwtSettings:SecretKey"];

if (string.IsNullOrWhiteSpace(jwtSecret))
{
    throw new InvalidOperationException("JwtSettings:SecretKey não configurada.");
}

// 🌐 CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 🔐 Auth JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

builder.Services.AddAuthorization();

// 🧠 Services
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGiraRepository, GiraRepository>();
builder.Services.AddScoped<IGiraService, GiraService>();

// 📦 Controllers / Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CasaDeAxeAPI",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando Bearer",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// 🩺 HealthCheck
builder.Services.AddHealthChecks();

// 🧾 Validation padrão
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var problem = new ValidationProblemDetails(context.ModelState)
        {
            Title = "Erro de validação",
            Status = StatusCodes.Status400BadRequest
        };

        return new BadRequestObjectResult(problem);
    };
});

// 🗄️ Banco
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(defaultConnection));

var app = builder.Build();

// 🔌 Porta dinâmica (Render)
var port = Environment.GetEnvironmentVariable("PORT") ?? "500";
app.Urls.Add($"http://*:{port}");

// 🧠 Teste de conexão + migrations
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    try
    {
        await db.Database.OpenConnectionAsync();
        await db.Database.CloseConnectionAsync();
        logger.LogInformation("Banco conectado com sucesso");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Erro ao conectar no banco");
        throw;
    }

    var applyMigrations = app.Configuration.GetValue<bool>("Database:ApplyMigrationsOnStartup");
    if (applyMigrations)
    {
        await db.Database.MigrateAsync();
    }
}

// 🔥 Middlewares
app.UseGlobalExceptionHandling();

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();
app.UseCors("AllowAll");

// ⚠️ evita loop no Render
if (!app.Environment.IsDevelopment())
{
    // opcional: deixa comentado se der problema
    // app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();