using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Orchestrator.Infrastructure.Workers
{
    public class FastWorker : Worker
    {
        public override bool DoWork(CancellationToken token)
        {
            var random = new Random();
            var next = random.Next(1, 21);

            if (next % 20 != 0)
            {
                Trace.TraceInformation($"      Nested Worker {Id}: Did Work");
            }
            else
            {
                int x = 0, y = 1;
                int z = y / x;
            }

            return true;
        }
    }
}
