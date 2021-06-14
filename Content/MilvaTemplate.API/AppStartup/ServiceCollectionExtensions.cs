using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Milvasoft.Helpers.DataAccess.Abstract;
using Milvasoft.Helpers.DataAccess.Concrete;
using Milvasoft.Helpers.DataAccess.MilvaContext;
using Milvasoft.Helpers.DependencyInjection;
using Milvasoft.Helpers.FileOperations.Abstract;
using Milvasoft.Helpers.FileOperations.Concrete;
using Milvasoft.Helpers.Identity.Concrete;
using Milvasoft.Helpers.Mail;
using Milvasoft.Helpers.Models.Response;
using Milvasoft.Helpers.Utils;
using MilvaTemplate.API.Helpers;
using MilvaTemplate.API.Helpers.Identity;
using MilvaTemplate.API.Helpers.Swagger;
using MilvaTemplate.Data;
using MilvaTemplate.Data.Abstract;
using MilvaTemplate.Data.Concrete;
using MilvaTemplate.Entity.Identity;
using MilvaTemplate.Localization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MilvaTemplate.API.AppStartup
{
    /// <summary>
    /// Service collection helpers.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Applies configuration to cors.
        /// </summary>
        /// <param name="services"></param>
        public static void AddCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("ApiCorsPolicy", builder =>
                {
                    //builder.WithOrigins("https://localhost:3000")
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .Build();
                });

            });
        }

        /// <summary>
        /// Adds MVC services to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
        /// </summary>
        /// <param name="services"></param>
        public static void AddControllers(this IServiceCollection services)
        {
            services.AddControllers(opt =>
            {
                //opt.ModelBinderProviders.Insert(0, new JsonModelBinderProvider());
                opt.SuppressAsyncSuffixInActionNames = false;
                opt.EnableEndpointRouting = true;
            }).AddNewtonsoftJson(opt =>
            {
                opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                opt.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

            }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
              .ConfigureApiBehaviorOptions(options =>
              {
                  options.InvalidModelStateResponseFactory = actionContext =>
                  {
                      return CustomErrorResponse(actionContext);
                  };
              }).AddDataAnnotationsLocalization();
        }

        /// <summary>
        /// Adds MVC services to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
        /// </summary>
        /// <param name="services"></param>
        public static void AddVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(config =>
            {
                // Specify the default API Version
                config.DefaultApiVersion = new ApiVersion(1, 0);
                // If the client hasn't specified the API version in the request, use the default API version number 
                config.AssumeDefaultVersionWhenUnspecified = true;
                // Advertise the API versions supported for the particular endpoint
                config.ReportApiVersions = true;
            });
        }

        /// <summary>
        /// Configures AspNetCore Identity modules.
        /// </summary>
        /// <param name="services"></param>
        public static void AddIdentity(this IServiceCollection services)
        {
            Action<IdentityOptions> identityOptions = setupAction =>
            {
                setupAction.Lockout.DefaultLockoutTimeSpan = new TimeSpan(3, 1, 0);
                setupAction.Lockout.MaxFailedAccessAttempts = 5;
                setupAction.User.RequireUniqueEmail = true;
                setupAction.Password.RequireDigit = false;
                setupAction.Password.RequiredLength = 1;
                setupAction.Password.RequireLowercase = false;
                setupAction.Password.RequireNonAlphanumeric = false;
                setupAction.Password.RequireUppercase = false;
                setupAction.User.AllowedUserNameCharacters = "abcçdefghiıjklmnoöpqrsştuüvwxyzABCÇDEFGHIİJKLMNOÖPQRSŞTUÜVWXYZ0123456789-._";
            };

            services.AddIdentity<MilvaTemplateUser, MilvaTemplateRole>(identityOptions)
                    .AddEntityFrameworkStores<MilvaTemplateDbContext>()
                    .AddUserValidator<MilvaUserValidation<MilvaTemplateUser, Guid, IStringLocalizer<SharedResource>>>()
                    .AddErrorDescriber<MilvaIdentityDescriber<IStringLocalizer<SharedResource>>>()
                    .AddDefaultTokenProviders();
        }

        /// <summary>
        /// JWT Token configurations
        /// </summary>
        /// <param name="services"></param>
        /// <param name="jSONFile"></param>
        public static void AddJwtBearer(this IServiceCollection services, IJsonOperations jSONFile)
        {
            var tokenManagement = jSONFile.GetRequiredSingleContentCryptedFromJsonFileAsync<TokenManagement>(Path.Combine(GlobalConstants.RootPath,
                                                                                                                          "StaticFiles",
                                                                                                                          "JSON",
                                                                                                                          "tokenmanagement.json"),
                                                                                                             GlobalConstants.MilvaTemplateKey,
                                                                                                             new CultureInfo("tr-TR")).Result;

            services.AddSingleton<ITokenManagement>(tokenManagement);

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(jwtOpt =>
            {
                IStringLocalizer<SharedResource> GetLocalizerInstance(HttpContext httpContext)
                {
                    return httpContext.RequestServices.GetRequiredService<IStringLocalizer<SharedResource>>();
                }

                Task ReturnResponse(HttpContext httpContext, string localizerKey, int statusCode)
                {
                    if (!httpContext.Response.HasStarted)
                    {
                        var localizer = GetLocalizerInstance(httpContext);

                        ExceptionResponse validationResponse = new()
                        {
                            Message = localizer[localizerKey],
                            Success = false,
                            StatusCode = statusCode
                        };

                        httpContext.Response.ContentType = "application/json";
                        httpContext.Response.StatusCode = MilvaStatusCodes.Status200OK;
                        return httpContext.Response.WriteAsync(JsonConvert.SerializeObject(validationResponse));
                    }
                    return Task.CompletedTask;
                }

                jwtOpt.Events = new JwtBearerEvents()
                {
                    //Token içinde name kontrol etme
                    OnTokenValidated = (context) =>
                    {
                        if (string.IsNullOrEmpty(context.Principal.Identity.Name) || context.SecurityToken is not JwtSecurityToken accessToken)
                        {
                            var localizer = GetLocalizerInstance(context.HttpContext);

                            context.Fail(localizer["Unauthorized"]);
                            return ReturnResponse(context.HttpContext, "Unauthorized", MilvaStatusCodes.Status401Unauthorized);
                        }

                        return Task.CompletedTask;
                    },
                    OnForbidden = context =>
                    {
                        return ReturnResponse(context.HttpContext, "Forbidden", MilvaStatusCodes.Status403Forbidden);
                    },
                    OnChallenge = context =>
                    {
                        // Skip the default logic.
                        context.HandleResponse();

                        return ReturnResponse(context.HttpContext, "Unauthorized", MilvaStatusCodes.Status401Unauthorized);
                    },
                    OnAuthenticationFailed = context =>
                    {
                        string localizerKey = "Unauthorized";

                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            localizerKey = "TokenExpired";

                        return ReturnResponse(context.HttpContext, localizerKey, MilvaStatusCodes.Status401Unauthorized);
                    }
                };

                jwtOpt.RequireHttpsMetadata = false;
                jwtOpt.SaveToken = true;
                jwtOpt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(tokenManagement.Secret)),
                    ValidateIssuer = true,
                    ValidIssuer = tokenManagement.Issuer,
                    ValidateAudience = true,
                    ValidAudience = tokenManagement.Audience,
                    ValidateLifetime = true
                };
            });
        }

        /// <summary>
        /// Migration database connection clause.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="jSONFile"></param>
        public static void AddMilvaTemplateDbContext(this IServiceCollection services, IJsonOperations jSONFile)
        {
            string connectionString = jSONFile.GetRequiredSingleContentCryptedFromJsonFileAsync<string>(Path.Combine(GlobalConstants.RootPath,
                                                                                                                     "StaticFiles",
                                                                                                                     "JSON",
                                                                                                                     $"connectionstring.{Startup.WebHostEnvironment.EnvironmentName}.json"),
                                                                                                        GlobalConstants.MilvaTemplateKey,
                                                                                                        new CultureInfo("tr-TR")).Result;

            services.AddSingleton<IAuditConfiguration>(new AuditConfiguration(true, true, true, true, true, true));

            services.AddEntityFrameworkNpgsql().AddDbContext<MilvaTemplateDbContext>(opts =>
            {
                opts.UseNpgsql(connectionString, b => b.MigrationsAssembly("MilvaTemplate.API").EnableRetryOnFailure()).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });
        }

        /// <summary>
        /// Applies dependency injection to repositories.
        /// </summary>
        /// <param name="services"></param>
        public static void AddMilvaTemplateRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IMilvaTemplateRepositoryBase<,>), typeof(MilvaTemplateRepositoryBase<,>));
            services.AddScoped<IContextRepository<MilvaTemplateDbContext>, ContextRepository<MilvaTemplateDbContext>>();
        }

        /// <summary>
        /// Applies dependency injection to services.
        /// </summary>
        /// <param name="services"></param>
        public static void AddMilvaTemplateServices(this IServiceCollection services)
        {
            services.AddSingleton<SharedResource>();
            services.AddScoped<IJsonOperations, JsonOperations>();
            services.AddSingleton<IMilvaLogger, MilvaTemplateLogger>();
            services.AddHttpClient();
            services.AddHttpContextAccessor();

            services.AddSingleton<IMilvaMailSender>(new MilvaMailSender("websitelogs@milvasoft.com",
                                                                        new NetworkCredential("websitelogs@milvasoft.com", ""),
                                                                        587,
                                                                        "mail.milvasoft.com"));

            //Validation hatalarını optimize ettiğimiz için .net tarafından hata fırlatılmasını engelliyor.
            services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
        }

        /// <summary>
        /// Swagger Configuration
        /// </summary>
        /// <param name="services"></param>
        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1.0", new OpenApiInfo
                {
                    Version = "v1.0",
                    Title = "MilvaTemplate API",
                    Description = "MilvaTemplate API",
                    TermsOfService = new Uri("https://milvasoft.com"),
                    Contact = new OpenApiContact { Name = "Milvasoft Yazılım", Email = "info@milvasoft.com", Url = new Uri("https://milvasoft.com") },
                    License = new OpenApiLicense { Name = "MIT", Url = new Uri("https://opensource.org/licenses/MIT") }
                });

                options.SwaggerDoc("v1.1", new OpenApiInfo
                {
                    Version = "v1.1",
                    Title = "MilvaTemplate API",
                    Description = "MilvaTemplate API",
                    TermsOfService = new Uri("https://milvasoft.com"),
                    Contact = new OpenApiContact { Name = "Milvasoft Yazılım", Email = "info@milvasoft.com", Url = new Uri("https://milvasoft.com") },
                    License = new OpenApiLicense { Name = "MIT", Url = new Uri("https://opensource.org/licenses/MIT") }
                });


                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                  new OpenApiSecurityScheme
                  {
                   Reference = new OpenApiReference
                    {
                     Type = ReferenceType.SecurityScheme,
                     Id = "Bearer"
                   }
                  },
                  Array.Empty<string>()
                  }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);

                options.SchemaFilter<CustomAttributeSchemaFilter>();
                options.SchemaFilter<SwaggerExcludeFilter>();
                options.OperationFilter<RequestHeaderFilter>();
                options.OperationFilter<CustomAttributeOperationFilter>();
                options.DocumentFilter<ReplaceVersionWithExactValueInPathFilter>();
            });
        }

        #region Private Methods 

        /// <summary>
        /// Prepares custom validation model for response.
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        private static ObjectResult CustomErrorResponse(ActionContext actionContext)
        {
            var validationErrors = actionContext.ModelState.Where(modelError => modelError.Value.Errors.Count > GlobalConstants.Zero)
                                                           .Select(modelError => new ValidationError
                                                           {
                                                               ValidationFieldName = modelError.Key,
                                                               ErrorMessageList = modelError.Value.Errors.Select(i => i.ErrorMessage).ToList()
                                                           }).ToList();

            List<string> errorMessageList = new();

            validationErrors.ForEach(e => e.ErrorMessageList.ForEach(emg => errorMessageList.Add(emg)));

            var validationResponse = new ExceptionResponse
            {
                Success = false,
                Message = string.Join('~', errorMessageList),
                StatusCode = MilvaStatusCodes.Status600Exception,
                Result = new object(),
                ErrorCodes = new List<int>()
            };

            actionContext.HttpContext.Items.Add(new KeyValuePair<object, object>("StatusCode", MilvaStatusCodes.Status600Exception));

            return new OkObjectResult(validationResponse);
        }

        #endregion

    }
}
