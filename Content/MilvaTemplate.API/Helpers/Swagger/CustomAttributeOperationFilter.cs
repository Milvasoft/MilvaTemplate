using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using MilvaTemplate.API.Helpers.Attributes.ActionFilters;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;
using System.Reflection;

namespace MilvaTemplate.API.Helpers.Swagger
{
    /// <summary>
	/// Swagger document creation utility class.
	/// </summary>
    public class CustomAttributeOperationFilter : IOperationFilter
    {
        /// <summary>
        /// Applies filter on swagger document.
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (context.MethodInfo?.IsDefined(typeof(MValidateStringParameterAttribute)) ?? false)
            {
                var defaultValue = Attribute.GetCustomAttribute(context.MethodInfo, typeof(MValidateStringParameterAttribute)) as MValidateStringParameterAttribute;

                foreach (var parameter in operation.Parameters)
                {
                    if (parameter.Schema?.Type == "string")
                        parameter.Description += parameter.Description != null
                                                                        ? $"<br /> Minimum Length : {defaultValue.MinimumLength}" + $"<br /> Maximum Length : {defaultValue.MaximumLength}"
                                                                        : $"Minimum Length : {defaultValue.MinimumLength}" + $"<br /> Maximum Length : {defaultValue.MaximumLength}";
                }
            }

            if (context.MethodInfo?.IsDefined(typeof(AuthorizeAttribute)) ?? false)
            {
                var authorizeAttributes = Attribute.GetCustomAttributes(context.MethodInfo, typeof(AuthorizeAttribute));

                var roles = authorizeAttributes.Select(a => a as AuthorizeAttribute).Select(a => $"({a.Roles.Replace(",", " | ")})");

                var joinedString = string.Join(" & ", roles);

                operation.Description += $"<br /> <br /> <b> Allowed Roles : {joinedString}<b/>";
            }
            else operation.Description += $"<br /> <br /> <b> Allowed Roles : (Everyone) <b/>";

        }
    }
}
