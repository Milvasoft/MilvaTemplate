using Fody;
using Microsoft.AspNetCore.Http;
using Milvasoft.Helpers.DataAccess.Abstract;
using Milvasoft.Helpers.DataAccess.Abstract.Entity;
using Milvasoft.Helpers.DataAccess.Concrete.Entity;
using Milvasoft.Helpers.DataAccess.IncludeLibrary;
using Milvasoft.Helpers.Exceptions;
using Milvasoft.Helpers.Extensions;
using Milvasoft.Helpers.FileOperations.Concrete;
using Milvasoft.Helpers.FileOperations.Enums;
using Milvasoft.Helpers.Utils;
using MilvaTemplate.API.DTOs.AccountDTOs;
using MilvaTemplate.Data;
using MilvaTemplate.Data.Abstract;
using MilvaTemplate.Entity.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace MilvaTemplate.API.Helpers.Extensions
{
    /// <summary>
    /// Helper extensions methods for Ops!yon Project.
    /// </summary>
    [ConfigureAwait(false)]
    public static class HelperExtensions
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

            var allowedFileExtensions = GlobalConstants.AllowedFileExtensions.Find(i => i.FileType == fileType.ToString()).AllowedExtensions;

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
                    throw new MilvaUserFriendlyException("FileIsTooBigMessage", fileSizeInMB.ToString("0.#"));
                case FileValidationResult.InvalidFileExtension:
                    throw new MilvaUserFriendlyException("UnsupportedFileTypeMessage", string.Join(", ", allowedFileExtensions));
                case FileValidationResult.NullFile:
                    throw new MilvaUserFriendlyException("FileCannotBeEmpty"); ;
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
            string basePath = GlobalConstants.VideoLibraryPath;

            FormFileOperations.FilesFolderNameCreator imagesFolderNameCreator = CreateVideoFolderNameFromDTO;

            string propertyName = "Id";

            int maxFileLength = 140000000;

            var allowedFileExtensions = GlobalConstants.AllowedFileExtensions.Find(i => i.FileType == FileType.Video.ToString()).AllowedExtensions;

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
                    throw new MilvaUserFriendlyException("FileIsTooBigMessage", fileSizeInMB.ToString("0.#"));
                case FileValidationResult.InvalidFileExtension:
                    throw new MilvaUserFriendlyException("UnsupportedFileTypeMessage", string.Join(", ", allowedFileExtensions));
                case FileValidationResult.NullFile:
                    return "";
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
                    libraryType = $"{GlobalConstants.RoutePrefix}/ImageLibrary";
                    break;
                case FileType.Video:
                    libraryType = $"{GlobalConstants.RoutePrefix}/VideoLibrary";
                    break;
                case FileType.ARModel:
                    libraryType = $"{GlobalConstants.RoutePrefix}/ARModelLibrary";
                    break;
                case FileType.Audio:
                    libraryType = $"{GlobalConstants.RoutePrefix}/AudioLibrary";
                    break;
                case FileType.Document:
                    libraryType = $"{GlobalConstants.RoutePrefix}/DocumentLibrary";
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

        #region Language Helpers

        /// <summary>
        /// Stores language id and iso code.
        /// </summary>
        public static Dictionary<string, int> LanguageIdIsoPairs { get; set; } = new();

        private const string SystemLanguageIdString = "SystemLanguageId";

        /// <summary>
        /// Performs the necessary mapping for language dependent objects. For example, it is used to map the data in the Product class to the ProductDTO class.
        /// </summary>
        /// <param name="langs"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static string GetLang<TEntity>(this IEnumerable<TEntity> langs, Expression<Func<TEntity, string>> propertyName)
        {
            var requestedLangId = GetLanguageId(GlobalConstants.DefaultLanguageId);

            if (langs.IsNullOrEmpty()) return "";

            var propName = propertyName.GetPropertyName();

            TEntity requestedLang;

            if (requestedLangId != GlobalConstants.DefaultLanguageId) requestedLang = langs.FirstOrDefault(lang => (int)lang.GetType().GetProperty(SystemLanguageIdString).GetValue(lang) == requestedLangId)
                                                                                        ?? langs.FirstOrDefault(lang => (int)lang.GetType().GetProperty(SystemLanguageIdString).GetValue(lang) == GlobalConstants.DefaultLanguageId);

            else requestedLang = langs.FirstOrDefault(lang => (int)lang.GetType().GetProperty(SystemLanguageIdString).GetValue(lang) == GlobalConstants.DefaultLanguageId);

            requestedLang ??= langs.FirstOrDefault();

            return requestedLang.GetType().GetProperty(propName).GetValue(requestedLang, null)?.ToString();
        }

        /// <summary>
        /// Performs the necessary mapping for language dependent objects. For example, it is used to map the data in the Product class to the ProductDTO class.
        /// </summary>
        /// <param name="langs"></param>
        /// <returns></returns>
        public static IEnumerable<TDTO> GetLangs<TEntity, TDTO>(this IEnumerable<TEntity> langs) where TDTO : new()
        {
            if (langs.IsNullOrEmpty()) yield break;

            foreach (var lang in langs)
            {
                TDTO dto = new();
                foreach (var entityProp in lang.GetType().GetProperties())
                {
                    var dtoProp = dto.GetType().GetProperty(entityProp.Name);

                    var entityPropValue = entityProp.GetValue(lang, null);

                    if (entityProp.Name == SystemLanguageIdString) dtoProp.SetValue(dto, entityPropValue, null);

                    else if (entityProp.PropertyType == typeof(string)) dtoProp.SetValue(dto, entityPropValue, null);
                }
                yield return dto;
            }
        }

        /// <summary>
        /// Gets language id from CultureInfo.CurrentCulture.
        /// </summary>
        public static int GetLanguageId(int defaultLangId)
        {
            var culture = CultureInfo.CurrentCulture;
            if (LanguageIdIsoPairs.ContainsKey(culture.Name))
                return LanguageIdIsoPairs[culture.Name];
            else
                return defaultLangId;
        }

        #endregion

        #region HttpContextAccessor Helpers

        /// <summary>
        /// Gets institution id from request's header. Then returns that id variable.
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        public static (int pageIndex, int itemCount) GetPaginationVariablesFromHeader(this IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor.HttpContext.Request.Headers.Keys.Contains("pageindex") && httpContextAccessor.HttpContext.Request.Headers.Keys.Contains("itemcount"))
            {
                int pageIndex;
                httpContextAccessor.HttpContext.Request.Headers.TryGetValue("pageindex", out var pageIndexValue);
                if (!pageIndexValue.IsNullOrEmpty())
                {
                    pageIndex = Convert.ToInt32(pageIndexValue[0]);
                    if (pageIndex <= GlobalConstants.Zero) throw new MilvaUserFriendlyException("InvalidPageIndexException");
                }
                else
                    throw new MilvaUserFriendlyException("MissingHeaderException", "PageIndex");

                int itemCount;
                httpContextAccessor.HttpContext.Request.Headers.TryGetValue("itemcount", out var itemCountValue);
                if (!itemCountValue.IsNullOrEmpty())
                {
                    itemCount = Convert.ToInt32(itemCountValue[0]);
                    if (itemCount <= GlobalConstants.Zero) throw new MilvaUserFriendlyException("InvalidItemRangeException");
                }
                else
                    throw new MilvaUserFriendlyException("MissingHeaderException", "ItemCount");

                return (pageIndex, itemCount);
            }
            else throw new MilvaUserFriendlyException("MissingHeaderException", "PageIndex,ItemCount");
        }

        #endregion

        #region Default Record Check Helpers

        /// <summary>
        /// Checks <paramref name="id"/> is default record id or not.
        /// </summary>
        /// 
        /// <exception cref="MilvaUserFriendlyException"> Throwns when <paramref name="id"/> is defult record id. </exception>
        /// 
        /// <param name="id"></param>
        public static void CheckContentIsDefaultRecord(this int id)
        {
            if (id is > GlobalConstants.Zero and < 50) throw new MilvaUserFriendlyException(MilvaException.CannotUpdateOrDeleteDefaultRecord);
        }

        /// <summary>
        /// Checks <paramref name="idList"/> contains default record or not.
        /// </summary>
        /// 
        /// <exception cref="MilvaUserFriendlyException"> Throwns when contents contains defult record id. </exception>
        /// 
        /// <param name="idList"></param>
        public static void CheckContentIsDefaultRecord(this List<int> idList)
        {
            if (idList.Any(i => i is > GlobalConstants.Zero and < 50)) throw new MilvaUserFriendlyException(MilvaException.CannotUpdateOrDeleteDefaultRecord);
        }

        /// <summary>
        /// Checks <paramref name="id"/> is default record id or not.
        /// </summary>
        /// <param name="id"></param>
        public static bool IsDefaultRecord(this int id)
        {
            return id is > GlobalConstants.Zero and < 50;
        }

        #endregion

        #region Reflection Helpers

        /// <summary>
        /// Get langs property in runtime.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="langPropName"></param>
        /// <param name="requestedPropName"></param>
        /// <returns></returns>
        public static dynamic GetLangPropValue(this object obj, string langPropName, string requestedPropName)
        {
            var langValues = obj.GetType().GetProperty(langPropName)?.GetValue(obj, null) ?? throw new MilvaUserFriendlyException(MilvaException.InvalidParameter);

            var enumerator = langValues.GetType().GetMethod("GetEnumerator").Invoke(langValues, null);
            enumerator.GetType().GetMethod("MoveNext").Invoke(enumerator, null);
            var entityType = enumerator.GetType().GetProperty("Current").GetValue(enumerator, null).GetType();

            MethodInfo langMethod = typeof(HelperExtensions).GetMethod("GetLang", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(entityType);

            return langMethod.Invoke(langValues, new object[] { langValues, requestedPropName });
        }

        /// <summary>
        /// Performs the necessary mapping for language dependent objects. For example, it is used to map the data in the Product class to the ProductDTO class.
        /// </summary>
        /// <param name="langs"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        private static string GetLang<TEntity>(this HashSet<TEntity> langs, string propName)
        {
            var requestedLangId = GetLanguageId(GlobalConstants.DefaultLanguageId);

            if (langs.IsNullOrEmpty()) return "";

            TEntity requestedLang;

            if (requestedLangId != GlobalConstants.DefaultLanguageId) requestedLang = langs.FirstOrDefault(lang => (int)lang.GetType().GetProperty(SystemLanguageIdString).GetValue(lang) == requestedLangId)
                                                                                        ?? langs.FirstOrDefault(lang => (int)lang.GetType().GetProperty(SystemLanguageIdString).GetValue(lang) == GlobalConstants.DefaultLanguageId);

            else requestedLang = langs.FirstOrDefault(lang => (int)lang.GetType().GetProperty(SystemLanguageIdString).GetValue(lang) == GlobalConstants.DefaultLanguageId);

            requestedLang ??= langs.FirstOrDefault();

            return requestedLang.GetType().GetProperty(propName)?.GetValue(requestedLang, null)?.ToString();
        }

        /// <summary>
        /// Gets requested property value.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"> e.g : ProductLangs.Name </param>
        /// <returns></returns>
        public static object GetPropertyValue(this object obj, string propertyName)
        {
            var propNames = propertyName.Split('.').ToList();

            if (propNames.Count > 2) throw new MilvaUserFriendlyException(MilvaException.InvalidParameter);

            foreach (string propName in propNames)
            {
                if (typeof(IEnumerable).IsAssignableFrom(obj.GetType()))
                {
                    var count = (int)obj.GetType().GetProperty("Count").GetValue(obj, null);

                    var enumerator = obj.GetType().GetMethod("GetEnumerator").Invoke(obj, null);

                    List<object> listProp = new();

                    for (int i = 0; i < count; i++)
                    {
                        if (i == GlobalConstants.Zero) enumerator.GetType().GetMethod("MoveNext").Invoke(enumerator, null);

                        var currentValue = enumerator.GetType().GetProperty("Current").GetValue(enumerator, null);

                        var isLangPropExist = currentValue.GetType().GetProperties().Any(i => i.Name == "SystemLanguageId");
                        if (isLangPropExist)
                        {
                            var langId = (int)currentValue.GetType().GetProperty("SystemLanguageId").GetValue(currentValue, null);

                            if (langId == GetLanguageId(GlobalConstants.DefaultLanguageId))
                            {
                                obj = currentValue.GetType().GetProperty(propName).GetValue(currentValue, null);
                                break;
                            }
                        }
                        else
                        {
                            listProp.Add(currentValue.GetType().GetProperty(propName).GetValue(currentValue, null));
                        }

                        enumerator.GetType().GetMethod("MoveNext").Invoke(enumerator, null);
                    }
                    return listProp;

                }
                else obj = obj.GetType().GetProperty(propName).GetValue(obj, null);
            }

            return obj;
        }

        #endregion

        #region IEnumerable Helpers

        /// <summary>
        /// Checks guid list. If list is null or empty return default(<typeparamref name="TDTO"/>). Otherwise invoke <paramref name="returnFunc"/>.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TDTO"></typeparam>
        /// <param name="toBeCheckedList"></param>
        /// <param name="returnFunc"></param>
        /// <returns></returns>
        public static List<TDTO> CheckGuidList<TEntity, TDTO>(this IEnumerable<TEntity> toBeCheckedList, Func<IEnumerable<TEntity>, IEnumerable<TDTO>> returnFunc)
         where TDTO : new()
         where TEntity : class, IBaseEntity<Guid>
         => toBeCheckedList.IsNullOrEmpty() ? new List<TDTO>() : returnFunc.Invoke(toBeCheckedList).ToList();

        /// <summary>
        /// Checks int list. If list is null or empty return default(<typeparamref name="TDTO"/>). Otherwise invoke <paramref name="returnFunc"/>.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TDTO"></typeparam>
        /// <param name="toBeCheckedList"></param>
        /// <param name="returnFunc"></param>
        /// <returns></returns>
        public static List<TDTO> CheckIntList<TEntity, TDTO>(this IEnumerable<TEntity> toBeCheckedList, Func<IEnumerable<TEntity>, IEnumerable<TDTO>> returnFunc)
         where TDTO : new()
         where TEntity : class, IBaseEntity<int>
         => toBeCheckedList.IsNullOrEmpty() ? new List<TDTO>() : returnFunc.Invoke(toBeCheckedList).ToList();

        /// <summary>
        /// Checks guid object. If is null return default(<typeparamref name="TDTO"/>). Otherwise invoke <paramref name="returnFunc"/>.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TDTO"></typeparam>
        /// <param name="toBeCheckedObject"></param>
        /// <param name="returnFunc"></param>
        /// <returns></returns>
        public static TDTO CheckGuidObject<TEntity, TDTO>(this TEntity toBeCheckedObject, Func<TEntity, TDTO> returnFunc)
          where TDTO : new()
          where TEntity : class, IBaseEntity<Guid>
       => toBeCheckedObject == null ? default : returnFunc.Invoke(toBeCheckedObject);

        /// <summary>
        /// Checks int object. If is null return default(<typeparamref name="TDTO"/>). Otherwise invoke <paramref name="returnFunc"/>.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TDTO"></typeparam>
        /// <param name="toBeCheckedObject"></param>
        /// <param name="returnFunc"></param>
        /// <returns></returns>
        public static TDTO CheckIntObject<TEntity, TDTO>(this TEntity toBeCheckedObject, Func<TEntity, TDTO> returnFunc)
         where TDTO : new()
         where TEntity : class, IBaseEntity<int>
         => toBeCheckedObject == null ? default : returnFunc.Invoke(toBeCheckedObject);

        #endregion

        #region Pagination Helpers

        /// <summary>
        /// Prepares pagination dto according to pagination parameters.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="repository"></param>
        /// <param name="pageIndex"></param>
        /// <param name="requestedItemCount"></param>
        /// <param name="orderByProperty"></param>
        /// <param name="orderByAscending"></param>
        /// <param name="condition"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        public static async Task<(IEnumerable<TEntity> entities, int pageCount, int totalDataCount)>
            PreparePagination<TEntity, TKey>(this IMilvaTemplateRepositoryBase<TEntity, TKey> repository,
                                             int pageIndex,
                                             int requestedItemCount,
                                             string orderByProperty = null,
                                             bool orderByAscending = false,
                                             Expression<Func<TEntity, bool>> condition = null,
                                             Func<IIncludable<TEntity>, IIncludable> includes = null)
            where TKey : struct, IEquatable<TKey>
            where TEntity : class, IBaseEntity<TKey>
            => await PreparePagination<IMilvaTemplateRepositoryBase<TEntity, TKey>, TEntity, TKey>(repository,
                                                                                                   pageIndex,
                                                                                                   requestedItemCount,
                                                                                                   orderByProperty,
                                                                                                   orderByAscending,
                                                                                                   condition,
                                                                                                   includes);

        /// <summary>
        /// Prepares pagination dto according to pagination parameters.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="repository"></param>
        /// <param name="pageIndex"></param>
        /// <param name="requestedItemCount"></param>
        /// <param name="orderByKeySelector"></param>
        /// <param name="orderByAscending"></param>
        /// <param name="condition"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        public static async Task<(IEnumerable<TEntity> entities, int pageCount, int totalDataCount)>
            PreparePagination<TEntity, TKey>(this IMilvaTemplateRepositoryBase<TEntity, TKey> repository,
                                             int pageIndex,
                                             int requestedItemCount,
                                             Expression<Func<TEntity, object>> orderByKeySelector = null,
                                             bool orderByAscending = false,
                                             Expression<Func<TEntity, bool>> condition = null,
                                             Func<IIncludable<TEntity>, IIncludable> includes = null)
            where TKey : struct, IEquatable<TKey>
            where TEntity : class, IBaseEntity<TKey>
            => await PreparePagination<IMilvaTemplateRepositoryBase<TEntity, TKey>, TEntity, TKey>(repository,
                                                                                                   pageIndex,
                                                                                                   requestedItemCount,
                                                                                                   orderByKeySelector,
                                                                                                   orderByAscending,
                                                                                                   condition,
                                                                                                   includes);

        /// <summary>
        /// Prepares pagination dto according to pagination parameters.
        /// </summary>
        /// <typeparam name="TRepository"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="repository"></param>
        /// <param name="pageIndex"></param>
        /// <param name="requestedItemCount"></param>
        /// <param name="orderByProperty"></param>
        /// <param name="orderByAscending"></param>
        /// <param name="condition"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        public static async Task<(IEnumerable<TEntity> entities, int pageCount, int totalDataCount)>
            PreparePagination<TRepository, TEntity, TKey>(this TRepository repository,
                                                          int pageIndex,
                                                          int requestedItemCount,
                                                          string orderByProperty = null,
                                                          bool orderByAscending = false,
                                                          Expression<Func<TEntity, bool>> condition = null,
                                                          Func<IIncludable<TEntity>, IIncludable> includes = null)
            where TRepository : IBaseRepository<TEntity, TKey, MilvaTemplateDbContext>
            where TKey : struct, IEquatable<TKey>
            where TEntity : class, IBaseEntity<TKey>
            => string.IsNullOrWhiteSpace(orderByProperty) ? await repository.GetAsPaginatedAsync(pageIndex,
                                                                                            requestedItemCount,
                                                                                            includes,
                                                                                            condition)
                                                     : await repository.GetAsPaginatedAndOrderedAsync(pageIndex,
                                                                                                      requestedItemCount,
                                                                                                      includes,
                                                                                                      orderByProperty,
                                                                                                      orderByAscending,
                                                                                                      condition);

        /// <summary>
        /// Prepares pagination dto according to pagination parameters.
        /// </summary>
        /// <typeparam name="TRepository"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="repository"></param>
        /// <param name="pageIndex"></param>
        /// <param name="requestedItemCount"></param>
        /// <param name="orderByKeySelector"></param>
        /// <param name="orderByAscending"></param>
        /// <param name="condition"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        public static async Task<(IEnumerable<TEntity> entities, int pageCount, int totalDataCount)>
            PreparePagination<TRepository, TEntity, TKey>(this TRepository repository,
                                                          int pageIndex,
                                                          int requestedItemCount,
                                                          Expression<Func<TEntity, object>> orderByKeySelector = null,
                                                          bool orderByAscending = false,
                                                          Expression<Func<TEntity, bool>> condition = null,
                                                          Func<IIncludable<TEntity>, IIncludable> includes = null)
            where TRepository : IBaseRepository<TEntity, TKey, MilvaTemplateDbContext>
            where TKey : struct, IEquatable<TKey>
            where TEntity : class, IBaseEntity<TKey>
            => orderByKeySelector == null ? await repository.GetAsPaginatedAsync(pageIndex,
                                                                                 requestedItemCount,
                                                                                 includes,
                                                                                 condition)
                                          : await repository.GetAsPaginatedAndOrderedAsync(pageIndex,
                                                                                           requestedItemCount,
                                                                                           includes,
                                                                                           orderByKeySelector,
                                                                                           orderByAscending,
                                                                                           condition);

        #endregion

        #region Exception Helpers

        /// <summary>
        /// Throwns <see cref="MilvaUserFriendlyException"/> if <paramref name="parameterObject"/> is null.
        /// </summary>
        /// <param name="parameterObject"></param>
        /// <param name="localizerKey"></param>
        public static void ThrowIfParameterIsNull(this object parameterObject, string localizerKey = null)
        {
            if (parameterObject == null)
            {
                if (string.IsNullOrWhiteSpace(localizerKey))
                {
                    throw new MilvaUserFriendlyException(MilvaException.NullParameter);
                }
                else
                {
                    throw new MilvaUserFriendlyException(localizerKey);
                }
            }
        }

        /// <summary>
        /// Throwns <see cref="MilvaUserFriendlyException"/> if <paramref name="list"/> is null or empty.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="localizerKey"></param>
        public static void ThrowIfListIsNullOrEmpty(this List<object> list, string localizerKey = null)
        {
            if (list.IsNullOrEmpty())
            {
                if (string.IsNullOrWhiteSpace(localizerKey))
                {
                    throw new MilvaUserFriendlyException(MilvaException.CannotFindEntity);
                }
                else
                {
                    throw new MilvaUserFriendlyException(localizerKey);
                }
            }
        }

        /// <summary>
        /// Throwns <see cref="MilvaUserFriendlyException"/> if <paramref name="list"/> is null or empty.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="localizerKey"></param>
        public static void ThrowIfParameterIsNullOrEmpty<T>(this List<T> list, string localizerKey = null) where T : IEquatable<T>
        {
            if (list.IsNullOrEmpty())
            {
                if (string.IsNullOrWhiteSpace(localizerKey))
                {
                    throw new MilvaUserFriendlyException(MilvaException.NullParameter);
                }
                else
                {
                    throw new MilvaUserFriendlyException(localizerKey);
                }
            }
        }

        /// <summary>
        /// Throwns <see cref="MilvaUserFriendlyException"/> if <paramref name="list"/> is null or empty.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="localizerKey"></param>
        public static void ThrowIfListIsNullOrEmpty(this IEnumerable<object> list, string localizerKey = null)
        {
            if (list.IsNullOrEmpty())
            {
                if (string.IsNullOrWhiteSpace(localizerKey))
                {
                    throw new MilvaUserFriendlyException(MilvaException.CannotFindEntity);
                }
                else
                {
                    throw new MilvaUserFriendlyException(localizerKey);
                }
            }
        }

        /// <summary>
        /// Throwns <see cref="MilvaUserFriendlyException"/> if <paramref name="list"/> is not null or empty.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="message"></param>
        public static void ThrowIfListIsNotNullOrEmpty(this IEnumerable<object> list, string message = null)
        {
            if (list.IsNullOrEmpty())
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    throw new MilvaUserFriendlyException(MilvaException.NullParameter);
                }
                else
                {
                    throw new MilvaUserFriendlyException(message);
                }
            }
        }

        /// <summary>
        /// Throwns <see cref="MilvaUserFriendlyException"/> if <paramref name="entity"/> is null.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="localizerKey"></param>
        public static void ThrowIfNullForGuidObject<TEntity>(this TEntity entity, string localizerKey = null) where TEntity : class, IBaseEntity<Guid>
        {
            if (entity == null)
            {
                if (string.IsNullOrWhiteSpace(localizerKey))
                {
                    throw new MilvaUserFriendlyException(MilvaException.CannotFindEntity, $"{LocalizerKeys.LocalizedEntityName}{nameof(TEntity)}");
                }
                else
                {
                    throw new MilvaUserFriendlyException(localizerKey);
                }
            }
        }

        /// <summary>
        /// Throwns <see cref="MilvaUserFriendlyException"/> if <paramref name="entity"/> is null.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="localizerKey"></param>
        public static void ThrowIfNullForIntObject<TEntity>(this TEntity entity, string localizerKey = null) where TEntity : class, IBaseEntity<int>
        {
            if (entity == null)
            {
                if (string.IsNullOrWhiteSpace(localizerKey))
                {
                    throw new MilvaUserFriendlyException(MilvaException.CannotFindEntity, $"{LocalizerKeys.LocalizedEntityName}{nameof(TEntity)}");
                }
                else
                {
                    throw new MilvaUserFriendlyException(localizerKey);
                }
            }
        }

        #endregion

        #region DateTime Helpers

        /// <summary>
        /// Compares <paramref name="date"/> for whether between <paramref name="startTime"/> and <paramref name="endTime"/>. 
        /// </summary>
        /// 
        /// <remarks>
        /// This is a time comparison not a date comparison.
        /// </remarks>
        /// 
        /// <param name="date"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static bool IsBetween(this DateTime date, TimeSpan startTime, TimeSpan endTime)
        {
            DateTime startDate = new(date.Year, date.Month, date.Day);
            DateTime endDate = startDate;

            //Check whether the endTime is lesser than startTime
            if (startTime >= endTime)
            {
                //Increase the date if endTime is timespan of the Nextday 
                endDate = endDate.AddDays(1);
            }

            //Assign the startTime and endTime to the Dates
            startDate = startDate.Date + startTime;
            endDate = endDate.Date + endTime;

            return (date >= startDate) && (date <= endDate);
        }

        /// <summary>
        /// Compares <paramref name="date"/> for whether between <paramref name="startDate"/> and <paramref name="endDate"/>. 
        /// </summary>
        /// <param name="date"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static bool IsBetween(this DateTime date, DateTime startDate, DateTime endDate) => (date >= startDate) && (date <= endDate);

        #endregion

        #region Mapping Helpers

        /// <summary>
        /// Getting cretion user datas.
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static TDTO MapAuditData<TDTO, TEntity>(this TDTO dto, TEntity entity)
            where TDTO : AuditableEntityWithCustomUser<MilvaTemplateUserDTO, Guid, Guid>
            where TEntity : IAuditable<MilvaTemplateUser, Guid, Guid>
        {
            dto.CreationDate = entity.CreationDate;
            dto.CreatorUser = new MilvaTemplateUserDTO
            {
                Id = entity.CreatorUser?.Id ?? default,
                UserName = entity.CreatorUser?.UserName ?? ""
            };
            dto.LastModificationDate = entity.LastModificationDate;
            dto.LastModifierUser = new MilvaTemplateUserDTO
            {
                Id = entity.LastModifierUser?.Id ?? default,
                UserName = entity.LastModifierUser?.UserName ?? ""
            };
            return dto;
        }

        /// <summary>
        /// Getting cretion user datas.
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static TDTO MapAuditDataForIntTypes<TDTO, TEntity>(this TDTO dto, TEntity entity)
            where TDTO : AuditableEntityWithCustomUser<MilvaTemplateUserDTO, Guid, int>
            where TEntity : IAuditable<MilvaTemplateUser, Guid, int>
        {
            dto.CreationDate = entity.CreationDate;
            dto.CreatorUser = new MilvaTemplateUserDTO
            {
                Id = entity.CreatorUser?.Id ?? default,
                UserName = entity.CreatorUser?.UserName ?? ""
            };
            dto.LastModificationDate = entity.LastModificationDate;
            dto.LastModifierUser = new MilvaTemplateUserDTO
            {
                Id = entity.LastModifierUser?.Id ?? default,
                UserName = entity.LastModifierUser?.UserName ?? ""
            };
            return dto;
        }

        #endregion
    }
}
