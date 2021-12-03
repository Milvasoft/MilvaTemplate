using Microsoft.AspNetCore.Http;
using Milvasoft.Helpers.FileOperations.Concrete;
using Milvasoft.Helpers.FileOperations.Enums;
using System.IO;

namespace MilvaTemplate.API.Helpers.Extensions;

/// <summary>
/// Helper extensions methods for Ops!yon Project.
/// </summary>
public static partial class HelperExtensions
{
    #region IFormFile Helpers

    /// <summary>
    /// Validates file. 
    /// </summary>
    /// <param name="file"></param>
    /// <param name="fileType"></param>
    public static void ValidateFile(this IFormFile file, FileType fileType)
    {
        int maxFileLength = 14000000;

        var allowedFileExtensions = GlobalConstant.AllowedFileExtensions.Find(i => i.FileType == fileType.ToString()).AllowedExtensions;

        var validationResult = file.ValidateFile(maxFileLength, allowedFileExtensions, fileType);

        switch (validationResult)
        {
            case FileValidationResult.Valid:
                break;
            case FileValidationResult.FileSizeTooBig:
                // Get length of file in bytes
                long fileSizeInBytes = file.Length;
                // Convert the bytes to Kilobytes (1 KB = 1024 Bytes)
                double fileSizeInKB = fileSizeInBytes / 1024;
                // Convert the KB to MegaBytes (1 MB = 1024 KBytes)
                double fileSizeInMB = fileSizeInKB / 1024;
                throw new MilvaUserFriendlyException(nameof(ResourceKey.FileIsTooBigMessage), fileSizeInMB.ToString("0.#"));
            case FileValidationResult.InvalidFileExtension:
                throw new MilvaUserFriendlyException(nameof(ResourceKey.UnsupportedFileTypeMessage), string.Join(", ", allowedFileExtensions));
            case FileValidationResult.NullFile:
                throw new MilvaUserFriendlyException(nameof(ResourceKey.FileCannotBeEmpty));
        }
    }

    /// <summary>
    /// Save uploaded IFormFile file to server. Target Path will be : ".../wwwroot/Media Library/Image Library/<paramref name="entity"></paramref>.Id".
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="file"> Uploaded file in entity. </param>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static async Task<string> SaveVideoToServerAsync<TEntity, TKey>(this IFormFile file, TEntity entity)
    {
        string basePath = GlobalConstant.VideoLibraryPath;

        FormFileOperations.FilesFolderNameCreator imagesFolderNameCreator = CreateVideoFolderNameFromDTO;

        string propertyName = "Id";

        int maxFileLength = 140000000;

        var allowedFileExtensions = GlobalConstant.AllowedFileExtensions.Find(i => i.FileType == FileType.Video.ToString()).AllowedExtensions;

        var validationResult = file.ValidateFile(maxFileLength, allowedFileExtensions, FileType.Video);

        switch (validationResult)
        {
            case FileValidationResult.Valid:
                break;
            case FileValidationResult.FileSizeTooBig:
                // Get length of file in bytes
                long fileSizeInBytes = file.Length;
                // Convert the bytes to Kilobytes (1 KB = 1024 Bytes)
                double fileSizeInKB = fileSizeInBytes / 1024;
                // Convert the KB to MegaBytes (1 MB = 1024 KBytes)
                double fileSizeInMB = fileSizeInKB / 1024;
                throw new MilvaUserFriendlyException(nameof(ResourceKey.FileIsTooBigMessage), fileSizeInMB.ToString("0.#"));
            case FileValidationResult.InvalidFileExtension:
                throw new MilvaUserFriendlyException(nameof(ResourceKey.UnsupportedFileTypeMessage), string.Join(", ", allowedFileExtensions));
            case FileValidationResult.NullFile:
                return string.Empty;
        }

        var path = await file.SaveFileToPathAsync(entity, basePath, imagesFolderNameCreator, propertyName);

        await file.OpenReadStream().DisposeAsync();

        return path;
    }

    /// <summary>
    /// Returns the path of the uploaded file.
    /// </summary>
    /// <param name="originalImagePath"> Uploaded file. </param>
    /// <param name="fileType"> Uploaded file type. (e.g image,video,sound) </param>
    public static string GetFileUrlFromPath(string originalImagePath, FileType fileType)
    {
        string libraryType = string.Empty;
        switch (fileType)
        {
            case FileType.Image:
                libraryType = $"{GlobalConstant.RoutePrefix}/ImageLibrary";
                break;
            case FileType.Video:
                libraryType = $"{GlobalConstant.RoutePrefix}/VideoLibrary";
                break;
            case FileType.ARModel:
                libraryType = $"{GlobalConstant.RoutePrefix}/ARModelLibrary";
                break;
            case FileType.Audio:
                libraryType = $"{GlobalConstant.RoutePrefix}/AudioLibrary";
                break;
            case FileType.Document:
                libraryType = $"{GlobalConstant.RoutePrefix}/DocumentLibrary";
                break;
            default:
                break;
        }
        return FormFileOperations.GetFileUrlPathSectionFromFilePath(originalImagePath, libraryType);
    }

    /// <summary>
    /// Converts data URI formatted base64 string to IFormFile.
    /// </summary>
    /// <param name="milvaBase64"></param>
    /// <returns></returns>
    public static IFormFile ConvertToFormFile(string milvaBase64)
    {
        var splittedBase64String = milvaBase64.Split(";base64,");
        var base64String = splittedBase64String?[1];

        var contentType = splittedBase64String[0].Split(':')[1];

        var splittedContentType = contentType.Split('/');

        var fileType = splittedContentType?[0];

        var fileExtension = splittedContentType?[1];

        var array = Convert.FromBase64String(base64String);

        var memoryStream = new MemoryStream(array)
        {
            Position = 0
        };

        return new FormFile(memoryStream, 0, memoryStream.Length, fileType, $"File.{fileExtension}")
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
    }

    private static string CreateImageFolderNameFromDTO(Type type)
    {
        return type.Name.Split("DTO")[0] + "Images";
    }

    private static string CreateVideoFolderNameFromDTO(Type type)
    {
        return type.Name + "Videos";
    }

    #endregion
}
