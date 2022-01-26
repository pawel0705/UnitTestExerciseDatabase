using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Benday.Presidents.Api.DataAccess;
using Benday.Presidents.Api.Features;
using Benday.Presidents.Api.Interfaces;
using Benday.Presidents.Api.Services;
using Benday.Presidents.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Benday.DataAccess;
using Benday.Presidents.Api.DataAccess.SqlServer;
using Benday.Presidents.Api.Models;
using Benday.Presidents.WebUI.Controllers;

namespace Benday.Presidents.WebUi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            RegisterTypes(services);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=President}/{action=Index}/{id?}");
            });

            CheckDatabaseHasBeenDeployed(app);
        }

        private void CheckDatabaseHasBeenDeployed(IApplicationBuilder app)
        {
            using (var scope =
                app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                using (var context = scope.ServiceProvider.GetService<PresidentsDbContext>())
                {
                    context.Database.Migrate();
                }
            }
        }

        void RegisterTypes(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddTransient<IUsernameProvider, HttpContextUsernameProvider>();

            services.AddTransient<IFeatureManager, FeatureManager>();

            services.AddTransient<Api.Services.ILogger, Logger>();

            services.AddDbContext<PresidentsDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("default")));

            services.AddTransient<IPresidentsDbContext, PresidentsDbContext>();

            services.AddTransient<IRepository<Person>, SqlEntityFrameworkPersonRepository>();

            services.AddTransient<IValidatorStrategy<President>, DefaultValidatorStrategy<President>>();
            services.AddTransient<IDaysInOfficeStrategy, DefaultDaysInOfficeStrategy>();

            services.AddTransient<IFeatureRepository, SqlEntityFrameworkFeatureRepository>();

            services.AddTransient<IPresidentService, PresidentService>();

            services.AddTransient<ITestDataUtility, TestDataUtility>();
        }
    }
}
