using MilvaTemplate.API.DTOs.ContentDTOs;

namespace MilvaTemplate.API.Services.Abstract;

/// <summary>
/// An intermediate layer for data that the interface needs independent from anywhere..
/// </summary>
public interface IContentService
{
    /// <summary>
    /// Gets required content by <paramref name="contentParameterList"/>.EntityName.
    /// </summary>
    /// <param name="contentParameterList"></param>
    /// <returns></returns>
    Task<List<Contents>> GetRequiredContentAsync(List<ContentParameters> contentParameterList);

    #region SpecMaxValues

    /// <summary>
    /// Gets the requested property's(<paramref name="propName"/>) max value in requested entity by <paramref name="entityName"/>.
    /// </summary>
    /// <param name="entityName"></param>
    /// <param name="propName"></param>
    /// <returns></returns>
    Task<decimal> GetSpecMaxValueAsync(string entityName, string propName);

    #endregion
}
