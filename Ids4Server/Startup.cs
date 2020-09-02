using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Services;
using Ids4Server.Data;
using Ids4Server.Models;
using Ids4Server.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Ids4Server
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
            //添加DbContext
            services.AddDbContext<ApplicationDbContext>(options => { options.UseMySql(Configuration["ConnectionStrings:DefaultConnection"]); });
            //添加Identity
            services.AddIdentity<ApplicationUser, ApplicationUserRole>().AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            //修改默认严格密码模式
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                //options.Password.RequiredLength = 10;
            });
            services.AddIdentityServer()
               .AddDeveloperSigningCredential()
               //测试使用内存
               //.AddInMemoryClients(Config.GetClients())
               //.AddInMemoryApiResources(Config.GetResource())
               //.AddInMemoryIdentityResources(Config.GetIdentityResource())
               .AddConfigurationStore(options => {
                   options.ConfigureDbContext = builder =>
                   {
                       builder.UseMySql(Configuration["ConnectionStrings:DefaultConnection"], sql => sql.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name));
                   };
               })
               .AddOperationalStore(options => {
                   options.ConfigureDbContext = builder =>
                   {
                       builder.UseMySql(Configuration["ConnectionStrings:DefaultConnection"], sql => sql.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name));
                   };
               })
               //正式数据库
               .AddAspNetIdentity<ApplicationUser>()
               .Services.AddScoped<IProfileService, ProfileService>();
            //ConsentService
            services.AddScoped<ConsentService>();
            services.AddControllersWithViews();
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
                app.UseExceptionHandler("/Home/Error");
            }
            InitIdentityServerDatabase(app);
            app.UseIdentityServer();
            app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        public void InitIdentityServerDatabase(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
                var configurationDbContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                if (!configurationDbContext.Clients.Any())
                {
                    foreach (var item in Config.GetClients())
                    {
                        configurationDbContext.Clients.Add(item.ToEntity());
                    }
                    configurationDbContext.SaveChanges();
                }
                if (!configurationDbContext.ApiResources.Any())
                {
                    foreach (var item in Config.GetResource())
                    {
                        configurationDbContext.ApiResources.Add(item.ToEntity());
                    }
                    configurationDbContext.SaveChanges();
                }
                if (!configurationDbContext.IdentityResources.Any())
                {
                    foreach (var item in Config.GetIdentityResource())
                    {
                        configurationDbContext.IdentityResources.Add(item.ToEntity());
                    }
                    configurationDbContext.SaveChanges();
                }
            }
        }
    }
}
