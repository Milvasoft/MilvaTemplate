using Milvasoft.Helpers.Encryption.Concrete;
using Milvasoft.Helpers.FileOperations.Abstract;
using MilvaTemplate.API.Helpers.Models;
using System.IO;

namespace MilvaTemplate.API.AppStartup;

/// <summary>
/// Class in which initial configurations are specified.
/// </summary>
[ConfigureAwait(false)]
public static class StartupConfiguration
{

    /// <summary>
    /// Checks media library folders. If folders not exists creates that folders.
    /// </summary>
    public static void CheckPublicFiles()
    {
        if (!Directory.Exists(GlobalConstant.MediaLibraryPath))
        {
            Directory.CreateDirectory(GlobalConstant.MediaLibraryPath);
            Directory.CreateDirectory(GlobalConstant.ImageLibraryPath);
            Directory.CreateDirectory(GlobalConstant.ARModelLibraryPath);
            Directory.CreateDirectory(GlobalConstant.VideoLibraryPath);
            Directory.CreateDirectory(GlobalConstant.DocumentLibraryPath);
        }
        if (!Directory.Exists(GlobalConstant.ImageLibraryPath))
        {
            Directory.CreateDirectory(GlobalConstant.ImageLibraryPath);
        }
        if (!Directory.Exists(GlobalConstant.ARModelLibraryPath))
        {
            Directory.CreateDirectory(GlobalConstant.ARModelLibraryPath);
        }
        if (!Directory.Exists(GlobalConstant.VideoLibraryPath))
        {
            Directory.CreateDirectory(GlobalConstant.VideoLibraryPath);
        }
        if (!Directory.Exists(GlobalConstant.DocumentLibraryPath))
        {
            Directory.CreateDirectory(GlobalConstant.DocumentLibraryPath);
        }
    }

    /// <summary>
    /// For development.
    /// </summary>
    /// <returns></returns>
    public static async Task EncryptFile()
    {
        var provider = new MilvaEncryptionProvider(GlobalConstant.MilvaTemplateKey);

        await provider.EncryptFileAsync(Path.Combine(GlobalConstant.JsonFilesPath, "stringblacklist.json"));
        await provider.EncryptFileAsync(Path.Combine(GlobalConstant.JsonFilesPath, "allowedfileextensions.json"));
        await provider.EncryptFileAsync(Path.Combine(GlobalConstant.JsonFilesPath, "connectionstring.Development.json"));
        await provider.EncryptFileAsync(Path.Combine(GlobalConstant.JsonFilesPath, "connectionstring.Production.json"));
        await provider.EncryptFileAsync(Path.Combine(GlobalConstant.JsonFilesPath, "configurations.json"));
    }

    /// <summary>
    /// For development.
    /// </summary>
    /// <returns></returns>
    public static async Task DecryptFile()
    {
        var provider = new MilvaEncryptionProvider(GlobalConstant.MilvaTemplateKey);

        await provider.DecryptFileAsync(Path.Combine(GlobalConstant.JsonFilesPath, "stringblacklist.json"));
        await provider.DecryptFileAsync(Path.Combine(GlobalConstant.JsonFilesPath, "allowedfileextensions.json"));
        await provider.DecryptFileAsync(Path.Combine(GlobalConstant.JsonFilesPath, "connectionstring.Development.json"));
        await provider.DecryptFileAsync(Path.Combine(GlobalConstant.JsonFilesPath, "connectionstring.Production.json"));
        await provider.DecryptFileAsync(Path.Combine(GlobalConstant.JsonFilesPath, "configurations.json"));
    }

    /// <summary>
    /// Fill constants from json files.
    /// </summary>
    /// <param name="jsonOperations"></param>
    /// <returns></returns>
    public static async Task FillConstansAsync(this IJsonOperations jsonOperations)
    {
        await jsonOperations.FillAllowedFileExtensionsAsync();
        await jsonOperations.FillStringBlacklistAsync();
    }

    /// <summary>
    /// Fills <see cref="GlobalConstant.StringBlacklist"/> list from stringblacklist.json file.
    /// </summary>
    /// <param name="jsonOperations"></param>
    /// <returns></returns>
    public static async Task FillStringBlacklistAsync(this IJsonOperations jsonOperations)
    {
        GlobalConstant.StringBlacklist = await jsonOperations.GetCryptedContentAsync<List<InvalidString>>("stringblacklist.json");
    }

    /// <summary>
    /// Fills <see cref="GlobalConstant.AllowedFileExtensions"/> list from allowedfileextensions.json file.
    /// </summary>
    /// <param name="jsonOperations"></param>
    /// <returns></returns>
    public static async Task FillAllowedFileExtensionsAsync(this IJsonOperations jsonOperations)
    {
        GlobalConstant.AllowedFileExtensions = await jsonOperations.GetCryptedContentAsync<List<AllowedFileExtensions>>("allowedfileextensions.json");
    }
}
