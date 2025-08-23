using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UserManagement.Data;
using UserManagement.Data.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Services.Implementations;
using UserManagement.Services.Interfaces;
using Microsoft.IdentityModel.JsonWebTokens;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add JWT Authentication
builder.Services.AddAuthentication(authOptions =>
{
    // Authentication configuration
    // Set default authentication scheme to JWT Bearer
    authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    // Set default challenge scheme to JWT Bearer
    authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    // Set default scheme to JWT Bearer
    authOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwtOptions =>
{
    // This event handler is the core of the blacklisting logic.
    // It runs every time a token is validated to check if its JTI is on the blacklist.
    jwtOptions.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            var blacklistService = context.HttpContext.RequestServices.GetRequiredService<ITokenBlacklistService>();
            var jti = context.Principal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

            if (string.IsNullOrEmpty(jti) || blacklistService.IsTokenBlacklisted(jti))
            {
                context.Fail("This token has been blacklisted.");
            }
            return Task.CompletedTask;
        }
    };

    // JWT Bearer configuration
    // Save the token in the authentication properties
    jwtOptions.SaveToken = true;
    // Disable HTTPS metadata requirement (for development purposes)
    jwtOptions.RequireHttpsMetadata = false;
    // Set token validation parameters
    jwtOptions.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
    {
        // Validate the signing key
        ValidateIssuer = true,
        // Issuer to validate against
        ValidIssuer = configuration["JWT:ValidIssuer"],
        // Validate the audience
        ValidateAudience = true,
        // Audience to validate against
        ValidAudience = configuration["JWT:ValidAudience"],
        // Security key for signing the token
        // This ensures that the token is valid and has not been tampered with
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]!))
    };
});

// Add Identity services to the container
// AddDefaultTokenProviders() is necessary for password reset tokens, etc.
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Register services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddSingleton<ITokenBlacklistService, TokenBlacklistService>();

// Register DbContext with InMemory Database
//builder.Services.AddDbContext<DataContext>(options =>
//    options.UseInMemoryDatabase("UserManagement.Data.DataContext"));

//Register the SQL Server
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        // This tells EF Core where to store the migration files
        b => b.MigrationsAssembly("UserManagement.API")));


builder.Services.AddScoped<IDataContext, DataContext>();

builder.Services.AddCors(corsOptions =>
{
    corsOptions.AddPolicy("BlazorCorsPolicy", policy =>
    {
        policy.WithOrigins("https://localhost:7256", "https://localhost:5116")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("BlazorCorsPolicy");

app.UseHttpsRedirection();

app.UseAuthentication();    
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<DataContext>();

    // This applies any pending migrations to the database
    context.Database.Migrate();

    // The seeder now runs only if the Users table is empty
    if (!context.Users.Any())
    {
        DataSeeder.SeedDatabase(context);
    }
}

app.Run();
