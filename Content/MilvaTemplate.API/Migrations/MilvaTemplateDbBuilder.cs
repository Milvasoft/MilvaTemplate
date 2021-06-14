using Fody;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Milvasoft.Helpers.Caching;
using MilvaTemplate.API.AppStartup;
using MilvaTemplate.Data;
using System;
using System.Threading.Tasks;

namespace MilvaTemplate.API.Migrations
{
    /// <summary>
    /// Static class that runs while the project is up and running the reset methods to fake data on the tables.
    /// </summary>
    [ConfigureAwait(false)]
    public static class MilvaTemplateDbBuilder
    {
        public static async Task SeedDatabase(this IApplicationBuilder app)
        {
            var dbContext = app.ApplicationServices.GetRequiredService<MilvaTemplateDbContext>();

            var redisCacheService = app.ApplicationServices.GetRequiredService<IRedisCacheService>();

            try
            {
                await redisCacheService.ConnectAsync();
                if (redisCacheService.IsConnected())
                    await redisCacheService.FlushDatabaseAsync();
            }
            catch (Exception)
            {
            }

            DataSeed.Services = app.ApplicationServices;

            if (Startup.WebHostEnvironment.EnvironmentName == "Production")
                await dbContext.Database.EnsureDeletedAsync();

            await dbContext.Database.EnsureDeletedAsync();

            await dbContext.Database.MigrateAsync();

            DataSeed.ResetAnyway = false;

            MilvaTemplateDbContext.ActivateSoftDelete();
        }
    }
}