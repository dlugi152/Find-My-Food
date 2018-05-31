using System.Globalization;
using FindMyFood.Data;
using FindMyFood.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FindMyFood
{
    public class Startup
    {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(Configuration
                    .GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequiredLength = 5;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredUniqueChars = 3;
                    options.Password.RequireNonAlphanumeric = false;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add application services.
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true
                });
            }
            else {
                app.UseExceptionHandler("/Home/Error");
            }

            var supportedCultures = new[]
            {
                new CultureInfo("pl-PL")
            };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("pl-PL"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });
            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "default", "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    "spa-fallback",
                    new {controller = "Restaurant", action = "Index"});
            });
        }
    }
}