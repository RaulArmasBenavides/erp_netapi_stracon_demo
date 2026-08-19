using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using SupplierServiceNet.Application.Dtos;
using SupplierServiceNet.Application.Interfaces;
using SupplierServiceNet.Application.Services;
using SupplierServiceNet.Application.Validators;
using SupplierServiceNet.Core.Interfaces;
using SupplierServiceNet.Core.IRepositorio;
using SupplierServiceNet.CrossCutting.Options;
using SupplierServiceNet.CrossCutting.Supplier;
using SupplierServiceNet.Repositorio;
using SupplierServiceNet.Services;
using System.Text;

namespace SupplierServiceNet.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CloudinaryOptions>(configuration.GetSection("Cloudinary"));
            services.Configure<ApiSettings>(configuration.GetSection(ApiSettings.SectionName));

            services.AddScoped<ICloudinaryService, CloudinaryService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<ISupplierRepository, SupplierRepository>();
            services.AddScoped<IUserContextService, UserContextService>();
            services.AddScoped<IExcelService, ExcelService>();

            services.AddScoped<IValidator<CreateSupplierDto>, CreateSupplierDtoValidator>();
            services.AddScoped<IValidator<UsuarioRegistroDto>, UsuarioRegistroDtoValidator>();

            services.AddAuthorization(options => options.DefaultPolicy =
                new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build());

            var apiSettings = configuration.GetSection(ApiSettings.SectionName).Get<ApiSettings>();
            if (apiSettings?.Secreta == null)
            {
                throw new InvalidOperationException($"Missing required configuration: {ApiSettings.SectionName}:Secreta");
            }

            var key = apiSettings.Secreta;
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
