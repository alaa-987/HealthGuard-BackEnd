using HealthGuard.Core;
using HealthGuard.Core.Entities;
using HealthGuard.Core.Entities.Identity;
using HealthGuard.Core.Repository.contract;
using HealthGuard.Core.Services.contract;
using HealthGuard.GradProject.Extension;
using HealthGuard.GradProject.Helpers;
using HealthGuard.GradProject.MiddleWare;
using HealthGuard.Service.AuthService;
using HealthGuard.Service.EmailService;
using HealthGuard.Service.OrderService;
using HealthGuard.Service.PaymentService;
using HealthGurad.Repository;
using HealthGurad.Repository.Data;
using HealthGurad.Repository.Data.Identity;
using HealthGurad.Repository.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Configuration;
using System.Net.Mail;
using System.Text;

namespace HealthGuard.GradProject
{
    public class Program
    {
        static Program()
        {
            
        }

        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<StoreContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            builder.Services.AddDbContext<AppIDentityDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
            });
            builder.Services.AddSingleton<IConnectionMultiplexer>((serviceProvider) =>
            {
                var connection = builder.Configuration.GetConnectionString("Redis");
                return ConnectionMultiplexer.Connect(connection);
            });
            builder.Services.AddApplicationServices();
            builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<AppIDentityDbContext>().AddDefaultTokenProviders();
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWT:ValidAudiance"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:AuthKey"] ?? string.Empty))
                };
            });
            builder.Services.AddScoped(typeof(IAuthService), typeof(AuthService));
            builder.Services.AddScoped(typeof(IPaymentService), typeof(PaymentService));
            builder.Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            builder.Services.AddScoped(typeof(IOrderService), typeof(OrderService));
            builder.Services.Configure<EmailSetting>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.AddTransient<IEmailService, EmailService>();
            builder.Services.AddLogging();
            // builder.Services.AddTransient<SmtpClient>();
            builder.Services.AddCors(Options =>
            {
                Options.AddPolicy("MyPolicy", options =>
                {
                    options.AllowAnyHeader();
                    options.AllowAnyMethod();
                    options.WithOrigins("https://localhost:3000", "https://localhost:3001");
                });
            });

            var app = builder.Build();
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var _dbContext = services.GetRequiredService<StoreContext>();
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            var _identityDbContext = services.GetRequiredService<AppIDentityDbContext>();
            
            try
            {
                await _dbContext.Database.MigrateAsync();
                await StoreContextSeed.SeedAsync(_dbContext);
                await _identityDbContext.Database.MigrateAsync();
                var _userManager = services.GetRequiredService<UserManager<AppUser>>();
                var _roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                await AppIdentityDbContextSeed.SeedUsersAsync(_userManager, _roleManager);
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "An Error Occurred During Migration");
            }
            // Configure the HTTP request pipeline.
            app.UseMiddleware<ApiEcxeptionMiddleWare>();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwaggarMiddleWare();
            }
            app.UseStatusCodePagesWithReExecute("/Errors/{0}");
            app.UseHttpsRedirection();

            // app.UseAuthorization();
            app.UseStaticFiles();
            app.UseCors("MyPolicy");
            app.UseAuthorization();
            app.UseAuthentication();
            app.MapControllers();

            app.Run();
        }
    }
}
