using demo.Properties;
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
            
            string getTime = Settings.Default.odoAdminTime;
            ITrigger trigger;
            if (getTime == "")
            {
                //默认每天每小时的00分00秒
                trigger = TriggerBuilder.Create()
                    .WithIdentity("trigger3", "group1")
                    .WithCronSchedule("0 0 * * * ?")
                    .ForJob("myJob", "group1")
                    .Build();
            }
            else
            {
                trigger = TriggerBuilder.Create()
                .WithIdentity("trigger3", "group1")
                .WithCronSchedule(getTime)
                .ForJob("myJob", "group1")
                .Build();
            }
            
            await scheduler.ScheduleJob(job, trigger);
        }

        //收到来自里程设置的时间并计算后转化成字符串，每次调用需要重启软件才能生效
        public static string GetTime()
        {
            return "";
        }
    }
}
