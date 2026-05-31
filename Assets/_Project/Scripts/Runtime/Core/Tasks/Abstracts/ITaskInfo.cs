using System;

namespace Game.Runtime.Core.Tasks
{
    public interface ITaskInfo
    {
        string Name { get; }
        float Progress { get; }	// 0 ... 1
        event Action<float> OnProgressChanged;
    }
}
