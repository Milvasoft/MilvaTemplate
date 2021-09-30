using Fody;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MilvaTemplate.API.AppStartup;
using MilvaTemplate.Data;
using System.Threading.Tasks;

namespace MilvaTemplate.API.Migrations
{
    /// <summary>
    /// Static class that runs while the project is up and running the reset methods to fake data on the tables.
    /// </summary>
    [ConfigureAwait(false)]
    public static class MilvaTemplateDbBuilder
    {
        /// <summary>
        /// Seeds database.
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static async Task SeedDatabaseAsync(this IApplicationBuilder app)
        {
            var dbContext = app.ApplicationServices.GetRequiredService<MilvaTemplateDbContext>();

            DataSeed.Services = app.ApplicationServices;

            if (Startup.WebHostEnvironment.EnvironmentName == "Production")
                await dbContext.Database.EnsureDeletedAsync();

            await dbContext.Database.EnsureDeletedAsync();

            await dbContext.Database.MigrateAsync();

            //TODO reset methods will be here...

            MilvaTemplateDbContext.ActivateSoftDelete();
        }
    }
}