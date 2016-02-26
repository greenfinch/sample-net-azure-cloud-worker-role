using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Orchestrator.Infrastructure.Workers
{
    public class SlowWorker : WorkerEntryPoint
    {
        public SlowWorker(string name) : base(name) { }

        public override async Task RunAsync(CancellationToken cancelationToken)
        {
            Trace.TraceInformation($"   Nested Worker {Name}: running");

            while (!cancelationToken.IsCancellationRequested)
            {
                try
                {
                    var random = new Random();
                    var next = random.Next(1, 21);

                    if (next % 20 != 0)
                    {
                        Trace.TraceInformation($"   Nested Worker {Name}: Did Work");
                    }
                    else
                    {
                        int x = 0, y = 1;
                        int z = y / x;
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError($"   Nested Worker {Name}: Error: {ex}");
                }

                await Task.Delay(5000); //7 seconds cos I'm fast
            }
        }
    }
}
