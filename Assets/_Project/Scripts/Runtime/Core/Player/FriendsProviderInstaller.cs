using Zenject;

namespace Game.Runtime.Core.Player
{
    public class FriendsProviderInstaller : MonoInstaller
	{
        public override void InstallBindings()
        {
			Container
				.Bind<FriendsProvider>()
				.AsSingle()
				.NonLazy();
        }
	}

}