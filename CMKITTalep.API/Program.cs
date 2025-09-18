using Microsoft.EntityFrameworkCore;
using CMKITTalep.DataAccess.Context;
using CMKITTalep.DataAccess.Interfaces;
using CMKITTalep.DataAccess.Repositories;
using CMKITTalep.Business.Interfaces;
using CMKITTalep.Business.Services;
using CMKITTalep.Entities;
using CMKITTalep.API.Models;
using CMKITTalep.API.Services;
using CMKITTalep.API.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "CMK IT Talep API", Version = "v1" });
    
    // JWT Authentication için Swagger yapılandırması
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement()
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:3001", "http://localhost:5000", "http://localhost:5001", "http://localhost:5173", "https://localhost:5173", "https://localhost:7097", "https://localhost:7098", "http://localhost:7097", "http://localhost:7098")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials() // SignalR için gerekli
              .SetIsOriginAllowedToAllowWildcardSubdomains(); // Subdomain desteği
    });
});

// Database Configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Dependency Injection Configuration
// Repositories
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IUserTypeRepository, UserTypeRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRequestTypeRepository, RequestTypeRepository>();
builder.Services.AddScoped<ISupportTypeRepository, SupportTypeRepository>();
builder.Services.AddScoped<IRequestStatusRepository, RequestStatusRepository>();
builder.Services.AddScoped<IPriorityLevelRepository, PriorityLevelRepository>();
builder.Services.AddScoped<IRequestRepository, RequestRepository>();
builder.Services.AddScoped<IRequestResponseRepository, RequestResponseRepository>();
builder.Services.AddScoped<IMessageReadStatusRepository, MessageReadStatusRepository>();
builder.Services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();

// Services
builder.Services.AddScoped(typeof(IGenericService<>), typeof(GenericService<>));
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IUserTypeService, UserTypeService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRequestTypeService, RequestTypeService>();
builder.Services.AddScoped<ISupportTypeService, SupportTypeService>();
builder.Services.AddScoped<IRequestStatusService, RequestStatusService>();
builder.Services.AddScoped<IPriorityLevelService, PriorityLevelService>();
builder.Services.AddScoped<IRequestService, RequestService>();
builder.Services.AddScoped<IRequestResponseService, RequestResponseService>();
builder.Services.AddScoped<IMessageReadStatusService, MessageReadStatusService>();
builder.Services.AddScoped<IPasswordResetTokenService, PasswordResetTokenService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// SMTP Configuration
var smtpSettings = builder.Configuration.GetSection("SmtpSettings").Get<SmtpSettings>()!;
builder.Services.AddSingleton(smtpSettings);

// JWT Configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()!;
builder.Services.AddSingleton(jwtSettings);
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
            ClockSkew = TimeSpan.FromMinutes(5), // 5 dakika tolerans
            RequireExpirationTime = true,
            RequireSignedTokens = true,
            ValidateActor = false,
            ValidateTokenReplay = false
        };
        
        // SignalR için JWT yapılandırması
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var path = context.HttpContext.Request.Path;
                
                // SignalR hub'ı için token kontrolü
                if (path.StartsWithSegments("/messageHub"))
                {
                    // Query parameter'dan token al
                    var accessToken = context.Request.Query["access_token"];
                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        context.Token = accessToken;
                        Console.WriteLine("🔑 SignalR Token from query received");
                        Console.WriteLine($"🔑 SignalR Path: {path}");
                        return Task.CompletedTask;
                    }
                    
                    // Authorization header'dan token al
                    var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                    if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
                    {
                        var tokenString = authHeader.Substring("Bearer ".Length).Trim();
                        context.Token = tokenString;
                        Console.WriteLine("🔑 SignalR Token from header received");
                        Console.WriteLine($"🔑 SignalR Path: {path}");
                        return Task.CompletedTask;
                    }
                    
                    Console.WriteLine($"🔑 SignalR Token not found - Path: {path}");
                }
                
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"❌ JWT Authentication Failed: {context.Exception?.Message}");
                Console.WriteLine($"❌ JWT Authentication Failed - Path: {context.HttpContext.Request.Path}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine($"✅ JWT Token Validated - User: {context.Principal?.Identity?.Name}");
                Console.WriteLine($"✅ JWT Claims count: {context.Principal?.Claims?.Count() ?? 0}");
                if (context.Principal?.Claims != null)
                {
                    foreach (var claim in context.Principal.Claims)
                    {
                        Console.WriteLine($"✅ JWT Claim: {claim.Type} = {claim.Value}");
                    }
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// SignalR Configuration
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
    options.HandshakeTimeout = TimeSpan.FromSeconds(15);
    options.MaximumReceiveMessageSize = 1024 * 1024;
    options.StreamBufferCapacity = 10;
})
.AddJsonProtocol(options =>
{
    options.PayloadSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.PayloadSerializerOptions.WriteIndented = false;
});

// Background Service for cleaning up inactive users

var app = builder.Build();

// Ensure database is created and apply migrations
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        // Try to apply migrations first
        context.Database.Migrate();
    }
    catch
    {
        // If migrations fail, ensure database is created
        context.Database.EnsureCreated();
    }
    
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowAll");

// CORS debugging
app.Use(async (context, next) =>
{
    Console.WriteLine($"CORS Request - Origin: {context.Request.Headers.Origin}, Method: {context.Request.Method}, Path: {context.Request.Path}");
    await next();
});

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// SignalR Hub mapping
app.MapHub<MessageHub>("/messageHub");
// Authorization hub içinde kontrol edilecek

app.Run();
