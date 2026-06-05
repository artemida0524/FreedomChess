using UnityEngine;
using Zenject;

public class IconStatInstaller : MonoInstaller
{
    [SerializeField] private IconStatManager iconStatManager;
    public override void InstallBindings()
    {
        Container
            .Bind<IconStatManager>()
            .FromInstance(iconStatManager)
            .AsSingle();
    }
}