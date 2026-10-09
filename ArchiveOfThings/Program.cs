using ArchiveOfThings;
using ArchiveOfThings.Components;
using ArchiveOfThings.Data;
using ArchiveOfThings.Model.Features.Collections;
using ArchiveOfThings.Model.Features.Dashboard;
using ArchiveOfThings.Model.Features.Items;
using ArchiveOfThings.Model.Features.Login;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add Framework Services
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

// Add Database Context
builder.Services.AddDbContext<ArchiveDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Application State & Feature Handlers
builder.Services.AddScoped<SessionState>();
builder.Services.AddScoped<LoginHandler>();
builder.Services.AddScoped<DashboardHandler>();
builder.Services.AddScoped<CollectionHandler>();
builder.Services.AddScoped<ItemsHandler>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseAntiforgery();
app.UseStaticFiles();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();
