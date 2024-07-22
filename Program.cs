using Microsoft.EntityFrameworkCore;
using PROYECTO_PRUEBA.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using PROYECTO_PRUEBA.Models;
using PROYECTO_PRUEBA.Custom;
using Microsoft.CodeAnalysis.Options;
using Google.Apis.Auth;
using Newtonsoft.Json;
using System.Net.Http;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Variable para la cadena de conexion
var connectionString = builder.Configuration.GetConnectionString("Connection");

//Registrar servicio para la conexion
builder.Services.AddDbContext<AppDbContext> (options=> options.UseSqlServer (connectionString));

builder.Services.AddSingleton<Utilidades>();

builder.Services.AddAuthentication(config =>
{
    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(config =>
{
    config.RequireHttpsMetadata = false;
    config.SaveToken = true;
    config.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateAudience = false,
        ValidateIssuer = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        ValidIssuer = "https://accounts.google.com",
        ValidAudience = "159134234283-50bbfc07s291e2vnalpa0cgvgr0el23d.apps.googleusercontent.com",
        IssuerSigningKeys = GetGoogleSigningKeys(),
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

IEnumerable<SecurityKey> GetGoogleSigningKeys()
{
    using var httpClient = new HttpClient();
    var response = httpClient.GetStringAsync("https://www.googleapis.com/oauth2/v3/certs").Result;
    var keys = JsonConvert.DeserializeObject<JsonWebKeySet>(response);
    return keys.Keys;
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("newPolicy", app =>
    {
        app.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("newPolicy");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
