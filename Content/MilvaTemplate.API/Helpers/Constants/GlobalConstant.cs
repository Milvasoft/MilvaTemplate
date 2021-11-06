using Milvasoft.Helpers.Models;
using MilvaTemplate.API.AppStartup;
using MilvaTemplate.API.Helpers.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace MilvaTemplate.API.Helpers.Constants
{
    /// <summary>
    /// Global constants.
    /// </summary>
    public static class GlobalConstant
    {
        /// <summary>
        /// Route prefix of api.
        /// </summary>
        public const string RoutePrefix = "xrouteprefixx";

        /// <summary>
        /// Base route path of api.
        /// </summary>
        public const string RouteBase = RoutePrefix + "/" + "v{version:apiVersion}";

        /// <summary>
        /// Full route path of api. It includes <see cref="RouteBase"/> and controller name. 
        /// </summary>
        public const string FullRoute = RouteBase + "/" + "[controller]";

        /// <summary>
        /// Base route with "/" at the beginning.
        /// </summary>
        public const string RoutePrefixAndVersion = "/" + RoutePrefix + "/" + "v{version:apiVersion}";

        /// <summary>
        /// Json files path.
        /// </summary>
        public static string RootPath { get; } = Environment.CurrentDirectory;

        /// <summary>
        /// Rootpath of application.  
        /// </summary>
        public static string JsonFilesPath { get; } = Path.Combine(RootPath, "StaticFiles", "JSON");

        /// <summary>
        /// Allowed file extensions for media files.
        /// </summary>
        public static List<AllowedFileExtensions> AllowedFileExtensions { get; set; }

        /// <summary>
        /// Invalid strings for prevent hacking or someting ;)
        /// </summary>
        public static List<InvalidString> StringBlacklist { get; set; }

        /// <summary>
        /// Path of "Media Library" folder in wwwroot folder.
        /// </summary>
        public static string MediaLibraryPath { get; } = Path.Combine(Startup.WebHostEnvironment.WebRootPath, "Media Library");

        /// <summary>
        /// Path of "Image Library" folder in wwwroot folder.
        /// </summary>
        public static string ImageLibraryPath { get => Path.Combine(MediaLibraryPath, "Image Library"); }

        /// <summary>
        /// Path of "ARModel Library" folder in wwwroot folder.
        /// </summary>
        public static string ARModelLibraryPath { get => Path.Combine(MediaLibraryPath, "ARModel Library"); }

        /// <summary>
        /// Path of "Video Library" folder in wwwroot folder.
        /// </summary>
        public static string VideoLibraryPath { get; } = Path.Combine(MediaLibraryPath, "Video Library");

        /// <summary>
        /// Path of "Video Library" folder in wwwroot folder.
        /// </summary>
        public static string DocumentLibraryPath { get; } = Path.Combine(MediaLibraryPath, "Document Library");

        /// <summary>
        /// Mail content of injection mails.
        /// </summary>
        public static string MailContent { get; } = $"Injection warning from MilvaTemplate.";

        /// <summary>
        /// Default language id of system.
        /// </summary>
        public static sbyte DefaultLanguageId { get; set; }

        /// <summary>
        /// Zero
        /// </summary>
        public const int Zero = 0;

        /// <summary>
        /// Name of application.
        /// </summary>
        public const string ApplicationName = "MilvaTemplate";

        /// <summary>
        /// Mail of application.
        /// </summary>
        public const string AppMail = "app@yourdomain.com";

        /// <summary>
        /// Key of project.
        /// </summary>
        public const string MilvaTemplateKey = "w!z%C*F-JaNdRgUk";

        public static bool RealProduction { get; set; } = false;
    }
}
