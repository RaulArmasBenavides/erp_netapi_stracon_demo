namespace ApiMovies.Extensions
{
    public static class CorsConfigurationExtension
    {
        private static readonly string[] AllowedOrigins = new[]
        {
                "http://localhost:4200",
                "http://localhost:4300",
                "http://localhost:4400",
                "http://localhost:5050",
                "https://www.lps.com",
                "https://www.qa.lps.com",
                "http://10.100.100.1860",
        };

        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(
                    "CorsLpsPolicy",
                    builder =>
                    {
                        builder
                        .WithOrigins(AllowedOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                    });
            });
        }

        public static void UseCustomCors(this IApplicationBuilder app)
        {
            app.UseCors(x =>
                x.WithOrigins(AllowedOrigins)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
        }
    }
}
