using backend.Db;
using backend.Models;
using backend.Services;
using Microsoft.EntityFrameworkCore;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Configurar Stripe
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

// Configurar EF Core con PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// CORS para Angular
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", p =>
        p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

builder.Services.Configure<StripeModel>(builder.Configuration.GetSection("Stripe"));
//Stripe
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<ChargeService>();
builder.Services.AddScoped<ProductService>();

// Inyectar servicios
builder.Services.AddScoped<ClerkService>();
builder.Services.AddScoped<OrganizationService>();
builder.Services.AddScoped<OrganizationService>();



builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngular");
app.MapControllers();
app.Run();
