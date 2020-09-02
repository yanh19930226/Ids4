using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MVCClient
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
            //���OpenIDConnect
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            }).AddCookie("Cookies")
                .AddOpenIdConnect("oidc", options =>
                {
                    options.SignInScheme = "Cookies";
                    options.Authority = "http://localhost:5000/";
                    options.RequireHttpsMetadata = false;
                    options.ClientId = "mvc";
                    options.ClientSecret = "secret";
                    options.SaveTokens = true;

                    #region MyRegion
                    //�ڶ��ֹ��ڷ���ProfileService������(����������Ҫ���ص���Ϣ)
                    //options.GetClaimsFromUserInfoEndpoint = true;
                    //options.ClaimActions.MapJsonKey("sub", "sub");
                    //options.ClaimActions.MapJsonKey("preferred_username", "preferred_username");
                    ////options.ClaimActions.MapJsonKey("sub", "sub");
                    //options.ClaimActions.MapJsonKey("avatar", "avatar");
                    //options.ClaimActions.MapCustomJson("role", jobj => jobj["role"].ToString());

                    //options.Scope.Add("offline_access");
                    //options.Scope.Add("openid");
                    //options.Scope.Add("profile"); 
                    #endregion
                });
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
            app.UseStaticFiles();

            app.UseRouting();
            //oidc
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

  
