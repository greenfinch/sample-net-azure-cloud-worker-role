using Autofac;
using Orchestrator.Infrastructure.Workers;

namespace Orchestrator.Infrastructure
{
    public static class AutoFacBootstrapper
    {
        public static IContainer Setup()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<FastWorker>()
                .Named<IWorker>("Fast");

            builder.RegisterType<SlowWorker>()
                .Named<IWorker>("Slow");
            
            return builder.Build();
        }
    }
}
