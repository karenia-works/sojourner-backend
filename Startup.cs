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
using Sojourner.Models.Settings;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Sojourner.Interface;
using Sojourner.Repository;
using Sojourner.Store;
using IdentityServer4.Test;
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
            services.AddRouting();
            services.AddAuthentication().AddOpenIdConnect("ocid", "OpenID Connect",
            options =>
            {
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                options.SignOutScheme = IdentityServerConstants.SignoutScheme;
                options.Authority = "localhost:5000";
                options.ClientId = "cilent";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    RoleClaimType = "role"
                };
                options.RequireHttpsMetadata = false;
            }
            );
            //services.AddTransient<IRepository,MongoRepository>();
            //services.AddTransient<ICorsPolicyService,InMemoryCorsPolicyService>();
            //services.AddTransient<IResourceStore,CustomResourceStore>();
            //services.AddTransient<IPersistedGrantStore,CustomPersistedGrantStore>();
            services.AddIdentityServer().
            AddDeveloperSigningCredential().
            AddInMemoryClients(config.GetClients()).
            AddInMemoryApiResources(config.GetApiResources()).
            AddTestUsers(config.GetTestUsers());
            services.AddSingleton<OrderService>();
            services.AddSingleton<UserService>();
            services.AddSingleton<HousesService>();
            services.AddControllers();

            services.Configure<DbSettings>(Configuration.GetSection("DbSettings"));
            services.AddSingleton<IDbSettings>(settings => settings.GetRequiredService<IOptions<DbSettings>>().Value);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseIdentityServer();
            app.UseRouting();


            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
