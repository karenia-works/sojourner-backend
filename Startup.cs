using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sojourner.Services;
using Sojourner.Models;
using Sojourner.Models;
using Sojourner.Services;
using Sojourner.Models.Settings;
using IdentityServer4.AspNetIdentity;
namespace Sojourner
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
            services.Configure<DbSettings>(
                Configuration.GetSection(nameof(DbSettings)));
            services.AddSingleton<IDbSettings>(sp =>
            sp.GetRequiredService<IOptions<DbSettings>>().Value);
            //services.AddIdentityServer();
            services.AddRouting();
            services.AddSingleton<OrderService>();
            services.AddSingleton<UserService>();
            services.AddSingleton<HousesService>();
            services.AddMvc();
            // services.AddControllers();
            //     .AddJsonOptions(options => options.UseCamelCasing(false))
            //     .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.Configure<DbSettings>(Configuration.GetSection("DbSettings"));
            services.AddSingleton<IDbSettings>(settings => settings.GetRequiredService<IOptions<DbSettings>>().Value);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.EnvironmentName == "development")
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //app.UseIdentityServer();
            // app.UseHttpsRedirection();
            app.UseMvc();


            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

        }
    }
}
