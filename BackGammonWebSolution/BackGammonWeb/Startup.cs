using BackGammonDb;
using BackGammonWeb.Helpers;
using BackGammonWeb.Hubs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.Tasks;

namespace BackGammonWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        readonly string CorsPolicy = "CorsPolicy";

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<DbManager>();

           // services.AddSingleton<ServerHub>();

            services.AddSingleton(Configuration);

            services.AddSingleton<IUserIdProvider, NameUserIdProvider>();

            // Enable cookie authentication
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie();

            services.AddHttpContextAccessor();


            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            services.AddCors(o => o.AddPolicy(CorsPolicy, builder =>
            {
                builder

                .WithOrigins("http://localhost:4300")
                .WithOrigins("http://localhost:4200")
                .WithOrigins("http://michaelt-001-site3.btempurl.com")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                // .AllowAnyOrigin()
                ;
            }));

            //services.AddSignalR(hubOptions =>
            //{
            //    hubOptions.EnableDetailedErrors = true;
            //      hubOptions.KeepAliveInterval = TimeSpan.FromMinutes(20);
            //});
            services.AddSignalR(o =>
            {
                o.EnableDetailedErrors = true;
            });

            //JWT Config

            var appSettingsSection = Configuration.GetSection("AppSetting");
            services.Configure<AppSettings>(appSettingsSection);

            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments("/backgammon")))
                        {
                            // Read the token out of the query string
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });
            IdentityModelEventSource.ShowPII = true;

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            // Add authentication to request pipeline
            app.UseAuthentication();

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
            app.UseSpaStaticFiles();


            app.UseCors(CorsPolicy);

            app.UseSignalR(routes =>
            {
                routes.MapHub<ServerHub>("/backgammon");
            });

            app.UseMvc(routes =>
            {
            routes.MapRoute(
                name: "default",
                template: "{controller}/{action}/{id?}");

                //routes.MapSpaFallbackRoute("spa-fallback", new { controller = "Authentication", action = "Login" });

            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    // spa.UseAngularCliServer(npmScript: "start");
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
                }
            });
        }
    }
}
