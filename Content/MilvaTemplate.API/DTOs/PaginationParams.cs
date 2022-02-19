using MilvaTemplate.API.Helpers.Attributes.ValidationAttributes;

namespace MilvaTemplate.API.DTOs;

/// <summary>
/// Paginatination params
/// </summary>
public class PaginationParams
{
    /// <summary>
    /// Requested page number.
    /// </summary>
    [MValidateDecimal(0, LocalizerKey = StringKey.WrongRequestedPageNumber, FullMessage = true)]
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// Requested item count in requested page.
    /// </summary>
    [MValidateDecimal(0, LocalizerKey = StringKey.WrongRequestedItemCount, FullMessage = true)]
    public int RequestedItemCount { get; set; } = 10;

    /// <summary>
    /// If order by column requested then Property name of entity.
    /// </summary>
    [MValidateString(0, 200)]
    public string OrderByProperty { get; set; } = string.Empty;

    /// <summary>
    /// If order by column requested then ascending or descending.
    /// </summary>
    public bool OrderByAscending { get; set; } = false;
}
