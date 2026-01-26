using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using SupplierServiceNet.Application.Dtos;
using SupplierServiceNet.Core.Entities;
 
using SupplierServiceNet.Extensions;
using SupplierServiceNet.Infrastructure;
using SupplierServiceNet.Infrastructure.Data;
using SupplierServiceNet.Middlewares;
using Serilog;

public class Program
{
    public class PeliculasMapper : Profile
    {
        public PeliculasMapper()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User, DataUserDto>().ReverseMap();
        }
    }
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        //Soporte para autenticación con .NET Identity
        builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
        builder.Services.AddApplicationServices(builder.Configuration);
        builder.Services.AddPersistence(builder.Configuration);
        builder.Services.AddCustomResponseCompression();
        builder.Services.AddCustomHealthChecks();
        builder.Host.UseSerilog((context, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration));

        builder.Services.AddAutoMapper(typeof(PeliculasMapper));
        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services
        .AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;

            // URL segment: /api/v1/...
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        })
        .AddMvc()
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";           // v1
            options.SubstituteApiVersionInUrl = true;     // reemplaza {version:apiVersion}
        });
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerDocumentation();
        builder.Services.ConfigureCors();
        builder.Services.AddHttpContextAccessor();

        var app = builder.Build();
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwaggerGen();
        }
        //app.UseHttpsRedirection();
        app.UseSerilogRequestLogging();
        app.UseMiddleware<ExceptionMiddleware>();
        app.UseCustomCors();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.MapHealthChecks("/health");
        app.Run();
    } 

}