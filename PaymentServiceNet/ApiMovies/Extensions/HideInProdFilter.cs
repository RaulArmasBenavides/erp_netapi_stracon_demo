using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ApiMovies.Extensions
{
    public class HideInProdFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            //var isProduction = /* tu lógica para determinar el entorno */;
            var routesToRemove = new List<string> { "usuarios", "usuarios/{usuarioId}" };
            foreach (var route in routesToRemove)
            {
              var key = $"/api/{route}"; //optionally you can use toLower
              swaggerDoc.Paths.Remove(key);
            }
             
        }
    }
}
