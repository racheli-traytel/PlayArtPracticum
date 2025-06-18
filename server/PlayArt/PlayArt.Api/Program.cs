using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PlayArt.Core.entities;
using PlayArt.Core;
using PlayArt.Core.Entities;
using PlayArt.Core.Interfaces;
using PlayArt.Core.Interfaces.Services_interfaces;
using PlayArt.Data;
using PlayArt.Data.Repositories;
using PlayArt.Data.Repository;
using PlayArt.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using PlayArt.Api;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using PlayArt.Core.Interfaces.IRepositories;
using Microsoft.OpenApi.Models;
using PlayArt.Sevice;

var builder = WebApplication.CreateBuilder(args);

// äåñôú sarai  sari updated


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

// äåñôú JWT Authentication
builder.Services.AddScoped<AuthService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

//// äåñôú äøùàåú îáåññåú-úô÷éãéí
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserorOrAdmin", policy => policy.RequireRole("User", "Admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
});

builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.AllowSynchronousIO = true;
});
// ? äåñôú ùéøåúéí ìôðé éöéøú ä-Application
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer Authentication with JWT Token",
        Type = SecuritySchemeType.Http
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});
builder.Services.AddDbContext<DataContext>();

//? äåñôú ä-Repositories
builder.Services.AddScoped<IRepository<User>, UserRepository>();
builder.Services.AddScoped<IRepository<Drawing>, DrawingRepository>();
builder.Services.AddScoped<IRepository<PaintedDrawing>, PaintedDrawingRepository>();
builder.Services.AddScoped<IDrawingRepository, DrawingRepository>();
builder.Services.AddScoped<IDrawingService, DrawingService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();

//?äåñôú ä-Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDrawingService, DrawingService>();
builder.Services.AddScoped<IPaintedDrawingService, PaintedDrawingService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRoleService, UserRoleService>();

// ? äåñôú AutoMapper
builder.Services.AddAutoMapper(typeof(ProfileMapping), typeof(ProfileMappingPostModel));
builder.Services.AddDbContext<DataContext>(option =>
{
    option.UseSqlServer("Data Source = DESKTOP-SSNMLFD; Initial Catalog = PlayArt; Integrated Security = true; Trusted_Connection = SSPI; MultipleActiveResultSets = true; TrustServerCertificate = true;");
}
 );
// ?? òëùéå éåöøéí àú ä-Application
var app = builder.Build();

// ? Middleware ìäâãøú ä-API
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // îôòéì àú Swagger
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "PlayArt API V1");
    });
}

app.UseHttpsRedirection();

// äôòìú ä-CORS
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
