using Quartz;
using Sales.Application.UseCases;
using Sales.Infrastructure.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.Infrastructure.Jobs.DynamicJobs
{
    public class SendSmsJob : IJob
    {
        private readonly SendSmsUseCase useCase;
        private readonly SalesDbContext dbContext;

        public SendSmsJob(SendSmsUseCase useCase, SalesDbContext dbContext)
        {
            this.useCase = useCase;
            this.dbContext = dbContext;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var jobId = int.Parse(context.MergedJobDataMap.GetString("jobId")!);
            var phone = context.MergedJobDataMap.GetString("phone");

            var job = await dbContext.ScheduledJobs.FindAsync(jobId);
            if (job == null) return;

            try
            {
                await useCase.ExecuteAsync(phone!);
                job.Status = "Executed";
                job.ExecutedAt = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                job.Status = "Failed";
                job.Error = ex.Message;
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
