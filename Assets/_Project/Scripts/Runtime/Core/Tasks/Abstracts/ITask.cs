using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Runtime.Core.Tasks
{
	public interface ITask : ITaskInfo
    {
        UniTask Execute();
    }

    public interface ITask<T> : ITaskInfo
    {
		UniTask<T> Execute();
    }
}
