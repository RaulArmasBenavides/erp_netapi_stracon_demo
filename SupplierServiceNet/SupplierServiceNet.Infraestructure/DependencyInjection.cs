using SupplierServiceNet.Infrastructure.Repositorio;
using SupplierServiceNet.Infrastructure.Repositorio.WorkContainer;
using SupplierServiceNet.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SupplierServiceNet.Core.IRepositorio;
namespace SupplierServiceNet.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services,
         IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("ConexionSql"),
                  b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)), 
                  ServiceLifetime.Scoped);

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}