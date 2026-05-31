using Game.Runtime.Core.Connections;
using Zenject;

public class ConnectionServiceInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container
            .BindInterfacesAndSelfTo<ConnectionServiceFirebase>()
            .AsSingle()
            .NonLazy()
            ;
    }
}
