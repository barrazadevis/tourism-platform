using Microsoft.EntityFrameworkCore;
using TourismPlatform.Data;
using TourismPlatform.Data.Services;
using TourismPlatform.API.Middleware;
using TourismPlatform.Data.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<TourismDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("TourismPlatform.API")));

// Services
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<IQuoteService, QuoteService>();
builder.Services.AddScoped<ITravelPlanService, TravelPlanService>();
builder.Services.AddHttpClient<IAuthClient, AuthClient>();
builder.Services.AddHttpClient();
// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://*.tourism-platform.com")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");

// Custom middleware
app.UseMiddleware<TenantMiddleware>();
app.UseMiddleware<AuthMiddleware>();

app.UseAuthorization();
app.MapControllers();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TourismDbContext>();
    context.Database.EnsureCreated();

    // Seed default tenant for development
    if (app.Environment.IsDevelopment())
    {
        await SeedDefaultTenant(context);
    }
}

app.Run();

async Task SeedDefaultTenant(TourismDbContext context)
{
    if (!await context.Tenants.AnyAsync(t => t.Subdomain == "default"))
    {
        var defaultTenant = new TourismPlatform.Core.Entities.Tenant
        {
            Id = Guid.NewGuid(),
            Name = "Demo Tourism Company",
            Subdomain = "default",
            Description = "Empresa de demostraci�n",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        context.Tenants.Add(defaultTenant);
        await context.SaveChangesAsync();
    }
}