using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using BizMall.Data.DBContexts;
using BizMall.Models;
using BizMall.Services;
using BizMall.Data.Repositories.Abstract;
using BizMall.Data.Repositories.Concrete;
using BizMall.Models.CompanyModels;

namespace BizMall
{
    public class AppSettings
    {
        public CategoryType CategoryType { get; set; }
        public string HeaderTitle { get; set; }
        public string FooterTitle { get; set; }
        public string ApplicationTitle { get; set; }
        public string CountOfSimilarArticlesOnArticlePage { get; set; }
        //размеры страниц
        public string PageSize { get; set; }
        public string PageAdminSize { get; set; }
        public string PageSearchSize { get; set; }
    }

    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();

                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            var tmp = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>(o =>
            {
                // add TimeSpan with N minutes plus timezone difference from Utc time
                o.Cookies.ApplicationCookie.ExpireTimeSpan = DateTime.Now.Subtract(DateTime.UtcNow).Add(TimeSpan.FromMinutes(45));

            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc();

            // Add application services.
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddTransient<IRepositoryArticle, RepositoryArticle>();
            services.AddTransient<IRepositoryCompany, RepositoryCompany>();
            services.AddTransient<IRepositoryUser, RepositoryUser>();
            services.AddTransient<IRepositoryImage, RepositoryImage>();
            services.AddTransient<IRepositoryCategory, RepositoryCategory>();
            services.AddTransient<IRepositoryKW, RepositoryKW>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseApplicationInsightsRequestTelemetry();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseStaticFiles();

            app.UseIdentity();

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                routes.MapRoute(null,
                    "",
                    new
                    {
                        controller = "Home",
                        action = "IndexCat",
                        category = (string)null,
                        page = 1
                    }
                );

                routes.MapRoute(
                    null,
                    "Page{Page}",
                    defaults: new { controller = "Home", action = "IndexCat", Category = (string)null },
                    constraints: new { Page = @"\d+" }
                );

                routes.MapRoute(
                    null,
                    "ArticleDetails/{articleId}",
                    new { controller = "Home", action = "ArticleDetails" }
                );
                
                routes.MapRoute(null,
                    "Shops/{Shop}",
                    new { controller = "Home", action = "IndexShop" }
                );

                routes.MapRoute(null,
                    "Categories/{Category}",
                    new { controller = "Home", action = "IndexCat" }
                );

                routes.MapRoute(null,
                    "Categories/{Category}/Page{Page}",
                    new { controller = "Home", action = "IndexCat"},
                    constraints: new { page = @"\d+" }
                );

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=IndexCat}/{id?}");

            });
        }
    }
}
