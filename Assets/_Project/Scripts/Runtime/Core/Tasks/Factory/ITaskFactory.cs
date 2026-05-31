namespace Game.Runtime.Core.Tasks
{
    public interface ITaskFactory
    {
        T Create<T>();
        T Create<T>(params object[] args);
    }
}