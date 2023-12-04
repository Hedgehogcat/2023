using Exceptionless;
using Exceptionless.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExceptionLessDemo.Controllers
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

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            _logger.LogInformation("INF"+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            _logger.LogWarning("INF"+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            _logger.LogError("INF"+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            _logger.LogCritical("INF"+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            _logger.LogDebug("INF"+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            try
            {
                throw new Exception("Info996");

            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();

                // �����ָ����־��Դ������־����
                // ��־�������⼸��: Trace, Debug, Information, Warning, Error ,Critical ,None

                ExceptionlessClient.Default.SubmitLog("controllerGget", "This is so easy3", "Info");
                ExceptionlessClient.Default.CreateLog("controllerGget", "This is so easy4", "Info").AddTags("Exceptionless").Submit();


                /* // ������־
                ExceptionlessClient.Default.SubmitLog("Logging made easy"); 

                 // ���� Feature Usages
                 ExceptionlessClient.Default.SubmitFeatureUsage("MyFeature");
                 ExceptionlessClient.Default.CreateFeatureUsage("MyFeature").AddTags("Exceptionless").Submit();

                 // ����һ�� 404
                 ExceptionlessClient.Default.SubmitNotFound("/somepage");
                 ExceptionlessClient.Default.CreateNotFound("/somepage").AddTags("Exceptionless").Submit();

                 // ����һ���Զ����¼�
                 ExceptionlessClient.Default.SubmitEvent(new Event { Message = "Low Fuel", Type = "racecar", Source = "Fuel System" });
 */
            }
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            }).ToArray();


        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Postweather")]
        public IEnumerable<WeatherForecast> Post(WeatherForecast request)
        {
            _logger.LogInformation("LogInformation"+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            _logger.LogWarning("LogWarning"+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            _logger.LogError("LogError"+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            _logger.LogCritical("LogCritical"+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            _logger.LogDebug("LogDebug"+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            _logger.LogInformation($"LogInformation���²ֵ� ����:{request.TemperatureC} Code: {request.TemperatureF}"+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            Serilog.Log.ForContext("WeatherForecastController", request, true).Information($"SerilogInformation���²ֵ� ����:{request.TemperatureC} Code: {request.TemperatureF}");

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            }).ToArray();
        }
    }
}