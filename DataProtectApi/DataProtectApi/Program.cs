using Microsoft.AspNetCore.HostFiltering;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;


namespace DataProtectApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureAppConfiguration((webHostBuilder, config) =>
                    {
                        var env = webHostBuilder.HostingEnvironment;


                        if (!env.EnvironmentName.Equals("development", StringComparison.OrdinalIgnoreCase))
                        {
                            config.AddJsonFile2("appsettings.json", optional: true, true);
                        }
                        //config.AddJsonFile2($"appsettings.{env.EnvironmentName}.json", optional: true, true);
                    });
                    webBuilder.UseStartup<Startup>();
                });

    }
}
