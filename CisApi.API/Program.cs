using System.Text;
using CisApi.Core.Interfaces;
using CisApi.Infrastructure.Data;
using CisApi.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models; // Adicione este using

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Adicionar serviços ao contêiner.
builder.Services.AddControllers();

// Configurar o DbContext para o MySQL
var connectionString = configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<CisDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Configurar Injeção de Dependência
builder.Services.AddScoped<ITopicRepository, TopicRepository>();

// Configurar AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Configurar autenticação JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSecret = configuration["Jwt:Secret"];
        if (string.IsNullOrEmpty(jwtSecret))
        {
            throw new InvalidOperationException("JWT Secret not configured in appsettings.json");
        }
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true
        };
    });

// Adicionar serviços de autorização
builder.Services.AddAuthorization();

// CORREÇÃO: Adicionar serviços do Swagger (Swashbuckle)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Adiciona o cadeado de "Authorize" na interface do Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira 'Bearer' [espaço] e o seu token JWT.\n\nExemplo: 'Bearer 12345abcdef'"
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
            new string[] {}
        }
    });
});


var app = builder.Build();

// Configurar o pipeline de requisições HTTP.
if (app.Environment.IsDevelopment())
{
    // CORREÇÃO: Usar os métodos do Swashbuckle
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CIS API V1");
        c.RoutePrefix = string.Empty; // Define o Swagger como a página inicial
    });
}

app.UseHttpsRedirection();

// Habilitar autenticação e autorização
app.UseAuthentication();
app.UseAuthorization();

// Mapear os controllers
app.MapControllers();

app.Run();