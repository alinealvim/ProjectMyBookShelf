using BlazorBootstrap;
using Microsoft.AspNetCore.Authentication.Cookies; // Import authentication cookie package
using Microsoft.EntityFrameworkCore;
using MyBookShelf.Components;
using MyBookShelf.Data;
using MyBookShelf.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddBlazorBootstrap();
// Add services to the container
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddControllers();

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IUserClaimService, UserClaimService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IProgressService, ProgressService>();

// Set up authentication cookie structure
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.Cookie.Name = "auth_token";
        options.LoginPath = "/login";        
        options.AccessDeniedPath = "/access-denied";
        options.LogoutPath = "/logout";

        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;

        options.ExpireTimeSpan = TimeSpan.FromDays(1);
        options.SlidingExpiration = true;
    });

// Configure DbContext
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection")));

// Configure HttpClient for API requests
builder.Services.AddHttpClient("BookAPI", client =>
{
    client.BaseAddress = new Uri("http://localhost:5293/"); 
});

// Register HttpClient as default scoped service
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("BookAPI"));

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
// Map Razor Components
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Map API endpoints
app.MapControllers(); 

app.Run();
