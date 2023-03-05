using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace InterfaceSecurityDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
        /// <summary>
        /// 检查header中的secret
        /// </summary>
        /// <param name="secret"></param>
        /// <param name="id"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpStatusCode CheckSecret([FromHeader] string secret, int id, int page = 1)

        {
            if (string.IsNullOrEmpty(secret))
                return HttpStatusCode.BadRequest;
            var tupleSecret = Commons.GetSecretParams(secret);
            if (!Commons.CheckSignature(tupleSecret.Item2, tupleSecret.Item3, tupleSecret.Item1))
                return HttpStatusCode.Unauthorized;
            return HttpStatusCode.OK;
        }
        /// <summary>
        /// 生成header中的secret
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public string GenerateSecret(int id)
        {
            var nonce = Guid.NewGuid();
            var timestamp = Commons.GetCurrentTimeStepNumber().ToString();
            var token =
                Commons.GetSignature(
                    Commons.GetSecretParams(new SecretModels
                    {
                        apptype = "Web",
                        nonce = nonce.ToString(),
                        timestamp = timestamp
                    }));

            var secret =
                $"timestamp={timestamp}&nonce={nonce}&apptype={"Web"}&signature={token}";
            return EncryptUtils.Base64Code(secret);
        }

    }
}