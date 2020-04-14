using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Books.Api.Contexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Books.Api.Services;
using AutoMapper;

namespace Books.Api
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
            services.AddControllers();

            var connectionString = Configuration["ConnectionStrings:BooksDBConnectionString"];
            services.AddDbContext<BooksContext>(o => o.UseSqlServer(connectionString));
            //run "add-migration InitialMigration"  in tools>nuget>package manager console

            services.AddScoped<IBooksRepository, BooksRepository>();
            /*this repository uses books context which is a DBcontext,so must register it with a scope equal to or shorter than the DBContext scope.
             * when we call add DBContext , the books context is registered with a scoped lifetime, so we can't use singleton lifetiime since that would be larger than db context scope.
             * if we use transitent> as each time we request service with transient lifetime a new instance is served up,so we loose any state that our repository might hold, if it is requested by multiptle parts of our code.
             * so use scoped lifetime.
             */

            //register Automapper
            services.AddAutoMapper();//it not only register services, i will also scan the code and look for classes that inherit from profile and if it finds one like "BooksProfile", it will load there configuration mappings.use services.AddAutoMapper(typeof(Startup)); in latest version

            services.AddHttpClient();
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

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
