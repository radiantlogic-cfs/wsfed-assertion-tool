using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Logging;
using WsFed_MVC.Helpers;

namespace WsFed_MVC
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            IdentityModelEventSource.ShowPII = true;

            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.LoginPath = "/WsFed/Login";
                })
                .AddWsFederation(options =>
                {
                    options.MetadataAddress = Configuration["auth:wsfed:IdPMetadata"];
                    options.Wtrealm = Configuration["auth:wsfed:Wtrealm"];
                    options.CallbackPath = "/Home/Index";
                    options.RequireHttpsMetadata = true;
                    options.SkipUnrecognizedRequests = true;
                    options.TokenHandlers.Add(new CustomSamlSecurityTokenHandler());
                    
                    options.Events.OnRemoteSignOut = async context =>
                    {
                        await context.HttpContext.SignOutAsync();
                    };
                });
            
            services.AddHttpClient();

            services.AddControllersWithViews();
            services.AddSingleton(Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            
            app.Use(async (context, next) =>
            {
                if (context.User.Identity is { IsAuthenticated: true } && context.Request.Path == "/")
                {
                    context.Response.Redirect("/Home/Dashboard");
                    return;
                }

                await next();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}