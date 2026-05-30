using Zenject;

namespace Game.Runtime.Core.Tasks.Installers
{
    public class TaskFactoryInstaller : MonoInstaller
    {
        override public void InstallBindings()
        {
            Container
                .BindInterfacesAndSelfTo<TaskFactory>()
                .AsSingle()
                .NonLazy();
        }
    } 
}