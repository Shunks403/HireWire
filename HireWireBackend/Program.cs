
using System.Text;
using AutoMapper;
using HireWireBackend.Core.Interfaces.IServices;
using HireWireBackend.Core.Services;
using HireWireBackend.Mapper;
using HireWireBackend.Storage.Context;
using LibraryManegerBackend.Core.Interfaces;
using LibraryManegerBackend.Storage;
using MessangerBackend.Core.Interfaces;
using MessangerBackend.Core.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Local");
var SessionTimeout = (int)builder.Configuration.GetValue(typeof(int), "SessionTimeout");
var TokenKey = builder.Configuration.GetValue<string>("TokenKey");

builder.Services.AddDbContext<ApplicationDbContext>(opt => opt.UseSqlServer((connectionString)).UseLazyLoadingProxies());

builder.Services.AddTransient<IRepository, Repository>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IPasswordHasher, PasswordHasher>();


builder.Services.AddControllers();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(SessionTimeout);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddDistributedMemoryCache();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenKey!)),
            ValidateIssuer = false,
            ValidateAudience = false,
        };
        options.Events = new JwtBearerEvents()
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies["token"];
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("ApplicantOnly", policy => policy.RequireRole("Applicant"));
    
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Program));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");


app.UseSession();
app.UseAuthorization();

app.MapControllers();

app.Run();