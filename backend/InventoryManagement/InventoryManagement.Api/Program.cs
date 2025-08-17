using InventoryManagement.Api.Handlers;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Application.Services;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Infrastructure.Options;
using InventoryManagement.Infrastructure.Persistance;
using InventoryManagement.Infrastructure.Processors;
using InventoryManagement.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;
using InventoryManagement.Infrastructure.Seeders;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection(JwtOptions.JwtOptionsKey));

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder.WithOrigins(
            "https://jolly-dune-03325f203.2.azurestaticapps.net",
            "http://localhost:5173",
            "https://inventory-management-app-gvaphyaufbfsa3d0.germanywestcentral-01.azurewebsites.net"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
        .SetIsOriginAllowedToAllowWildcardSubdomains();
    });
});

builder.Services.AddIdentity<User, IdentityRole<Guid>>(opt =>
{
    opt.Password.RequireDigit = false;
    opt.Password.RequireLowercase = false;
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequireUppercase = false;
    opt.Password.RequiredLength = 1;
    opt.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<IAuthTokenProcessor, AuthTokenProcessor>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAccountService, AccountService>();

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddCookie().AddGoogle(options =>
{
    var clientId = builder.Configuration["Authentication:Google:ClientId"];

    if (clientId == null)
    {
        throw new ArgumentNullException(nameof(clientId));
    }

    var clientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

    if (clientSecret == null)
    {
        throw new ArgumentNullException(nameof(clientSecret));
    }

    options.ClientId = clientId;
    options.ClientSecret = clientSecret;
    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    var jwtOptions = builder.Configuration.GetSection(JwtOptions.JwtOptionsKey)
        .Get<JwtOptions>() ?? throw new ArgumentException(nameof(JwtOptions));

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtOptions.Issuer,
        ValidAudience = jwtOptions.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret)),
        ClockSkew = TimeSpan.FromMinutes(5)
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["ACCESS_TOKEN"];
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

await app.SeedDatabaseAsync();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(opt =>
    {
        opt.WithTitle("Inventory Management Service API");
    });
}

app.UseCors("CorsPolicy");

app.Use((context, next) =>
{
    var origin = context.Request.Headers["Origin"].FirstOrDefault();
    if (!string.IsNullOrEmpty(origin))
    {
        context.Response.Headers["Access-Control-Allow-Origin"] = origin;
        context.Response.Headers["Access-Control-Allow-Credentials"] = "true";
        context.Response.Headers["Vary"] = "Origin";
    }
    return next();
});

app.UseExceptionHandler(_ => { });
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Use(async (context, next) =>
{
    Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
    Console.WriteLine($"Origin: {context.Request.Headers["Origin"]}");
    Console.WriteLine($"Cookies: {context.Request.Headers["Cookie"]}");

    await next();

    Console.WriteLine($"Response: {context.Response.StatusCode}");
    Console.WriteLine($"CORS Headers: {string.Join(", ", context.Response.Headers)}");
});

app.Run();