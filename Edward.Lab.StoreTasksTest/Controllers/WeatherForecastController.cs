using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Edward.Lab.StoreTasksTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        public static ConcurrentDictionary<Guid, Task> TaskStore = new ConcurrentDictionary<Guid, Task>();


        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Guid> Get()
        {
            return TaskStore.Keys;
        }

        [HttpPost]
        public Guid RunTask()
        {
            Action<Guid> onSuccess = (Guid id) =>
            {
                Console.WriteLine("成功");
            };
            Action<Guid, Exception> onFail = (Guid id, Exception e) =>
            {
                Console.WriteLine("失敗" + e);
            };

            Guid jobId = Guid.NewGuid();
            bool addJobId = true;
            TaskStore[jobId] = DoSomething()
                .ContinueWith(async (responses) =>
                {
                    if (responses.IsFaulted)
                    {
                        onFail?.Invoke(jobId, responses.Exception);
                    }
                    else
                    {
                        onSuccess?.Invoke(jobId);
                    }
                    TaskStore.Remove(jobId, out _);
                });

            return jobId;
        }

        private async Task DoSomething()
        {
            Console.WriteLine("等十秒");
            throw new NotImplementedException();
            await Task.Delay(10 * 1000);
            Console.WriteLine("十秒過了");
        }
    }
}
