#region Using Directives
using Fody;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Milvasoft.Helpers.Middlewares;
using MilvaTemplate.API.Helpers.Extensions;
using MilvaTemplate.API.Middlewares;
using MilvaTemplate.Localization;
using System;
#endregion

namespace MilvaTemplate.API.AppStartup
{
    /*
     
     TODO What to do in step by step;
        - Check the GlobalConstant.cs for unnecessary variables for this project.
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

        private static IServiceCollection _serviceCollection;

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
            _serviceCollection = services;

            Console.Out.WriteAppInfo("Service collection registration starting...");

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddCors();

            services.AddControllers();

            //services.AddMilvaRedisCaching();

            services.AddVersioning();

            services.AddIdentity();

            var jsonOperations = services.AddJsonOperations();

            services.AddJwtBearer(jsonOperations);

            services.AddMilvaTemplateDbContext(jsonOperations);

            services.AddMilvaTemplateRepositories();

            services.AddMilvaTemplateServices();

            services.AddSwagger();

            Console.Out.WriteAppInfo("All services registered to service collection.");
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
            {
                app.UseDeveloperExceptionPage();

                app.UseMilvaResponseTimeCalculator();
            }

            app.UseRequestLocalization();

            app.UseActivityLogger();

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

            app.ConfigureAppStartupAsync(_serviceCollection).Wait();

            Console.Out.WriteAppInfo($"Hosting environment : {WebHostEnvironment.EnvironmentName}");
            Console.Out.WriteAppInfo($"Application started. Press Ctrl+C to shut down.");
        }

        private void OnShutdown()
        {
        }
    }
}
