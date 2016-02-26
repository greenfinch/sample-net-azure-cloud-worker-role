namespace Orchestrator.Infrastructure
{
    public interface IWorker
    {
        void Run();

        bool OnStart();

        void OnStop();

        string Name { get; }
    }
}
