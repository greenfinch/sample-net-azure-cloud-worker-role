using System;
using System.Threading;

namespace Orchestrator.Infrastructure
{
    public interface IWorker
    {
        Guid Id { get; }

        bool DoWork(CancellationToken cancelationToken);
    }
    
    public interface IWorkerEntryPoint
    {
        void Run();

        bool OnStart();

        void OnStop();

        string Name { get; }
    }
}
