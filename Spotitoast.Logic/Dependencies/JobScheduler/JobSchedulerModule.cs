using Job.Scheduler.Builder;
using Job.Scheduler.Scheduler;
using Ninject.Modules;

namespace Spotitoast.Logic.Dependencies.JobScheduler
{
    public class JobSchedulerModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IJobScheduler>().To<Job.Scheduler.Scheduler.JobScheduler>().InSingletonScope();
            Bind<JobRunnerBuilder>().ToSelf().InSingletonScope();
        }
    }
}