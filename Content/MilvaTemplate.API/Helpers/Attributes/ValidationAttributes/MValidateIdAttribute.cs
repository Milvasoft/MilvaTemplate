using Milvasoft.Helpers.Attributes.Validation;
using MilvaTemplate.Localization;
using System;

namespace MilvaTemplate.API.Helpers.Attributes.ValidationAttributes;

/// <summary>
/// Specifies that the class or property that this attribute is applied to requires the specified the valid id.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class MValidateIdAttribute : ValidateIdPropertyAttribute
{
    /// <summary>
    /// Constructor that accepts the maximum length of the string.
    /// </summary>
    public MValidateIdAttribute() : base(typeof(SharedResource)) { }
}
