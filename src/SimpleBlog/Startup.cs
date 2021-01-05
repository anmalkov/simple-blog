using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using SimpleBlog.Extensions;
using SimpleBlog.Repositories;
using SimpleBlog.Services;
using System;
using System.IO;
using System.Reflection;

namespace SimpleBlog
{
    public class Startup
    {
        private const string AppInsightsConnectionStringEnvironmentVariableName = "APPLICATIONINSIGHTS_CONNECTION_STRING";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var appInsightsConnectionString = Environment.GetEnvironmentVariable(AppInsightsConnectionStringEnvironmentVariableName);
            if (!string.IsNullOrEmpty(appInsightsConnectionString))
            {
                var options = new ApplicationInsightsServiceOptions
                {
                    EnableAdaptiveSampling = false,
                    EnablePerformanceCounterCollectionModule = false
                };
                services.AddApplicationInsightsTelemetry(options);
            }

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.LoginPath = "/login";
                    options.LogoutPath = "/logout";
                });
            services.AddAuthentication(options => 
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            });

            services.AddSingleton<IArticlesRepository, DiskArticlesRepository>();
            services.AddScoped<IArticlesService, ArticlesService>();

            services.AddSingleton<IArticleInfoRepository, DiskArticleInfoRepository>();
            services.AddSingleton<ITagsRepository, DiskTagsRepository>();

            services.AddSingleton<IPagesRepository, DiskPagesRepository>();
            services.AddScoped<IPagesService, PagesService>();

            services.AddSingleton<ISiteConfigurationRepository, DiskSiteConfigurationRepository>();
            services.AddSingleton<IImagesRepository, DiskImagesRepository>();

            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseBlogImages();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
