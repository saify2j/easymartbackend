// For Logger
using Easy.Logging.Extensions;
using EasyMart.API.Application.Common;
using EasyMart.API.Application.Common.Interfaces;
using EasyMart.API.Application.DTOs.Product;
using EasyMart.API.Application.Interfaces;
using EasyMart.API.Application.Interfaces.Services;
using EasyMart.API.Application.Services;
using EasyMart.API.Infrastructure.Persistence;
using EasyMart.API.Middleware;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwt = builder.Configuration.GetSection("Jwt");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwt["Key"]!)
            )
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SuperAdminPolicy",
        policy => policy.RequireRole("SuperAdmin"));

    options.AddPolicy("Anybody",
        policy => policy.RequireRole("Agent", "User"));
});


//builder.Logging.ClearProviders(); // optional - to remove default console if desired
builder.Logging.AddBufferedLogging(builder.Configuration);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: **Bearer {your JWT token}**"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();


// Middleware
builder.Services.AddScoped<RequestContext>();
builder.Services.AddScoped<IRequestContext>(sp =>
    sp.GetRequiredService<RequestContext>());

builder.Services.AddScoped<IValidator<ProductAddRequest>, ProductAddRequestValidator>();

// Infrastructure
builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Application
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IProductService, ProductService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<RequestIdMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
