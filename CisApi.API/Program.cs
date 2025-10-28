using System.Text;
using CisApi.Core.Interfaces;
using CisApi.Infrastructure.Repositories; // Aponta para os novos repositórios Mongo
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver; // Adicionado para o MongoDB

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// --- Início da Configuração ---

builder.Services.AddControllers();
builder.Services.AddHttpClient(); // Mantido para a API de login Java

// --- Início da Configuração do MongoDB ---

// 1. Registar o Cliente MongoDB como Singleton
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var connectionString = configuration["MongoDbSettings:ConnectionString"];
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("ERRO CRÍTICO: 'MongoDbSettings:ConnectionString' não foi encontrada no appsettings.json.");
    }
    return new MongoClient(connectionString);
});

// 2. Registar o IMongoDatabase (injeta a base de dados específica)
builder.Services.AddScoped<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var dbName = configuration["MongoDbSettings:DatabaseName"];
    if (string.IsNullOrEmpty(dbName))
    {
        throw new InvalidOperationException("ERRO CRÍTICO: 'MongoDbSettings:DatabaseName' não foi encontrada no appsettings.json.");
    }
    return client.GetDatabase(dbName);
});

// 3. Apagar a configuração antiga do DbContext
// var connectionString = configuration.GetConnectionString("DefaultConnection"); // REMOVIDO
// builder.Services.AddDbContext<CisDbContext>(...); // REMOVIDO

// 4. Registar os novos repositórios do MongoDB
builder.Services.AddScoped<ITopicRepository, MongoTopicRepository>();
builder.Services.AddScoped<IIdeaRepository, MongoIdeaRepository>();
builder.Services.AddScoped<IVoteRepository, MongoVoteRepository>();

// --- Fim da Configuração do MongoDB ---

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// --- Configuração JWT (Mantida) ---
var jwtSecret = configuration["Jwt:Secret"];
if (string.IsNullOrEmpty(jwtSecret))
{
    throw new InvalidOperationException("ERRO CRÍTICO: O segredo JWT (Jwt:Secret) não foi encontrado ou está vazio no appsettings.json");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// --- Configuração do Swagger (Mantida) ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CIS API", Version = "v1" });
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
            new OpenApiScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// --- Bloco de Migração do EF Core (REMOVIDO) ---
// O MongoDB é schemaless e não precisa deste bloco de migração.
// using (var scope = app.Services.CreateScope())
// {
//     var dbContext = ...
//     dbContext.Database.Migrate();
// }

// --- Início do Pipeline HTTP ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CIS API V1");
        c.RoutePrefix = string.Empty;
    });
}

//app.UseHttpsRedirection(); // Mantido comentado como no seu original
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();