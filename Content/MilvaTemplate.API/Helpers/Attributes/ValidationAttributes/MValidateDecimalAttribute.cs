using Milvasoft.Helpers.Attributes.Validation;
using MilvaTemplate.Localization;
using System;

namespace MilvaTemplate.API.Helpers.Attributes.ValidationAttributes
{
    /// <summary>
    /// Determines minimum decimal value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class MValidateDecimalAttribute : ValidateDecimalAttribute
    {
        /// <summary>
        /// Constructor of atrribute.
        /// </summary>
        public MValidateDecimalAttribute() : base(typeof(SharedResource)) { }

        /// <summary>
        /// Constructor of atrribute.
        /// </summary>
        /// <param name="minValue"></param>
        public MValidateDecimalAttribute(int minValue) : base(minValue, typeof(SharedResource)) { }
    }
}
