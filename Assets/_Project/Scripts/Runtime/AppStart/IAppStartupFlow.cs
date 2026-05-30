using Cysharp.Threading.Tasks;
using Game.Runtime.Core.Tasks;
using System;

namespace Game.Runtime.AppStart.StartupFlow
{
    public interface IAppStartupFlow
    {
        event Action<ITaskInfo> OnTaskStarted;
        event Action<float> OnProgressChanged;
        void Init();
        UniTask RunAsync();
    }
}
