using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataProtectApi
{
    // Encrypted configuration in ASP.NET Core
    // https://newbedev.com/encrypted-configuration-in-asp-net-core
    // https://visualstudiomagazine.com/articles/2019/09/26/decrypting-config-settings.aspx
    // Encrypted configuration in ASP.NET Core
    // https://stackoverflow.com/questions/36062670/encrypted-configuration-in-asp-net-core
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

            // Mapping User Secrets to a model
            services.Configure<MyProtectedSettings>(Configuration.GetSection("ConnectionStrings"));
            //services.Configure<Keys>(Configuration.GetSection("Keys"));
            //var serviceProvider = services.BuildServiceProvider();
            //var aesKey = serviceProvider.GetRequiredService<IOptions<Keys>>().Value.AesKey;


            //var base64aAsKey = Configuration.GetValue<string>("Keys:AesKey");
            //var keyBytes = Convert.FromBase64String(base64aAsKey);

            //var aesKey = new AesKey
            //{
            //    Key = keyBytes.Take(32).ToArray(),
            //    IV = keyBytes.Skip(32).Take(16).ToArray()
            //};










            // Configure protected config settings
            //services.AddProtectedConfiguration();
            //services.ConfigureProtected<MyProtectedSettings>(Configuration.GetSection("ConnectionStrings"));
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

    public class MyProtectedSettings
    {
        //public string ConnectionStrings { get; set; }
        public string DefaultConnection { get; set; }
    }

    public class Keys
    {
        public string AesKey { get; set; }
    }

    public class AesKey
    {
        public byte[] Key { get; set; }
        public byte[] IV { get; set; }
    }
}
