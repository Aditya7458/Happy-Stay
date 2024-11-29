using Cozy.Data;
using Cozy.Repositories.Interfaces;
using Cozy.Repositories.Implementations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Cozy
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure services

            // Database context registration
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")
                )
            );

            // Dependency injection for repositories
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IHotelRepository, HotelRepository>();

            // Configure JWT authentication
            var secretKey = "YourSecureSecretKeyHere123!"; // Replace with a strong, secure secret key
            var key = Encoding.UTF8.GetBytes(secretKey);
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true; // Enforce HTTPS in production
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "CozyAppIssuer",  // Replace with your application-specific issuer
                    ValidAudience = "CozyAppAudience", // Replace with your application-specific audience
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

            // Register controllers
            builder.Services.AddControllers();

            // Add Swagger/OpenAPI configuration for API documentation
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Middleware pipeline configuration

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Enable authentication and authorization middleware
            app.UseAuthentication();
            app.UseAuthorization();

            // Map API controllers
            app.MapControllers();

            app.Run();
        }
    }
}
