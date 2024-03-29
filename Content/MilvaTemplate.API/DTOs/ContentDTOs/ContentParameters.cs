﻿using MilvaTemplate.API.Helpers.Attributes.ValidationAttributes;

namespace MilvaTemplate.API.DTOs.ContentDTOs;

/// <summary>
/// For request content. 
/// </summary>
public class ContentParameters
{
    /// <summary>
    /// Requested entity name.
    /// </summary>
    [MValidateString(2, 40)]
    public string EntityName { get; set; }

    /// <summary>
    /// Requested properties.
    /// </summary>
    [MValidateString(0, 40)]
    public List<string> RequestedProps { get; set; }

    /// <summary>
    /// Requested language properties.
    /// </summary>
    [MValidateString(0, 40)]
    public List<string> RequestedLangProps { get; set; }
}
