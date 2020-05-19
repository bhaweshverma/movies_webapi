using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MoviesAPI.Models;
using Microsoft.OpenApi.Models;
using MoviesAPI.Middleware;

namespace MoviesAPI
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
            services.AddControllersWithViews();
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("V1", new OpenApiInfo { Title = "MoviesAPI", Version="V1"});
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/v1/movies"), app =>{
                app.UseMiddleware<ValidateAuthorizationHeaderMiddleware>();
            });
            
            app.UseAuthorization();

            app.UseSwagger();
            
            app.UseSwaggerUI( c => {
                c.SwaggerEndpoint("./swagger/V1/swagger.json", "MoviesAPI");
                c.RoutePrefix = string.Empty;
            });
            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapControllers();
                /*endpoints.MapControllerRoute(
                    name:"Index",
                    pattern:"api/v1/{controller=Users}/action={Index}"
                );*/
                endpoints.MapControllerRoute(
                    name:"default",
                    pattern:"api/v1/{controller=Movies}/action={AllMovies}/{id?}"
                );
            });

            

        }
    }
}
