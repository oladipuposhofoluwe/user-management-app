using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UserManagement.Infrastructure.Persistence;
using UserManagement.Infrastructure.Services;
using UserManagement.Application.Interfaces.Services;
using Serilog;
using Hangfire;
using Hangfire.MemoryStorage;
using UserManagement.Domain.Entities;
using Hangfire.MySql;


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("User Management Application is starting up");

    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseMySql(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
        ));

    // builder.Services.AddHangfire(config => config.UseMemoryStorage());

    builder.Services.AddScoped<IBackgroundJobService, BackgroundJobService>();

   builder.Services.AddHangfire(config =>
    {
        config
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseStorage(new MySqlStorage(
                builder.Configuration.GetConnectionString("DefaultConnection"),
                new MySqlStorageOptions
                {
                    TablesPrefix = "Hangfire",
                    QueuePollInterval = TimeSpan.FromSeconds(15)
                }));
    });


    builder.Services.AddHangfireServer();

    builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
    builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));


    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

    })
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings!.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
        };
    });

    builder.Services.AddControllers();
    builder.Services.AddScoped<IEmailService, EmailService>();
    builder.Services.AddDbContext<AppDbContext>();
    builder.Services.AddSwaggerGen();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddAuthentication();
    builder.Services.AddAuthorization();
    builder.Services.AddScoped<IPasswordHash, PasswordHash>();
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<IUserService, UserService>();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();


    builder.Services.AddAuthorization();


    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        var dbSeeder = new DbSeeder(scope.ServiceProvider.GetRequiredService<AppDbContext>());
        await dbSeeder.SeedAsync();
    }
    app.UseMiddleware<ExceptionHandlingMiddleware>();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseHangfireDashboard();
    app.UseHangfireDashboard("/hangfire");


    app.MapControllers();

    app.Run();
}catch (Exception ex)
{
    Log.Fatal(ex, "User Management Application start-up failed");
}
finally
{
    Log.CloseAndFlush();
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
