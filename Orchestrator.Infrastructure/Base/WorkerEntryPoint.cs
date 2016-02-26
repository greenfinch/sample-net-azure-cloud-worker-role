using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Orchestrator.Infrastructure
{
    public abstract class WorkerEntryPoint : IWorker
    {
        public WorkerEntryPoint(string name)
        {
            Name = name;
        }

        protected readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private EventWaitHandle runWorkerEntryPoint = new ManualResetEvent(false);

        public string Name { get; private set; }

        public virtual bool OnStart()
        {
            Trace.TraceInformation($"   Nested Worker {Name}: has been started");

            return true;
        }

        public abstract Task RunAsync(CancellationToken token);

        public virtual void Run()
        {
            try
            {
                RunAsync(cancellationTokenSource.Token).Wait();
            }
            finally
            {
                runWorkerEntryPoint.Set();
            }
        }

        public virtual void OnStop()
        {
            Trace.TraceInformation($"   Nested Worker {Name}: is stopping");

            cancellationTokenSource.Cancel();

            runWorkerEntryPoint.WaitOne();

            Trace.TraceInformation($"   Nested Worker {Name}: has stopped");
        }
    }
}
