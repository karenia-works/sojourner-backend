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
using System.Security.Claims;
using Sojourner.Models.Settings;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Sojourner.Interface;
using Sojourner.Repository;
using Sojourner.Store;
using IdentityServer4.Test;
using MongoDB.Bson.Serialization;

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

            services.AddLocalApiAuthentication();

            services.AddRouting();
            services.AddControllers();

            services.AddSingleton<OrderService>();
            services.AddSingleton<UserService>();
            services.AddSingleton<HousesService>();
            services.AddSingleton<ImageService>();
            services.AddSingleton<ProfileService>();
            services.Configure<DbSettings>(Configuration.GetSection("DbSettings"));
            services.AddSingleton<IDbSettings>(settings => settings.GetRequiredService<IOptions<DbSettings>>().Value);
            services.AddSingleton<CheckService>();
            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryClients(config.GetClients())
                .AddInMemoryApiResources(config.GetApiResources()).AddResourceOwnerValidator<UserStore>();
            services.AddSingleton<ICorsPolicyService>(new DefaultCorsPolicyService(new LoggerFactory().CreateLogger<DefaultCorsPolicyService>())
            {
                AllowedOrigins = new[] { "*" },
                AllowAll = true
            });

            services.AddAuthorization(option =>
            {
                option.AddPolicy(
                "adminApi", policy =>
                {
                    policy.AddAuthenticationSchemes(IdentityServerConstants.LocalApi.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("Role", "admin");
                }
                );
                option.AddPolicy(
                "workerApi", policy =>
                {
                    policy.AddAuthenticationSchemes(IdentityServerConstants.LocalApi.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("Role", "worker");
                }
                );
            });


            BsonClassMap.RegisterClassMap<Address>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIdentityServer();

            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.UseCors(policy =>
            {
                policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
            });
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
