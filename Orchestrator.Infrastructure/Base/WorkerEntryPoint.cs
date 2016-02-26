using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Orchestrator.Infrastructure
{
    public class WorkerEntryPoint : IWorkerEntryPoint
    {
        public WorkerEntryPoint(string id, IWorker worker, int iterationDelay)
        {
            if(worker == null)
                throw new ArgumentException("worker");

            Name = GetType().Name;
            Id = id;
            IterationDelay = iterationDelay;
            Worker = worker;
        }

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly EventWaitHandle runWorkerEntryPoint = new ManualResetEvent(false);

        public string Name { get; private set; }
        public string Id { get; private set; }
        public IWorker Worker{ get; private set; }
        public int IterationDelay { get; private set; }

        public virtual bool OnStart()
        {
            Trace.TraceInformation($"   Worker Entry Point {Name}-{Id}: has been started");

            return true;
        }

        public virtual async Task RunAsync(CancellationToken cancelationToken)
        {
            while (!cancelationToken.IsCancellationRequested)
            {
                try
                {
                    Trace.TraceInformation($"   Worker Entry Point {Name}-{Id}: working");
                    Worker.DoWork(cancelationToken);
                }
                catch (Exception ex)
                {
                    Trace.TraceError($"   Worker Entry Point {Name}-{Id}: Error: {ex}");
                }

                await Task.Delay(IterationDelay, cancelationToken);
            }
        }

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
            Trace.TraceInformation($"   Worker Entry Point {Name}-{Id}: is stopping");

            cancellationTokenSource.Cancel();
            runWorkerEntryPoint.WaitOne();

            Trace.TraceInformation($"   Worker Entry Point {Name}-{Id}: has stopped");
        }
    }
}
