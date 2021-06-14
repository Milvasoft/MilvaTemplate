using Microsoft.AspNetCore.Mvc.Filters;
using Milvasoft.Helpers.Attributes.ActionFilter;
using MilvaTemplate.Localization;
using System;

namespace MilvaTemplate.API.Attributes.ActionFilters
{
    /// <summary>
    /// Specifies that the class or method that this attribute is applied to requires the specified the valid id.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class MValidateIdParameterAttribute : ValidateIdParameterAttribute
    {
        /// <summary>
        /// Constructor of <see cref="MValidateIdParameterAttribute"/> for localization.
        /// </summary>
        public MValidateIdParameterAttribute() : base(typeof(SharedResource)) { }

        /// <summary>
        /// Performs when action executing.
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
        }
    }
}
