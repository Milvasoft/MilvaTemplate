using Milvasoft.Helpers.Exceptions;
using Milvasoft.Helpers.Extensions;
using MilvaTemplate.API.Helpers.Constants;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MilvaTemplate.API.Helpers.Extensions;

/// <summary>
/// Helper extensions methods for Ops!yon Project.
/// </summary>
public static partial class HelperExtensions
{
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
        var requestedLangId = GetLanguageId(GlobalConstant.DefaultLanguageId);

        if (langs.IsNullOrEmpty()) return string.Empty;

        var propName = propertyName.GetPropertyName();

        TEntity requestedLang;

        if (requestedLangId != GlobalConstant.DefaultLanguageId) requestedLang = langs.FirstOrDefault(lang => (int)lang.GetType().GetProperty(SystemLanguageIdString).GetValue(lang) == requestedLangId)
                                                                                    ?? langs.FirstOrDefault(lang => (int)lang.GetType().GetProperty(SystemLanguageIdString).GetValue(lang) == GlobalConstant.DefaultLanguageId);

        else requestedLang = langs.FirstOrDefault(lang => (int)lang.GetType().GetProperty(SystemLanguageIdString).GetValue(lang) == GlobalConstant.DefaultLanguageId);

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
        var requestedLangId = GetLanguageId(GlobalConstant.DefaultLanguageId);

        if (langs.IsNullOrEmpty()) return string.Empty;

        TEntity requestedLang;

        if (requestedLangId != GlobalConstant.DefaultLanguageId) requestedLang = langs.FirstOrDefault(lang => (int)lang.GetType().GetProperty(SystemLanguageIdString).GetValue(lang) == requestedLangId)
                                                                                    ?? langs.FirstOrDefault(lang => (int)lang.GetType().GetProperty(SystemLanguageIdString).GetValue(lang) == GlobalConstant.DefaultLanguageId);

        else requestedLang = langs.FirstOrDefault(lang => (int)lang.GetType().GetProperty(SystemLanguageIdString).GetValue(lang) == GlobalConstant.DefaultLanguageId);

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
                    if (i == GlobalConstant.Zero) enumerator.GetType().GetMethod("MoveNext").Invoke(enumerator, null);

                    var currentValue = enumerator.GetType().GetProperty("Current").GetValue(enumerator, null);

                    var isLangPropExist = currentValue.GetType().GetProperties().Any(i => i.Name == "SystemLanguageId");
                    if (isLangPropExist)
                    {
                        var langId = (int)currentValue.GetType().GetProperty("SystemLanguageId").GetValue(currentValue, null);

                        if (langId == GetLanguageId(GlobalConstant.DefaultLanguageId))
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
}
