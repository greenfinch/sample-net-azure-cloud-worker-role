using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using Orchestrator.Infrastructure;
using Autofac;

namespace Orchestrator
{
    public class OrchestratingWorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        private readonly List<Task> Tasks = new List<Task>();
        private readonly List<IWorkerEntryPoint> Workers = new List<IWorkerEntryPoint>();

        private IContainer container;

        private void DoSetup()
        {
            container = AutoFacBootstrapper.Setup();

            for (int i = 1; i <= 4; i++)
            {
                Workers.Add(
                    container.ResolveKeyed<IWorkerEntryPoint>(WorkerTypes.Fast,
                    new NamedParameter("id", i.ToString()))
                );
                Workers.Add(
                    container.ResolveKeyed<IWorkerEntryPoint>(WorkerTypes.Slow,
                    new NamedParameter("id", i.ToString()))
                );
            }
        }

        public override void Run()
        {
            Trace.TraceInformation("Orchestrator: is running");
            try
            {
                RunAsync(cancellationTokenSource.Token).Wait();
            }
            finally
            {
                runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            Trace.TraceInformation("Orchestrator: is starting");
            ServicePointManager.DefaultConnectionLimit = 12;
            DoSetup();
            Parallel.ForEach(Workers, (w) => w.OnStart());
            bool result = base.OnStart();
            Trace.TraceInformation("Orchestrator: has been started");
            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("Orchestrator: is stopping");
            Parallel.ForEach(Workers, (w) => w.OnStop());
            cancellationTokenSource.Cancel();
            runCompleteEvent.WaitOne();
            Trace.TraceInformation("Orchestrator: has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            foreach (var worker in Workers)
            {
                Tasks.Add(Task.Run(() => worker.Run()));
            }

            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Orchestrator: is working");
                await Task.Delay(15000);
            }
        }
    }
}
