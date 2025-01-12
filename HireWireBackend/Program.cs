
using System.Text;
using AutoMapper;
using HireWireBackend.Core.Interfaces.ILoggers;
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

builder.Services.AddDbContext<ApplicationDbContext>(opt => opt.UseSqlServer(connectionString).UseLazyLoadingProxies());

builder.Services.AddTransient<IRepository, Repository>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IPasswordHasher, PasswordHasher>();
builder.Services.AddTransient<IEmployerService, EmployerService>();
builder.Services.AddTransient<IJobVacancyService,JobVacancyService>();
builder.Services.AddTransient<IApplicantService, ApplicantService>();
builder.Services.AddTransient<IJobApplicationService, JobApplicationService>();
builder.Services.AddTransient<IJobVacancyTagService,JobVacancyTagService >();
builder.Services.AddTransient<ITagService,TagService >();
builder.Services.AddTransient<IBlobStorageService, BlobStorageService>();
builder.Services.AddSingleton<IBlobLogger>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var azureBlobConfig = configuration.GetSection("AzureBlobStorage");

    var connectionString = azureBlobConfig["ConnectionString"];
    var containerName = azureBlobConfig["ContainerLogger"];

    return new AzureBlobLogger(connectionString, containerName);
});


builder.Services.AddControllers();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});


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
                
                var authorizationHeader = context.Request.Headers["Authorization"].ToString();
                if (authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    context.Token = authorizationHeader.Substring("Bearer ".Length).Trim();
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("ApplicantOnly", policy => policy.RequireRole("Applicant"));
    options.AddPolicy("EmployerOnly", policy => policy.RequireRole("Employer"));
    
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.WithOrigins("http://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Program));
var app = builder.Build();


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