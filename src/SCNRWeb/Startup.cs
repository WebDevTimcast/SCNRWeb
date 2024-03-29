using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ON.Authentication;
using ON.Settings;
using SCNRWeb.Helper;
using SCNRWeb.Models;
using SCNRWeb.Services;

namespace SCNRWeb
{
    public class Startup
    {
        private static byte[] PONG_RESPONSE = { (byte)'p', (byte)'o', (byte)'n', (byte)'g' };

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddControllersWithViews();

            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            services.AddScoped<AssetService>();
            services.AddScoped<ContentService>();
            services.AddScoped<ContentUrlHelper>();
            services.AddScoped<UserService>();

            services.AddSingleton<SettingsService>();

            services.AddJwtAuthentication();
            services.AddSettingsHelpers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.Map("/ping", (app1) => app1.Run(async context => {
                await context.Response.BodyWriter.WriteAsync(PONG_RESPONSE);
            }));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseJwtAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
