using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace ApiMovies.Extensions
{
    public static class SwaggerServicesExtensions
    {
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            string assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string? fileVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(assemblyLocation).FileVersion;
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "MOVIES" + " API",
                    Version = "v" + fileVersion,
                    Description = "Microservice Architecture",
                    Contact = new OpenApiContact
                    {
                        Name = "Seguridad @ Cofide"
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Derechos Reservados"
                    },
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                        {
                            Description =
                           "Autenticación JWT usando el esquema Bearer. \r\n\r\n " +
                           "Ingresa la palabra 'Bearer' seguida de un [espacio] y despues su token en el campo de abajo \r\n\r\n" +
                           "Ejemplo: \"Bearer tkdknkdllskd\"",
                            Name = "Authorization",
                            In = ParameterLocation.Header,
                            Scheme = "Bearer"
                        });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                   {
                   {
                       new OpenApiSecurityScheme
                       {
                           Reference = new OpenApiReference
                                       {
                                           Type = ReferenceType.SecurityScheme,
                                           Id = "Bearer"
                                       },
                           Scheme = "Bearer",
                           Name = "Bearer",
                           In = ParameterLocation.Header
                       },
                       new List<string>()
                   }
               });
                c.DocumentFilter<HideInProdFilter>();
            });
            return services;
        }

        public static IApplicationBuilder UseSwaggerGen(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1")); 
            return app;
        }
    }
}
