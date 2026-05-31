using Game.Core;

using Zenject;

namespace Game.Runtime.Core.Tasks
{

    public class TaskFactory : ITaskFactory
    {
        private DiContainer _diContainer;
        [Inject]
        private void Construct(DiContainer diContainer)
        {
            _diContainer = diContainer;
        }

        public T Create<T>()
        {
            return _diContainer.Instantiate<T>();
        }

        public T Create<T>(params object[] args)
        {
            return _diContainer.Instantiate<T>(args);
        }
    } 
}