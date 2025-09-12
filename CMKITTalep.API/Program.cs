using Microsoft.EntityFrameworkCore;
using CMKITTalep.DataAccess.Context;
using CMKITTalep.DataAccess.Interfaces;
using CMKITTalep.DataAccess.Repositories;
using CMKITTalep.Business.Interfaces;
using CMKITTalep.Business.Services;
using CMKITTalep.Entities;
using CMKITTalep.API.Models;
using CMKITTalep.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
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
builder.Services.AddScoped<IRequestResponseTypeRepository, RequestResponseTypeRepository>();
builder.Services.AddScoped<IRequestRepository, RequestRepository>();
builder.Services.AddScoped<IRequestResponseRepository, RequestResponseRepository>();
builder.Services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();

// Services
builder.Services.AddScoped(typeof(IGenericService<>), typeof(GenericService<>));
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IUserTypeService, UserTypeService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRequestTypeService, RequestTypeService>();
builder.Services.AddScoped<ISupportTypeService, SupportTypeService>();
builder.Services.AddScoped<IRequestStatusService, RequestStatusService>();
builder.Services.AddScoped<IRequestResponseTypeService, RequestResponseTypeService>();
builder.Services.AddScoped<IRequestService, RequestService>();
builder.Services.AddScoped<IRequestResponseService, RequestResponseService>();
builder.Services.AddScoped<IPasswordResetTokenService, PasswordResetTokenService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// SMTP Configuration
var smtpSettings = builder.Configuration.GetSection("SmtpSettings").Get<SmtpSettings>()!;
builder.Services.AddSingleton(smtpSettings);

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

app.MapControllers();

app.Run();
