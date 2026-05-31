using Cysharp.Threading.Tasks;
using System;

namespace Game.Runtime.Core.Connections
{
    public interface IConnectionService
    {
        bool IsConnected { get; }
        event Action<bool> OnConnectionChanged;

        void Init();
    }

}