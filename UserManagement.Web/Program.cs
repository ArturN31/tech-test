using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Westwind.AspNetCore.Markdown;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Configure Entity Framework Core with InMemory Database
builder.Services.AddDbContext<UserManagement.Data.DataContext>(options =>
{
    options.UseInMemoryDatabase("UserManagement.Data.DataContext");
});

// Register services
builder.Services.AddScoped<UserManagement.Services.Domain.Interfaces.IUserService, UserManagement.Services.Domain.Implementations.UserService>();
builder.Services.AddScoped<UserManagement.Services.Domain.Interfaces.ILogService, UserManagement.Services.Implementations.LogService>();

// Add services to the container.
builder.Services
    .AddDataAccess()
    .AddDomainServices()
    .AddMarkdown()
    .AddControllersWithViews();

var app = builder.Build();

app.UseMarkdown();

app.UseHsts();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapDefaultControllerRoute();

app.Run();
