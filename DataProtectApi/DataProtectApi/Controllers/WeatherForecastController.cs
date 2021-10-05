using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataProtectApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly MyProtectedSettings _myProtectedSettings;
        private readonly Keys _keys;
        private readonly IConfiguration _configuration;

        public WeatherForecastController(ILogger<WeatherForecastController> logger
            , IOptions<MyProtectedSettings> myProtectedSettings
            , IOptions<Keys> keys
            , IConfiguration configuration)
        {
            _logger = logger;
            _myProtectedSettings = myProtectedSettings.Value;
            _keys = keys.Value;
            _configuration = configuration;

            var isSymmetric = _configuration.GetValue<string>("IsSymmetric");
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {

            var fileContentBase64 = GetBase64JsonFileContent("appsettings1.json");


            var rng = new Random();
            var weatherForcasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToList();

            weatherForcasts.Add(new WeatherForecast 
            {
                Date = DateTime.Now,
                TemperatureC = 37,
                Summary = _myProtectedSettings.DefaultConnection
            });


            return weatherForcasts;
        }


        private static string GetBase64FileContent(string path)
        {
            string base64FileContent = "";
            if (System.IO.File.Exists(path))
            {
                var bytesFileContent = System.IO.File.ReadAllBytes(path);
                var aesKeyBase64 = Environment.GetEnvironmentVariable("Keys__AesKey", EnvironmentVariableTarget.User);
                var aesHelper = new AesHelper(aesKeyBase64);
                base64FileContent = aesHelper.EncryptAsBase64(bytesFileContent);
            }
            return base64FileContent;
        }

        private static string GetBase64JsonFileContent(string path)
        {
            string base64FileContent = "";
            if (System.IO.File.Exists(path))
            {
                var fileContentBytes = System.IO.File.ReadAllBytes(path);
                var keyBase64 = AesUtil.GenerateKey(Environment.MachineName);
                
                base64FileContent = AesUtil.EncryptAsBase64(keyBase64, fileContentBytes);
            }
            return base64FileContent;
        }
    }
}
