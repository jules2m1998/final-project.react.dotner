using Auth.API.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Auth.API.Contants;
using Auth.API.Domain;
using System.Reflection;
using Auth.API.Application.Contracts.Infrastructure.JwtService;
using Auth.API.Infrastructure.JwtServices;
using Auth.API.Application.Contracts.Persistence;
using Auth.API.Persistance.Repositories;
using Auth.API.Models;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Auth.API.Application.Contracts.Infrastructure.ContextAccessor;
using Auth.API.Infrastructure.ContextAccessors;
using Api.Common;

namespace Auth.API;

public static class StartupExtension
{
    public static IServiceCollection AddAuthServices(this IServiceCollection @this, IConfiguration configuration)
    {
        @this.Configure<JwtSettingModel>(configuration.GetSection(nameof(JwtSettingModel)));

        var jwtSetting = new JwtSettingModel();
        configuration.GetSection(nameof(JwtSettingModel)).Bind(jwtSetting);

        @this.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<AuthDbContext>()
            .AddDefaultTokenProviders();

        @this.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Management", Version = "v1" });
            options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = JwtBearerDefaults.AuthenticationScheme
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
        @this
            .AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSetting.Secret)),
                    ValidateLifetime = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ClockSkew = TimeSpan.Zero
                };
            });
        @this.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireClaim(Roles.MANAGER));
        });
        @this.AddDbContext<AuthDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });


        return @this;
    }

    public static IServiceCollection AddServices(this IServiceCollection @this)
    {

        // Mediatr
        @this.AddMediatR(options => options.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        @this.AddAutoMapper(Assembly.GetExecutingAssembly());


        @this.AddScoped<IJwtService, JwtService>();
        @this.AddScoped<IAuthRepository, AuthRepository>();
        @this.AddScoped<IContextAccessor, ContextAccessor>();

        @this.AddCommonServices();

        return @this;
    }

    public static async Task<WebApplication> ApplyAuthApplication(this WebApplication @this)
    {
        @this.CommonConfigureApp();
        using var scope = @this.Services.CreateScope();
        try
        {
            var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
            if (context is not null)
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.MigrateAsync();
                await context.Database.EnsureCreatedAsync();

            }
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var roles = new List<string>()
            {
                Roles.MANAGER,
                Roles.CLIENT
            };

            foreach (var roleName in roles)
            {
                var role = new IdentityRole(roleName);
                await roleManager.CreateAsync(role);
            }


            var manager = new ApplicationUser
            {
                UserName = "manager@mail.com",
                Email = "manager@mail.com",
                Name = "manager",
                PhoneNumber = "1234567890",
            };


            var client = new ApplicationUser
            {
                UserName = "client@mail.com",
                Email = "client@mail.com",
                Name = "client",
                PhoneNumber = "1234567891",
            };

            var result = await userManager.CreateAsync(manager, "Manager123@test-password");
            await userManager.CreateAsync(client, "Manager123@test-password");

            await userManager.AddToRoleAsync(manager, Roles.MANAGER);
            await userManager.AddToRoleAsync(client, Roles.CLIENT);
        }
        catch(Exception ex)
        {
            var logger = scope.ServiceProvider.GetService<ILogger<Program>>();
            logger?.LogError(ex, "An error occurred during database initialisation");
        }

        return @this;
    }
}
