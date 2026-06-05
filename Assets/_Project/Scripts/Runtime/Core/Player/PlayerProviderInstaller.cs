using Zenject;

namespace Game.Runtime.Core.Player
{
    public class PlayerProviderInstaller : MonoInstaller
	{
        public override void InstallBindings()
        {
			Container
				.Bind<PlayerProvider>()
				.AsSingle()
				.NonLazy()
				;
        }
	}

}