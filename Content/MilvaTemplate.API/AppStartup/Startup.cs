#region Using Directives
using Fody;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Milvasoft.Helpers.FileOperations.Abstract;
using Milvasoft.Helpers.FileOperations.Concrete;
using MilvaTemplate.API.Helpers;
using MilvaTemplate.API.Middlewares;
using MilvaTemplate.Localization;
using System.Threading.Tasks;
#endregion

namespace MilvaTemplate.API.AppStartup
{
    /*
     
     TODO What to do in step by step;
        - Check the GlobalConstants.cs for unnecessary variables for this project.
        - Check the HelperExtensions.cs for unnecessary extensions for this project.
        - Check services and middlewares in this file.
        - Change the running port on IIS of the api in launchsetting.json.
        - Check the sample controller and service. (Account)
        - Check Migrations folder and add your necessary methods into classes.
        - Decrypt conncetionstring.*.json files and change connection string.
        - Enter mailsender password in ServiceCollectionExtensions.cs.
        - Change encryption keys.
        - Lastly and hardest, remove this comment block :)
    
     */

    /// <summary>
    /// Application configuration.
    /// </summary>
    [ConfigureAwait(false)]
    public class Startup
    {
        #region Fields

        /// <summary> Configuration value. </summary>
        private readonly IJsonOperations _jsonOperations;

        #endregion

        #region Properties

        /// <summary> WebHostEnvironment value. </summary>
        public static IWebHostEnvironment WebHostEnvironment { get; set; }

        /// <summary>
        /// For access shared resources.
        /// </summary>
        public static IStringLocalizer<SharedResource> SharedStringLocalizer { get; set; }

        #endregion

        /// <summary>
        /// Initializes new instance of <see cref="Startup"/>.
        /// </summary>
        /// <param name="env"></param>
        public Startup(IWebHostEnvironment env)
        {
            WebHostEnvironment = env;
            _jsonOperations = new JsonOperations();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            //Will be remove production.
            //StartupConfiguration.EncryptFile().Wait();
            //StartupConfiguration.DecryptFile().Wait();

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddCors();

            services.AddControllers();

            //services.AddMilvaRedisCaching();

            services.AddVersioning();

            services.AddIdentity();

            services.AddJwtBearer(_jsonOperations);

            services.AddMilvaTemplateDbContext(_jsonOperations);

            services.AddMilvaTemplateRepositories();

            services.AddMilvaTemplateServices();

            services.AddSwagger();

            StartupConfiguration.FillStringBlacklistAsync(_jsonOperations).Wait();

            services.AddSingleton(GlobalConstants.StringBlacklist);
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="sharedStringLocalizer"></param>
        /// <param name="applicationLifetime"></param>
        public void Configure(IApplicationBuilder app, IStringLocalizer<SharedResource> sharedStringLocalizer, IHostApplicationLifetime applicationLifetime)
        {
            //Initializes string localizer 
            SharedStringLocalizer = sharedStringLocalizer;

            applicationLifetime.ApplicationStopped.Register(OnShutdown);

            if (WebHostEnvironment.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseRequestLocalization();

            app.UseMilvaTemplateExceptionHandler();

            StartupConfiguration.CheckPublicFiles();

            app.UseStaticFiles();

            app.UseDirectoryBrowser();

            app.UseRouting();

            app.UseCors("ApiCorsPolicy");

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints();

            app.UseSwagger();

            ConfigureAppStartupAsync(app).Wait();
        }

        /// <summary>
        /// This method provides async configure process which configure() called by the runtime.
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public async Task ConfigureAppStartupAsync(IApplicationBuilder app)
        {
            //await app.SeedDatabase();

            //await app.LoadLanguageCodesAsync();

            await StartupConfiguration.FillAllowedFileExtensionsAsync(_jsonOperations);

            await StartupConfiguration.FillStringBlacklistAsync(_jsonOperations);
        }

        private void OnShutdown()
        {
        }
    }
}
