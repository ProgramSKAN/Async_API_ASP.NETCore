using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Books.Api.Contexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Books.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //CreateHostBuilder(args).Build().Run();

            //IIS uses lot of threads in the threadpool(>30000).scalabity improvement is obvious if not more threads availiable on the thread pool.so to simulate this, throttle the threads available to ASP.NET core
            //so in websurge we can see difference in sync&async request.apart from this we need to simulate db io operation,because the io operated to get set of books in the project occurs instanatly.so simulate long running io operation in repository.
            ThreadPool.SetMaxThreads(Environment.ProcessorCount, Environment.ProcessorCount);

            //ensure migration is executed whenever application starts.
            var host = CreateHostBuilder(args).Build();
            using(var scope = host.Services.CreateScope())
            {
                try
                {
                    var context = scope.ServiceProvider.GetService<BooksContext>();
                    context.Database.Migrate();
                }
                catch(Exception ex)
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurrred while migrating the database");
                }
            }
            //run the web app
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
