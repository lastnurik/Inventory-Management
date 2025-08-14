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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret))
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

app.UseExceptionHandler(_ => { });
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();