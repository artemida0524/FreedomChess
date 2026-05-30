using Game.Core.Player;
using Game.Runtime.Core.Auth;
using Zenject;

public class PlayerAuthRepositoryInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container
            .BindInterfacesAndSelfTo<PlayerAuthRepositoryFirebase>()
            .AsSingle()
            .NonLazy()
            ;
    }
}
