using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SupplierServiceNet.Core.Entities;
using SupplierServiceNet.Infrastructure.Data;

namespace SupplierServiceNet.Seed
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<ApplicationDbContext>();
                var userManager = services.GetRequiredService<UserManager<User>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
      
                // Aplicar migraciones pendientes
                await context.Database.MigrateAsync();

                // Crear roles si no existen
                await EnsureRolesAsync(roleManager);

                // Crear usuarios por defecto
                await EnsureDefaultUsersAsync(userManager);

                Console.WriteLine("✓ Seeding completado exitosamente");
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "✗ Error durante el seeding de la base de datos");
                throw;
            }
        }

        private static async Task EnsureRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Requester", "Approver" };

            foreach (var role in roles)
            {
                var roleExists = await roleManager.RoleExistsAsync(role);
                if (!roleExists)
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                    Console.WriteLine($"✓ Rol '{role}' creado");
                }
            }
        }
        private static async Task WaitForDatabaseAsync(ApplicationDbContext context)
        {
            int maxAttempts = 30;
            int delay = 2000; // 2 segundos

            for (int i = 0; i < maxAttempts; i++)
            {
                try
                {
                    // Intenta abrir la conexión
                    if (context.Database.CanConnect())
                    {
                        Console.WriteLine("✓ Conexión a la base de datos exitosa.");
                        return;
                    }
                }
                catch (Exception)
                {
                    // Si hay error, esperar y reintentar
                    Console.WriteLine($"✗ Intento {i + 1} de {maxAttempts}: No se pudo conectar a la base de datos. Reintentando en {delay / 1000} segundos...");
                    await Task.Delay(delay);
                }
            }

            throw new Exception("No se pudo conectar a la base de datos después de múltiples intentos.");
        }

        private static async Task EnsureDefaultUsersAsync(UserManager<User> userManager)
        {
            // Usuario Requester
            var requesterEmail = "requester@stracon.com";
            var requesterUser = await userManager.FindByEmailAsync(requesterEmail);

            if (requesterUser == null)
            {
                requesterUser = new User
                {
                    UserName = requesterEmail,
                    Email = requesterEmail,
                    NormalizedUserName = "Usuario Requester",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow,
                };

                var result = await userManager.CreateAsync(requesterUser, "Dsr1#tec");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(requesterUser, "Requester");
                    Console.WriteLine($"✓ Usuario '{requesterEmail}' creado (Rol: Requester)");
                }
                else
                {
                    Console.WriteLine($"✗ Error creando usuario '{requesterEmail}': {string.Join(", ", result.Errors)}");
                }
            }

            // Usuario Approver (Raul Armas)
            var approverEmail = "admin@stracon.com";
            var approverUser = await userManager.FindByEmailAsync(approverEmail);

            if (approverUser == null)
            {
                approverUser = new User
                {
                    UserName = approverEmail,
                    Email = approverEmail,
                    NormalizedUserName = "Raul Armas",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(approverUser, "Dsr1#tec");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(approverUser, "Approver");
                    Console.WriteLine($"✓ Usuario '{approverEmail}' creado (Rol: Approver)");
                }
                else
                {
                    Console.WriteLine($"✗ Error creando usuario '{approverEmail}': {string.Join(", ", result.Errors)}");
                }
            }

    
        }
    }
}
