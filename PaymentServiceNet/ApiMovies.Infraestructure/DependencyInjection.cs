using ApiMovies.Infrastructure.Repositorio;
using ApiMovies.Infrastructure.Repositorio.WorkContainer;
using ApiMovies.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ApiMovies.Core.IRepositorio;
namespace ApiMovies.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services,
         IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("ConexionSQL"),
                  b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)), 
                  ServiceLifetime.Scoped);

            // Configuración para Oracle
            //services.AddDbContext<OracleDBContext>(options =>
            //    options.UseOracle(
            //        configuration.GetConnectionString("ConexionOracle"),
            //        b => b.MigrationsAssembly(typeof(OracleDBContext).Assembly.FullName)),
            //    ServiceLifetime.Scoped);

            // Configuración para PostgreSQL
            //services.AddDbContext<PostgreSqlContext>(options =>
            //    options.UseNpgsql(
            //        configuration.GetConnectionString("ConexionPostgreSQL"),
            //        b => b.MigrationsAssembly(typeof(PostgreSqlContext).Assembly.FullName)),
            //    ServiceLifetime.Scoped);
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}