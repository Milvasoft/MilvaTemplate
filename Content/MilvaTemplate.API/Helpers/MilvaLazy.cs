using Microsoft.Extensions.DependencyInjection;
using System;

namespace MilvaTemplate.API.Helpers;

/// <summary>
/// Custom <see cref="Lazy{T}"/> class.
/// </summary>
/// <typeparam name="T"></typeparam>
public class MilvaLazy<T> : Lazy<T>
{
    /// <summary>
    /// Constructor of <see cref="MilvaLazy{T}"/>.
    /// </summary>
    /// <param name="serviceProvider"></param>
    public MilvaLazy(IServiceProvider serviceProvider) : base(() => serviceProvider.GetRequiredService<T>())
    {
    }
}
