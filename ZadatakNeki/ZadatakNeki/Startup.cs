using System;
using System.IO;
using System.Reflection;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Web.CodeGeneration.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Swagger;
using ZadatakNeki.Filter;
using ZadatakNeki.Models;
using ZadatakNeki.Repository;
using ZadatakNeki.Repositorys.IRepository;
using ZadatakNeki.Repositorys.Repository;

namespace ZadatakNeki
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
            // doodavanje autoMappera
            services.AddAutoMapper();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddDbContext<ToDoContext>(op => op.UseSqlServer(Configuration["ConnectionString:ZadatakDB"]));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Zadatak2-vezba",
                    Description = "Testni zadatak povezivanje metoda sa serverom",
                    TermsOfService = "None",
                    Contact = new Contact
                    {
                        Name = "Mirsan Kajovic",
                        Email = "mirsan.kajovic@bild-studio.net",
                        Url = "https://www.facebook.com/profile.php?id=100002829721051"
                    },
                    License = new License
                    {
                        Name = "Use under LICX",
                        Url = "https://example.com/license"
                    }
                });
                
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            // dodavanje filtera
            services.AddMvc(opt => opt.Filters.Add(new MajFilter()));

            // dodavanje DI
            services.AddScoped<IOsobaRepository, OsobaRepository>();
            services.AddScoped<IKancelarijaRepository, KancelarijaRepository>();
            services.AddScoped<IOsobaUredjajRepository, OsobaUredjajRepository>();
            services.AddScoped<IUredjajRepository, UredjajRepository>();
            services.AddScoped<IRepository<ClassNameModel>, Repository<ClassNameModel>>();
            // unitOfWork DI
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // koriscenje swagger-a
            app.UseSwagger();
            app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Zadatak");
                    c.RoutePrefix = string.Empty;
                }
            );

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
