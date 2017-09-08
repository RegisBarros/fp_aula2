﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fiap2.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;

namespace Fiap2.Web
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
            //    services.AddScoped<Core.IPhotoService, PhotoService>();
            //    services.AddSingleton<Core.IPhotoService, PhotoService>();
            services.AddTransient<Core.IPhotoService, PhotoService>();
            services.AddMvc();
            services.AddMemoryCache();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();

                app.UseStaticFiles();
            }
            else
            {
                app.UseExceptionHandler("/Photos/Error");

                app.UseStaticFiles(new StaticFileOptions()
                {
                    OnPrepareResponse = ctx =>
                    {
                        const int durationInSeconds = 60 * 60 * 24;
                        ctx.Context.Response.Headers[HeaderNames.CacheControl] =
                            $"public,max-age={durationInSeconds}";
                    }
                });
            }


            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "list_photos_category",
                    template: "photos/{category}/{total?}",
                    defaults: new { controller = "Photos", action = "Category" });

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Photos}/{action=Index}/{id?}");

            });
        }
    }
}
