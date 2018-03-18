using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;

namespace TravelMateApi.Scheduler
{
    public class TaskRunner
    {
        public async Task TaskRunnerCreate()
        {
            var schedulerFactory = new StdSchedulerFactory();
            var scheduler = schedulerFactory.GetScheduler().Result;

            scheduler.Start().Wait();
            const int scheduleIntervalInMinute = 1; //job will run every minute

            var jobKey = JobKey.Create("DisruptonChecker");

            var job = JobBuilder.Create<DisruptionChecker>().WithIdentity(jobKey).Build();
            var trigger = TriggerBuilder.Create()
                .WithIdentity("JobTrigger")
                .StartNow()
                .WithSimpleSchedule(x => x.WithIntervalInMinutes(scheduleIntervalInMinute).RepeatForever())
                .Build();

            await scheduler.ScheduleJob(job, trigger);
        }
    }
}
