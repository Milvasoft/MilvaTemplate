using Milvasoft.Helpers.Attributes.Validation;

namespace MilvaTemplate.API.Helpers.Attributes.ValidationAttributes;

/// <summary>
/// Specifies that the class or property that this attribute is applied to requires the specified the valid list count.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class MValidateListCountAttribute : ValidateListCountAttribute
{
    /// <summary>
    /// Constructor of atrribute.
    /// </summary>
    public MValidateListCountAttribute() : base(typeof(SharedResource)) { }

    /// <summary>
    /// Constructor of atrribute.
    /// </summary>
    /// <param name="minValue"></param>
    public MValidateListCountAttribute(int minValue) : base(minValue, typeof(SharedResource)) { }

    /// <summary>
    /// Constructor of atrribute.
    /// </summary>
    /// <param name="minValue"></param>
    /// <param name="maxValue"></param>
    public MValidateListCountAttribute(int minValue, int maxValue) : base(minValue, maxValue, typeof(SharedResource)) { }
}
