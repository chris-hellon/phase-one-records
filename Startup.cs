using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using PhaseOneRecords.RestApi.ShopifyApi;
using PhaseOneRecords.RestApi.SpotifyAPI;
using Rollbar;
using ShopifySharp;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhaseOneRecords
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews().AddRazorRuntimeCompilation();

            //Spotify
            var config = SpotifyClientConfig.CreateDefault()
                .WithAuthenticator(new ClientCredentialsAuthenticator(
                    Configuration["SpotifyApi:ClientId"], Configuration["SpotifyApi:ClientSecret"]));

            services.AddHttpContextAccessor();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromDays(365);
            });

            services.AddSingleton<ISpotifyApiClient, SpotifyApiClient>(s=> new SpotifyApiClient(config, Configuration["SpotifyApi:ClientId"], Configuration["SpotifyApi:ClientSecret"]));

            var shopifyOptions = new ShopifyStorefrontApiOptions(Configuration);

            var graphClient = new GraphQLHttpClient(shopifyOptions.BuildGraphUrl(), new NewtonsoftJsonSerializer());

            services.AddSingleton<GraphQLHttpClient, GraphQLHttpClient>(client => graphClient);
            services.AddSingleton<IShopifyApiClient, ShopifyStorefrontApiClient>(options => new ShopifyStorefrontApiClient(graphClient, shopifyOptions));

            //bundler
            services.AddWebOptimizer(pipeline => {
                pipeline.AddCssBundle("/css/main.css", "/css/**/*.css", "/css/*.css");
                pipeline.AddLessBundle("/css/global.css", "/css/less/**/*.less");
                pipeline.AddJavaScriptBundle("/js/main.js", "/js/**/*.js", "/js/*.js");
            });

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                RollbarLocator.RollbarInstance.Configure(new RollbarConfig(Configuration["RollbarKey"]));
                RollbarLocator.RollbarInstance.Info("Rollbar is configured properly.");

                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();

             
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            //bundlers
            app.UseWebOptimizer();
        }
    }
}
