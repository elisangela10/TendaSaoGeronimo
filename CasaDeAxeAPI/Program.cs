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
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(defaultConnection))
{
    throw new InvalidOperationException("ConnectionStrings:DefaultConnection não configurada. Defina via variável de ambiente ou secret manager.");
}

var jwtSecret = builder.Configuration["JwtSettings:SecretKey"];
if (string.IsNullOrWhiteSpace(jwtSecret))
{
    throw new InvalidOperationException("JwtSettings:SecretKey não configurada. Defina via variável de ambiente ou secret manager.");
}

// Configuração do CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configuração da autenticação JWT
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

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("JwtAuthentication");

                logger.LogWarning(context.Exception, "Falha de autenticação JWT para {Path}", context.HttpContext.Request.Path);
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHealthChecks();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var validationProblem = new ValidationProblemDetails(context.ModelState)
        {
            Title = "Erro de validação",
            Status = StatusCodes.Status400BadRequest,
            Detail = "Um ou mais campos estão inválidos.",
            Instance = context.HttpContext.Request.Path
        };

        validationProblem.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
        return new BadRequestObjectResult(validationProblem);
    };
});

// Configuração do Swagger/OpenAPI
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CasaDeAxeAPI",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando o esquema Bearer. Exemplo: \"Authorization: Bearer {token}\"",
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

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

// Configuração do banco de dados PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(defaultConnection));

// Injeção de dependências dos repositórios e serviços
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGiraRepository, GiraRepository>();
builder.Services.AddScoped<IGiraService, GiraService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    if (!await dbContext.Database.CanConnectAsync())
    {
        logger.LogError("Falha ao conectar no banco de dados durante o startup.");
        throw new InvalidOperationException("Não foi possível conectar ao banco de dados.");
    }

    logger.LogInformation("Conexão com banco validada com sucesso no startup.");

    // Compatibilidade de schema da tabela Giras (ambientes sem migration recente)
    await dbContext.Database.ExecuteSqlRawAsync(
        "ALTER TABLE \"Giras\" ADD COLUMN IF NOT EXISTS \"Cura\" text NOT NULL DEFAULT ''");

    await dbContext.Database.ExecuteSqlRawAsync(
        "ALTER TABLE \"Giras\" ADD COLUMN IF NOT EXISTS \"Status\" integer NOT NULL DEFAULT 0");

    await dbContext.Database.ExecuteSqlRawAsync(
        "ALTER TABLE \"Giras\" ADD COLUMN IF NOT EXISTS \"DataCriacao\" timestamp with time zone NOT NULL DEFAULT NOW()");

    var applyMigrations = app.Configuration.GetValue<bool>("Database:ApplyMigrationsOnStartup");
    if (applyMigrations)
    {
        logger.LogInformation("Aplicando migrations no startup.");
        await dbContext.Database.MigrateAsync();
    }
}

app.UseGlobalExceptionHandling();

app.UseSwagger(c =>
{
    c.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi2_0;
});
app.UseSwaggerUI();

app.UseRouting();
app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
