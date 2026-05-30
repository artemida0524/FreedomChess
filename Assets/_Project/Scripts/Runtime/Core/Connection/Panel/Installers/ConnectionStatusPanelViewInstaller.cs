using UnityEngine;
using Zenject;
namespace Game.Runtime.Core.Connections.Installers
{

	public class ConnectionStatusPanelViewInstaller : MonoInstaller
	{
        [SerializeField] private ConnectionStatusPanelView instance;

        public override void InstallBindings()
        {

            Container
                .BindInterfacesAndSelfTo<ConnectionStatusPanelView>()
                .FromInstance(instance)
                .AsSingle()
                ;
        }
	} 
}
