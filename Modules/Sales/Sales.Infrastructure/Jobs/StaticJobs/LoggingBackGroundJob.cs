using Microsoft.Extensions.Logging;
using Quartz;

namespace Sales.Infrastructure.Jobs.StaticJobs
{
    [DisallowConcurrentExecution]
    public class LoggingBackGroundJob(ILogger<LoggingBackGroundJob> logger) : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            var userId = context.MergedJobDataMap.GetInt("UserId");
            logger.LogInformation("{UtcNow}- UserId:{UserId}", DateTime.UtcNow, userId);
            return Task.CompletedTask;
        }
    }
}
