using System.Collections.Generic;

namespace MilvaTemplate.API.Helpers.Models;

/// <summary>
/// Allowed file extensions for media files.
/// </summary>
public class AllowedFileExtensions
{
    /// <summary>
    /// File type of media file.
    /// </summary>
    public string FileType { get; set; }

    /// <summary>
    /// Allowed extensions for this media type.
    /// </summary>
    public List<string> AllowedExtensions { get; set; }
}
