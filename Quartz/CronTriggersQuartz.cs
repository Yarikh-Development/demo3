using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demo.Quartz
{
    class CronTriggersQuartz
    {
        public static async Task UsingQAsync()
        {
            // construct a scheduler factory
            StdSchedulerFactory factory = new StdSchedulerFactory();

            // get a scheduler
            IScheduler scheduler = await factory.GetScheduler();
            await scheduler.Start();

            // define the job and tie it to our HelloJob class
            IJobDetail job = JobBuilder.Create<GetLinkOSPrintersToQuartz>()
                .WithIdentity("myJob", "group1")
                .Build();
            //每个15秒
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger3", "group1")
                .WithCronSchedule("0/15 * * * * ?")
                .ForJob("myJob", "group1")
                .Build();
            await scheduler.ScheduleJob(job, trigger);
        }
    }
}
