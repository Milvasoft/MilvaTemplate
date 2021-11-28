using Fody;
using Milvasoft.Helpers;
using Milvasoft.Helpers.DataAccess.EfCore.Concrete.Entity;
using Milvasoft.Helpers.Exceptions;
using Milvasoft.Helpers.Extensions;
using MilvaTemplate.API.DTOs.ContentDTOs;
using MilvaTemplate.API.Helpers.Constants;
using MilvaTemplate.API.Helpers.Extensions;
using MilvaTemplate.API.Services.Abstract;
using MilvaTemplate.Data;
using MilvaTemplate.Entity.Identity;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MilvaTemplate.API.Services.Concrete;

/// <summary>
/// An intermediate layer for data that the interface needs independent from anywhere.
/// </summary>
[ConfigureAwait(false)]
public class ContentService : IContentService
{
    private readonly MilvaTemplateDbContext _dbContext;

    /// <summary>
    /// Performs constructor injection for repository interfaces used in this service.
    /// </summary>
    /// <param name="dbContext"></param>
    public ContentService(MilvaTemplateDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    /// <summary>
    /// Gets required content by <paramref name="contentParameterList"/>.EntityName.
    /// </summary>
    /// <param name="contentParameterList"></param>
    /// <returns></returns>
    public async Task<List<Contents>> GetRequiredContentAsync(List<ContentParameters> contentParameterList)
    {
        List<Contents> allContents = new();

        if (!contentParameterList.IsNullOrEmpty())
        {
            #region Check Same Property And Entity

            HashSet<string> paramaters;
            HashSet<string> langs;

            foreach (var contentParameters in contentParameterList)
            {
                if (!contentParameters.RequestedLangProps.IsNullOrEmpty())
                {
                    langs = new HashSet<string>();

                    foreach (var lang in contentParameters.RequestedLangProps) { langs.Add(lang); }

                    //Aynı langProperty varsa hata fırlatıldı.
                    if (langs.Count != contentParameters.RequestedLangProps.Count)
                        throw new MilvaUserFriendlyException(MilvaException.InvalidParameter);
                }
                if (!contentParameters.RequestedProps.IsNullOrEmpty())
                {
                    paramaters = new HashSet<string>();

                    foreach (var prop in contentParameters.RequestedProps) { paramaters.Add(prop); }

                    //Aynı propertyName varsa hata fırlatıldı.
                    if (paramaters.Count != contentParameters.RequestedProps.Count)
                        throw new MilvaUserFriendlyException(MilvaException.InvalidParameter);
                }
            }

            #endregion

            foreach (var contentParameters in contentParameterList)
            {
                var entityName = contentParameters.EntityName;
                var requestedProps = contentParameters.RequestedProps;
                var requestedLangProps = contentParameters.RequestedLangProps;

                List<string> propNamesForProjection = new()
                {
                    EntityPropertyNames.Id,
                    EntityPropertyNames.CreationDate
                };

                if (!requestedProps.IsNullOrEmpty())
                {
                    if (requestedProps.Count > 3)
                        throw new MilvaUserFriendlyException(MilvaException.InvalidParameter);

                    propNamesForProjection.AddRange(requestedProps);
                }

                var assembly = Assembly.GetAssembly(typeof(MilvaTemplateUser));

                var type = assembly.GetTypes().FirstOrDefault(i => i.Name == entityName) ?? throw new MilvaUserFriendlyException(MilvaException.InvalidParameter);

                Type langType = null;

                if (!requestedLangProps.IsNullOrEmpty())
                {
                    if (requestedLangProps.Count > 2)
                        throw new MilvaUserFriendlyException(MilvaException.InvalidParameter);

                    propNamesForProjection.Add($"{type.Name}Langs");

                    langType = assembly.GetTypes().FirstOrDefault(i => i.Name == $"{type.Name}Lang")
                                    ?? throw new MilvaUserFriendlyException(MilvaException.InvalidParameter);
                }

                var createProjectionExpressionMethod = typeof(HelperExtensions).GetMethod(nameof(HelperExtensions.CreateProjectionExpression),
                                                                                          BindingFlags.Static | BindingFlags.Public);

                var projectionExpression = createProjectionExpressionMethod.MakeGenericMethod(type).Invoke(propNamesForProjection,
                                                                                                           new object[] { propNamesForProjection, requestedLangProps, langType });

                var taskResult = (Task)_dbContext.GetType()
                                                 .GetMethod("GetRequiredContentsAsync")
                                                 .MakeGenericMethod(type)
                                                 .Invoke(_dbContext, new object[] { projectionExpression });

                await taskResult;

                var resultProperty = taskResult.GetType().GetProperty("Result");

                var contentList = resultProperty.GetValue(taskResult);

                var count = (int)contentList.GetType().GetProperty("Count").GetValue(contentList, null);

                SortedList<DateTime, Content> contents = new();

                static object GetDefaultValue(Type t)
                {
                    if (t.IsValueType)
                    {
                        return Activator.CreateInstance(t);
                    }
                    else if (t.Name.Contains("String"))
                    {
                        return string.Empty;
                    }
                    else
                    {
                        return null;
                    }
                }

                if (count > GlobalConstant.Zero)
                {
                    var langPropName = $"{type.Name}Langs";

                    var enumerator = contentList.GetType().GetMethod("GetEnumerator").Invoke(contentList, null);

                    for (int i = 0; i < count; i++)
                    {
                        if (i == GlobalConstant.Zero)
                            enumerator.GetType().GetMethod("MoveNext").Invoke(enumerator, null);

                        var item = enumerator.GetType().GetProperty("Current").GetValue(enumerator, null);

                        PropertyInfo p1 = null;
                        PropertyInfo p2 = null;
                        PropertyInfo p3 = null;

                        if (!requestedProps.IsNullOrEmpty() && requestedProps.Count >= 1)
                            if (!CommonHelper.PropertyExists(item, requestedProps[0]))
                            {
                                throw new MilvaUserFriendlyException(MilvaException.InvalidParameter);
                            }
                            else p1 = item.GetType().GetProperty(requestedProps[0]);

                        if (!requestedProps.IsNullOrEmpty() && requestedProps.Count >= 2)
                            if (!CommonHelper.PropertyExists(item, requestedProps[1]))
                            {
                                throw new MilvaUserFriendlyException(MilvaException.InvalidParameter);
                            }
                            else p2 = item.GetType().GetProperty(requestedProps[1]);

                        if (!requestedProps.IsNullOrEmpty() && requestedProps.Count >= 3)
                            if (!CommonHelper.PropertyExists(item, requestedProps[2]))
                            {
                                throw new MilvaUserFriendlyException(MilvaException.InvalidParameter);
                            }
                            else p3 = item.GetType().GetProperty(requestedProps[2]);


                        if (!requestedLangProps.IsNullOrEmpty() && requestedLangProps.Count >= 1 && string.IsNullOrWhiteSpace(requestedLangProps[0]))
                            throw new MilvaUserFriendlyException(MilvaException.InvalidParameter);

                        if (!requestedLangProps.IsNullOrEmpty() && requestedLangProps.Count >= 2 && string.IsNullOrWhiteSpace(requestedLangProps[1]))
                            throw new MilvaUserFriendlyException(MilvaException.InvalidParameter);

                        var q = item.GetType().GetProperty(EntityPropertyNames.CreationDate).GetValue(item, null);

                        contents.Add((DateTime)item.GetType().GetProperty(EntityPropertyNames.CreationDate).GetValue(item, null), new Content
                        {
                            Id = item.GetType().GetProperty("Id").GetValue(item, null),
                            ContentProp = p1 != null
                                            ? p1.GetValue(item, null) ?? GetDefaultValue(p1.PropertyType)
                                            : null,
                            ContentProp1 = p2 != null
                                            ? p2.GetValue(item, null) ?? GetDefaultValue(p2.PropertyType)
                                            : null,
                            ContentProp2 = p3 != null
                                            ? p3.GetValue(item, null) ?? GetDefaultValue(p3.PropertyType)
                                            : null,
                            ContentLangProp = !requestedLangProps.IsNullOrEmpty() && requestedLangProps.Count >= 1 ? item.GetLangPropValue(langPropName, requestedLangProps[0]) : null,
                            ContentLangProp1 = !requestedLangProps.IsNullOrEmpty() && requestedLangProps.Count >= 2 ? item.GetLangPropValue(langPropName, requestedLangProps[1]) : null
                        });

                        enumerator.GetType().GetMethod("MoveNext").Invoke(enumerator, null);
                    }

                    allContents.Add(new Contents
                    {
                        Key = entityName,
                        ContentList = contents.OrderByDescending(i => i.Key).Select(i => i.Value).ToList(),
                    });
                }
            }
        }

        return allContents;
    }

    #region SpecMaxValues

    private static readonly ImmutableList<string> ValidMaxValuesEntities = new List<string>
        {
            string.Empty,
        }.ToImmutableList();

    /// <summary>
    /// Gets the requested property's(<paramref name="propName"/>) max value in requested entity by <paramref name="entityName"/>.
    /// </summary>
    /// <param name="entityName"></param>
    /// <param name="propName"></param>
    /// <returns></returns>
    public async Task<decimal> GetSpecMaxValueAsync(string entityName, string propName)
    {
        if (!ValidMaxValuesEntities.Contains(entityName)) throw new MilvaUserFriendlyException(MilvaException.InvalidParameter);

        var assembly = Assembly.GetAssembly(typeof(MilvaTemplateUser));
        var type = assembly.GetTypes().FirstOrDefault(i => i.Name == entityName) ?? throw new MilvaUserFriendlyException(MilvaException.CannotFindEntity);

        if (type.GetProperty(propName).PropertyType != typeof(decimal)) throw new MilvaUserFriendlyException(MilvaException.InvalidParameter);

        var taskResult = (Task)_dbContext.GetType().GetMethod("GetMaxValueAsync").MakeGenericMethod(type).Invoke(_dbContext, new object[] { propName });
        await taskResult;
        var resultProperty = taskResult.GetType().GetProperty("Result");
        var result = resultProperty.GetValue(taskResult);

        return (decimal)result;
    }

    #endregion
}
