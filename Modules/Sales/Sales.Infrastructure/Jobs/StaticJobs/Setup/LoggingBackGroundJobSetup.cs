using Microsoft.Extensions.Options;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.Infrastructure.Jobs.StaticJobs.Setup
{
    public class LoggingBackGroundJobSetup : IConfigureOptions<QuartzOptions>
    {
        public void Configure(QuartzOptions options)
        {
            //var jobKey = JobKey.Create(nameof(LoggingBackGroundJob));
            //options.AddJob<LoggingBackGroundJob>(jobBuilder => 
            //    jobBuilder.WithIdentity(jobKey)
            //                .UsingJobData("UserId", "1500"))
            //        .AddTrigger(trigger => trigger
            //                .ForJob(jobKey)
            //                .WithSimpleSchedule(schedule => schedule.WithIntervalInSeconds(5).RepeatForever()));
        }
    }
}
