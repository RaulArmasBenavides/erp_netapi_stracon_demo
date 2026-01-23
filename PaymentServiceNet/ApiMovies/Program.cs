using ApiMovies.Application.Dtos;
using ApiMovies.Core.Entities;
 
using ApiMovies.Extensions;
using ApiMovies.Infrastructure;
using ApiMovies.Infrastructure.Data;
using ApiMovies.Middlewares;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Serilog;

public class Program
{
    public class PeliculasMapper : Profile
    {
        public PeliculasMapper()
        {
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Category, CreateCategoryDto>().ReverseMap();
            CreateMap<Movie, MovieDto>().ReverseMap();
            CreateMap<AppUsuario, UserDto>().ReverseMap();
            CreateMap<AppUsuario, DataUserDto>().ReverseMap();
        }
    }
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        //Soporte para autenticación con .NET Identity
        builder.Services.AddIdentity<AppUsuario, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
        builder.Services.AddApplicationServices(builder.Configuration);
        builder.Services.AddPersistence(builder.Configuration);
        builder.Services.AddCustomResponseCompression();
        builder.Services.AddCustomHealthChecks();
        builder.Host.UseSerilog((context, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration));

        builder.Services.AddAutoMapper(typeof(PeliculasMapper));
        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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