using MilvaTemplate.API.Helpers.Attributes.ValidationAttributes;

namespace MilvaTemplate.API.DTOs
{
    /// <summary>
    /// Paginatination params
    /// </summary>
    public class PaginationParams
    {
        /// <summary>
        /// Requested page number.
        /// </summary>
        [MValidateDecimal(0, LocalizerKey = "WrongRequestedPageNumber", FullMessage = true)]
        public int PageIndex { get; set; } = 1;

        /// <summary>
        /// Requested item count in requested page.
        /// </summary>
        [MValidateDecimal(0, LocalizerKey = "WrongRequestedItemCount", FullMessage = true)]
        public int RequestedItemCount { get; set; } = 20;

        /// <summary>
        /// If order by column requested then Property name of entity.
        /// </summary>
        [MValidateString(0, 200)]
        public string OrderByProperty { get; set; } = "";

        /// <summary>
        /// If order by column requested then ascending or descending.
        /// </summary>
        public bool OrderByAscending { get; set; } = false;
    }
}
