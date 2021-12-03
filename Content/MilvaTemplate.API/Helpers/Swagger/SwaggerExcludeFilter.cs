using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace MilvaTemplate.API.Helpers.Swagger;

/// <summary>
/// It allows excluding the desired property from the swagger documentation.
/// </summary>
public class SwaggerExcludeFilter : ISchemaFilter
{
    /// <summary>
    /// Triggered event.
    /// </summary>
    /// <param name="schema"></param>
    /// <param name="context"></param>
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema?.Properties == null)
        {
            return;
        }

        var excludedProperties =
            context.Type.GetProperties().Where(
                t => t.GetCustomAttribute<SwaggerExcludeAttribute>() != null);

        foreach (var excludedProperty in excludedProperties)
        {
            var propertyToRemove =
                schema.Properties.Keys.SingleOrDefault(
                    x => x.ToLower() == excludedProperty.Name.ToLower());

            if (propertyToRemove != null)
            {
                schema.Properties.Remove(propertyToRemove);
            }
        }
    }
}
