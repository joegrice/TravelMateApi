using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;

namespace TravelMateApi.Scheduler
{
    public class QJobScheduler
    {
        public async Task ScheduleJob()
        {
            var schedulerFactory = new StdSchedulerFactory();
            var scheduler = schedulerFactory.GetScheduler().Result;

            scheduler.Start().Wait();
            const int scheduleIntervalInMinute = 1;

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
