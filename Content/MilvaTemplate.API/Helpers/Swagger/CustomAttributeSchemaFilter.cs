using Microsoft.OpenApi.Models;
using Milvasoft.Helpers.Attributes.Validation;
using Milvasoft.Helpers.Utils;
using MilvaTemplate.API.AppStartup;
using MilvaTemplate.API.Attributes.ValidationAttributes;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Reflection;

namespace MilvaTemplate.API.Helpers.Swagger
{
    /// <summary>
	/// Swagger document creation utility class.
	/// </summary>
    public class CustomAttributeSchemaFilter : ISchemaFilter
    {
        /// <summary>
        /// Applies filter on swagger document.
        /// </summary>
        /// <param name="swaggerSchema"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiSchema swaggerSchema, SchemaFilterContext context)
        {
            if (context.MemberInfo?.IsDefined(typeof(MValidateStringAttribute)) ?? false)
            {
                var defaultValue = Attribute.GetCustomAttribute(context.MemberInfo, typeof(MValidateStringAttribute)) as MValidateStringAttribute;

                swaggerSchema.MaxLength = defaultValue.MaximumLength;
                swaggerSchema.MinLength = defaultValue.MinimumLength;
            }

            if (context.MemberInfo?.IsDefined(typeof(MValidateDecimalAttribute)) ?? false)
            {
                var defaultValue = Attribute.GetCustomAttribute(context.MemberInfo, typeof(MValidateDecimalAttribute)) as MValidateDecimalAttribute;

                swaggerSchema.Minimum = defaultValue.MinValue == -1 ? 0 : defaultValue.MinValue;
            }

            if (context.MemberInfo?.IsDefined(typeof(MilvaRegexAttribute)) ?? false)
            {
                swaggerSchema.Pattern = Startup.SharedStringLocalizer[LocalizerKeys.RegexExample + context.MemberInfo.Name];
            }


        }
    }
}
