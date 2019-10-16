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
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using System.IO;



namespace TodoApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        //enable cors   
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // services.AddDbContext<TodoContext>(opt =>
            //   opt.UseSqlite("Data Source=TodoList.db"));

            services.AddDbContext<TodoContext>(opt =>
               opt.UseInMemoryDatabase("TodoList"));
            
            //adding logic for CORS
            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                builder =>
                {
                    builder.WithOrigins("https://localhost:4200",
                                        "http://localhost:4200",
                                        "https://simple-todo.azurewebsites.net/",
                                        "http://simple-todo.azurewebsites.net/")
                                        .AllowAnyHeader()
                                        .AllowAnyMethod();
                });
            });

        
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
              app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            

            // when a request comes in it will hit this first
            app.Use(async (context, next) => 
            {
              await next();

              if(context.Response.StatusCode == 404 && !Path.HasExtension(context.Request.Path.Value))
              {
                context.Request.Path = "/index.html";
                await next();
              }  

            });


            app.UseDefaultFiles();
            app.UseStaticFiles();



            app.UseRouting();

            app.UseAuthorization();

            // allow for specific origins
            app.UseCors(MyAllowSpecificOrigins);

            app.UseEndpoints(endpoints =>
            {
              endpoints.MapControllers();
            });
        }
    }
}
