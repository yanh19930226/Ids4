using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ids4Server.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Ids4Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build()
                .MigrateDbContext<ApplicationDbContext>((context, service) =>
                {
                    new ApplicationDbContextSeed().SeedAsync(context, service).Wait();
                })
                .Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
