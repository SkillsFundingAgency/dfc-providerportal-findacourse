
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Swashbuckle.AspNetCore.Swagger;
using Dfc.ProviderPortal.FindACourse.Helpers;
using Dfc.ProviderPortal.FindACourse.Interfaces;
using Dfc.ProviderPortal.FindACourse.Models;
using Dfc.ProviderPortal.FindACourse.Services;
using Dfc.ProviderPortal.FindACourse.Settings;
using Dfc.ProviderPortal.Identity.Data;


namespace Dfc.ProviderPortal.FindACourse.API
{
    public class Startup
    {
        private readonly IHostingEnvironment _env;

        public Startup(IHostingEnvironment env, IConfiguration configuration)
        {
            _env = env;

            Configuration = //configuration;
                new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{_env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables()
                .AddApplicationInsightsSettings()
                .Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddSwaggerGen(c => { c.SwaggerDoc("v2", new Info { Title = "Courses API", Version = "v2" }); });

            services.Configure<CosmosDbCollectionSettings>(Configuration.GetSection(nameof(CosmosDbCollectionSettings)))
                    .Configure<CosmosDbSettings>(Configuration.GetSection(nameof(CosmosDbSettings)))
                    .Configure<ProviderServiceSettings>(Configuration.GetSection(nameof(ProviderServiceSettings)))
                    .Configure<VenueServiceSettings>(Configuration.GetSection(nameof(VenueServiceSettings)))
                    .Configure<SearchServiceSettings>(Configuration.GetSection(nameof(SearchServiceSettings)))
                    .Configure<FACAuthenticationSettings>(Configuration.GetSection(nameof(FACAuthenticationSettings)))
                    .AddScoped<ICourseService, CoursesService>()
                    .AddScoped<ICosmosDbHelper, CosmosDbHelper>()
                    .AddScoped<IProviderServiceWrapper, ProviderServiceWrapper>()
                    .AddScoped<IVenueServiceWrapper, VenueServiceWrapper>();
                    //.AddScoped<UserManager<APIUser>, UserManager<APIUser>>()
                    //.AddScoped<SignInManager<APIUser>, SignInManager<APIUser>>()
                    //.AddIdentity<APIUser, IdentityRole>(options => {
                    //     //options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
                    //     options.SignIn.RequireConfirmedEmail = false; })
                    //.AddEntityFrameworkStores<ApplicationDbContext>()
                    //.AddDefaultTokenProviders();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            } else {
                app.UseHsts();
            }

            //app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseMvc();

            app.UseSwagger(c => {
                c.PreSerializeFilters.Add((swaggerDoc, httpReq) => swaggerDoc.Host = httpReq.Host.Value);
            });
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/swagger/v2/swagger.json", "Courses API v2"); });
        }
    }
}
