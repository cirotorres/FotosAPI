using FotosAPI;
using FotosAPI.Data;
using FotosAPI.Models;
using FotosAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using static FotosAPI.Services.AuthClaimsService;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("https://0.0.0.0:7147", "http://0.0.0.0:5216");


// CORS (Cross-Origin Resource Sharing)((requisição de origem cruzada)
// Para permitir que o front-end acesse a API, pois o front e a API possuem domínios diferentes. 
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Adicionado a interface de tokens no Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
    {
        new OpenApiSecurityScheme
        {
        Reference = new OpenApiReference
            {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
            },
            Scheme = "oauth2",
            Name = "Bearer",
            In = ParameterLocation.Header,

        },
        new List<string>()
        }
    });


});

builder.Services.AddTransient<IPhotoRepository, PhotoRepository>(); // Serviço Repositório Banco de Dados
builder.Services.AddTransient<ITokenService, TokenService>(); // Serviço TOKEN
builder.Services.AddTransient<IImageProcessingService, ImageProcessingService>(); // Serviço Process IMG 
builder.Services.AddTransient<IDeleteObjService, DeleteObjService>(); // Serviço de Deletar Objeto
builder.Services.AddTransient<IAuthClaimsService, AuthClaimsService> (); // Serviço extração Claims
builder.Services.AddHttpContextAccessor(); // Para a extração de Claims do "User" na área de "AuthClaimsService.cs".
builder.Services.AddTransient<IFindObjService, FindObjService>(); // Serviço encontrar Objeto.
builder.Services.AddTransient<IAllListService, AllListService>(); // Serviço lista todos os Objetos.
builder.Services.AddTransient<IViewObjService, ViewObjService>(); // Serviço visualiza Imagem do Objeto.

// Obtendo as configurações do JWT do appsettings.json
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];


// Configura as autenticações do JWT
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = issuer,
        ValidAudience = audience
    };
});

var app = builder.Build();

// CORS (Cross-Origin Resource Sharing)
app.UseCors("AllowReactApp");


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    //Faz com que o Swagger abra diretamente pelo localhost, use no "app.UseSwaggerUI();"

    //options =>
    //{
    //    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    //    options.RoutePrefix = string.Empty;
    //}
}

//app.UseHttpsRedirection();

// Ativa o middleware de autenticacao e autorizacao
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
