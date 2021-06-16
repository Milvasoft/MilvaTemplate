using MilvaTemplate.API.Helpers.Attributes.ValidationAttributes;
using System.Collections.Generic;

namespace MilvaTemplate.API.DTOs
{
    /// <summary>
    /// Content map object.
    /// </summary>
    public class Contents
    {
        /// <summary>
        /// Content key. (e.g. Product)
        /// </summary>
        [MValidateString(0, 500)]
        public string Key { get; set; }

        /// <summary>
        /// Contents.
        /// </summary>
        public List<Content> ContentList { get; set; }
    }
}
