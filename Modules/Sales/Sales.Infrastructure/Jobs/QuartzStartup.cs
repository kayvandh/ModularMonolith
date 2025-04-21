using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Sales.Infrastructure.DbContexts;
using Sales.Infrastructure.Jobs.DynamicJobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sales.Infrastructure.Jobs
{
    public static class QuartzStartup
    {
        public static async Task ScheduleAllJobsAsync(IServiceProvider serviceProvider)
        {
            var db = serviceProvider.GetRequiredService<SalesDbContext>();
            var schedulerFactory = serviceProvider.GetRequiredService<ISchedulerFactory>();
            var scheduler = await schedulerFactory.GetScheduler();

            var jobs = await db.ScheduledJobs
                .Where(j => j.Status == "Pending" && j.ScheduleTime > DateTime.UtcNow)
                .ToListAsync();

            foreach (var job in jobs)
            {
                var data = JsonSerializer.Deserialize<Dictionary<string, string>>(job.Payload)!;
                data.Add("jobId", job.Id.ToString());
                var jobDataMap = new JobDataMap(data);

                Type jobType = job.JobType switch
                {
                    "Email" => typeof(SendEmailJob),
                    "Sms" => typeof(SendSmsJob),
                    _ => throw new NotSupportedException()
                };

                var jobDetail = JobBuilder.Create(jobType)
                    .WithIdentity($"{job.JobType}-Job-{job.Id}")
                    .SetJobData(jobDataMap)
                    .Build();

                if (job.IsRecurring)
                {
                    var trigger = TriggerBuilder.Create()
                        .WithIdentity($"RecurringTrigger-{job.Id}")
                        .WithCronSchedule(job.CronExpression!)
                        .Build();

                    await scheduler.ScheduleJob(jobDetail, trigger);

                }
                else
                {
                    var trigger = TriggerBuilder.Create()
                        .WithIdentity($"{job.JobType}-Trigger-{job.Id}")
                        .StartAt(job.ScheduleTime)
                        .Build();

                    await scheduler.ScheduleJob(jobDetail, trigger);
                }

                job.Status = "Scheduled";
                job.ScheduledAt = DateTime.UtcNow;
            }

            await db.SaveChangesAsync();
        }

    }
}
