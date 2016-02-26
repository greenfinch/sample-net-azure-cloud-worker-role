using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Orchestrator.Infrastructure.Workers
{
    public abstract class Worker : IWorker
    {
        public Guid Id { get; private set; }

        public Worker()
        {
            Id = Guid.NewGuid();
        }

        public abstract bool DoWork(CancellationToken cancelationToken);
    }
}
