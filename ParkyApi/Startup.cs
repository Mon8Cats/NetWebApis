using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ParkyApi.Data;
using ParkyApi.Mapper;
using ParkyApi.Repository;
using ParkyApi.Repository.IRepository;

namespace ParkyApi
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
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<INationalParkRepository, NationalParkRepository>();
            services.AddAutoMapper(typeof(ParkyMappings));
            services.AddSwaggerGen(options=>
            {
                // swagger/ParkyOpenApiSpec/swagger.json
                // SwaggerDoc(doc name, OpenApiInfo) 
                options.SwaggerDoc("ParkyOpenApiSpec", new Microsoft.OpenApi.Models.OpenApiInfo()
                {
                    // properties
                    Title = "Parky Api",
                    Version = "1",
                    Description = "Steve's Parky Api",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact()
                    {
                        Email = "steve_kim@mail.com",
                        Name = "Steve Kim",
                        Url = new Uri("http://www.steve_kim.com")
                    },
                    License = new Microsoft.OpenApi.Models.OpenApiLicense()
                    {
                        Name = "MIT License",
                        Url = new Uri("http://test.com/License")
                    }
                });

                // we set xml file name as project name in project properties
                var xmlCommentFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"; 
                var cmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentFile);
                options.IncludeXmlComments(cmlCommentsFullPath);
                // now the xml comments are showing in swagger UI
                // missing xml comment warnings -- modify project property -- #pragma warning disable/restore CS1591
            });

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                // end point -- uri and name
                options.SwaggerEndpoint("/swagger/ParkyOpenApiSpec/swagger.json", "Parky Api");
                // http://localhost:52567/swagger/index.html
                options.RoutePrefix = "";
                // http://localhost:52567/index.html -- now it is default page
            });
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
