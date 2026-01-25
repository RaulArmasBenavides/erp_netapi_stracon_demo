using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using PaymentServiceNet.Application.Interfaces;
using PaymentServiceNet.Application.Services;
using PaymentServiceNet.Core.Interfaces;
using PaymentServiceNet.Core.IRepositorio;
using PaymentServiceNet.Repositorio;
using System.Text;

namespace PaymentServiceNet.Extensions
{
    public static class ApplicationServicesExtensions
    {

        public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
         
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<ISupplierRepository, SupplierRepository>(); 
            services.AddAuthorization(options => options.DefaultPolicy =
            new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme).RequireAuthenticatedUser().Build());
            var key = configuration.GetValue<string>("ApiSettings:Secreta");
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                };
            });
        }
    }
}
