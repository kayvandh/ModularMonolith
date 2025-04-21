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
    public class SendEmailJob : IJob
    {
        private readonly SendEmailUseCase useCase;
        private readonly SalesDbContext dbContext;

        public SendEmailJob(SendEmailUseCase useCase, SalesDbContext dbContext)
        {
            this.useCase = useCase;
            this.dbContext = dbContext;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var jobId = int.Parse(context.MergedJobDataMap.GetString("jobId")!);
            var email = context.MergedJobDataMap.GetString("email");

            var job = await dbContext.ScheduledJobs.FindAsync(jobId);
            if (job == null) return;

            try
            {
                await useCase.ExecuteAsync(email!);
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
