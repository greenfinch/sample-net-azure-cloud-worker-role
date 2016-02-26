using Autofac;
using Orchestrator.Infrastructure.Workers;

namespace Orchestrator.Infrastructure
{
    public static class AutoFacBootstrapper
    {
        public static IContainer Setup()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<FastWorker>().Keyed<IWorker>(WorkerTypes.Fast);
            builder.RegisterType<SlowWorker>().Keyed<IWorker>(WorkerTypes.Slow);


            builder.RegisterType<WorkerEntryPoint>()
                .Keyed<IWorkerEntryPoint>(WorkerTypes.Fast)
                .WithParameter("iterationDelay",3000)
                .WithParameter((p, c) => p.ParameterType == typeof(IWorker),
                               (p, c) => c.ResolveKeyed<IWorker>(WorkerTypes.Fast));

            builder.RegisterType<WorkerEntryPoint>()
                .Keyed<IWorkerEntryPoint>(WorkerTypes.Slow)
                .WithParameter("iterationDelay", 10000)
                .WithParameter((p, c) => p.ParameterType == typeof(IWorker),
                                (p, c) => c.ResolveKeyed<IWorker>(WorkerTypes.Slow));

            return builder.Build();
        }
    }
}
