using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Milvasoft.Core.EntityBase.Abstract;
using MilvaTemplate.Data;
using MilvaTemplate.Entity.Identity;

namespace MilvaTemplate.API.Migrations;

/// <summary>
/// Defines the <see cref="DataSeed" />.
/// </summary>
[ConfigureAwait(false)]
public static class DataSeed
{
    public const string AdminUserName = "templateAdmin";

    /// <summary>
    /// Contains dipendency injection services.
    /// </summary>
    public static IServiceProvider Services { get; set; }

    /// <summary>
    /// Resets the data of type of T. This method is only calls from Reset extension method of indelible tables.
    /// </summary>
    /// <param name="entityList">The entityList<see cref="List{TEntity}"/>.</param>
    private static async Task InitializeTableAsync<TEntity, TKey>(List<TEntity> entityList) where TEntity : class, IBaseEntity<TKey>
                                                                                            where TKey : struct, IEquatable<TKey>
    {
        var dbContext = Services.GetRequiredService<MilvaTemplateDbContext>();

        foreach (var entity in entityList) //For development
        {
            await dbContext.Set<TEntity>().AddAsync(entity);
            await dbContext.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Initilizes roles. This is a default data.
    /// </summary>
    /// <param name="roleManager"></param>
    /// <returns></returns>
    public static async Task InitializeRolesAsync(this RoleManager<MilvaTemplateRole> roleManager)
    {
        var roles = new List<MilvaTemplateRole>
        {
            new MilvaTemplateRole
            {
                Id = 1.ToGuid(),
                Name = RoleName.Administrator,
            },
        };

        foreach (var role in roles)
            if (roleManager.Roles.FirstOrDefault(r => r.Name == role.Name) == null)
                await roleManager.CreateAsync(role);
    }

    /// <summary>
    /// Initializes users.
    /// </summary>
    /// <param name="userManager"></param>
    public static async Task InitializeUsersAsync(this UserManager<MilvaTemplateUser> userManager)
    {
        var users = new List<MilvaTemplateUser>()
        {
            new MilvaTemplateUser {
                Id = 1.ToGuid(),
                UserName = AdminUserName,
                Email = "templateAdmin@templateAdmin.com",
                PhoneNumber = "0 500 000 00 00",
            }
        };

        foreach (var user in users)
        {
            var password = $"{user.UserName}-!";

            await userManager.CreateAsync(user, password);

            if (user.UserName == AdminUserName)
            {
                await userManager.AddToRoleAsync(user, RoleName.Administrator);
            }
        }
    }

    /// <summary>
    /// Initializes SystemLanguages.
    /// </summary>
    /// <returns></returns>
    public static async Task InitializeLanguagesAsync()
    {
        List<SystemLanguage> languageList = new()
        {
            new()
            {
                Id = 1,
                LanguageName = "Türkçe",
                IsoCode = "tr-TR",
                CreationDate = DateTime.Now.Date,
                IsDeleted = false
            },
            new()
            {
                Id = 2,
                LanguageName = "English(US)",
                IsoCode = "en-US",
                CreationDate = DateTime.Now.Date,
                IsDeleted = false
            },
            new()
            {
                Id = 3,
                LanguageName = "Azərbaycan Dili",
                IsoCode = "az-AZ",
                CreationDate = DateTime.Now.Date,
                IsDeleted = false
            },
            new()
            {
                Id = 4,
                LanguageName = "Ελληνικά",
                IsoCode = "el-GR",
                CreationDate = DateTime.Now.Date,
                IsDeleted = false
            },
            new()
            {
                Id = 5,
                LanguageName = "Deutsche",
                IsoCode = "de-DE",
                CreationDate = DateTime.Now.Date,
                IsDeleted = false
            },
            new()
            {
                Id = 6,
                LanguageName = "Nederlands",
                IsoCode = "nl-NL",
                CreationDate = DateTime.Now.Date,
                IsDeleted = false
            },
            new()
            {
                Id = 7,
                LanguageName = "English(UK)",
                IsoCode = "en-GB",
                CreationDate = DateTime.Now.Date,
                IsDeleted = false
            },
            new()
            {
                Id = 8,
                LanguageName = "Español",
                IsoCode = "es-ES",
                CreationDate = DateTime.Now.Date,
                IsDeleted = false
            },
            new()
            {
                Id = 9,
                LanguageName = "Français",
                IsoCode = "fr-FR",
                CreationDate = DateTime.Now.Date,
                IsDeleted = false
            },
            new()
            {
                Id = 10,
                LanguageName = "italiano",
                IsoCode = "it-IT",
                CreationDate = DateTime.Now.Date,
                IsDeleted = false
            },
            new()
            {
                Id = 11,
                LanguageName = "русский",
                IsoCode = "ru-RU",
                CreationDate = DateTime.Now.Date,
                IsDeleted = false
            },
            new()
            {
                Id = 12,
                LanguageName = "русский",
                IsoCode = "ru-RU",
                CreationDate = DateTime.Now.Date,
                IsDeleted = false
            },
            new()
            {
                Id = 13,
                LanguageName = "中文",
                IsoCode = "zh-CHS",
                CreationDate = DateTime.Now.Date,
                IsDeleted = false
            },
            new()
            {
                Id = 14,
                LanguageName = "日本人",
                IsoCode = "ja-JP",
                CreationDate = DateTime.Now.Date,
                IsDeleted = false
            },
            new()
            {
                Id = 15,
                LanguageName = "हिंदी",
                IsoCode = "hi-IN",
                CreationDate = DateTime.Now.Date,
                IsDeleted = false
            }
        };

        await InitializeTableAsync<SystemLanguage, int>(languageList).ConfigureAwait(false);
    }
}
