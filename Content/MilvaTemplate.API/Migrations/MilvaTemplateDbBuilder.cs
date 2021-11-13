using Fody;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MilvaTemplate.API.Helpers.Extensions;
using MilvaTemplate.Data;
using System;
using System.Threading.Tasks;

namespace MilvaTemplate.API.Migrations;

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

        await dbContext.Database.EnsureDeletedAsync();

        await Console.Out.WriteAppInfoAsync("Database deleted. Database creation starting...");

        await dbContext.Database.MigrateAsync();

        await Console.Out.WriteAppInfoAsync("Database created. Seed starting...\n");

        //TODO reset methods will be here...

        dbContext.ActivateSoftDelete();

        await Console.Out.WriteAppInfoAsync("Database seed successfully completed.");
    }
}
