using Zenject;

public class SystemsInstaller : MonoInstaller<SystemsInstaller>
{
    public override void InstallBindings()
    {
        Container.Bind<ISecurityService>().To<SecurityService>().AsSingle();
        Container.BindInterfacesAndSelfTo<NodesController>().AsSingle().NonLazy();

        Container.Bind<NodeInteractionHandler>().FromComponentInHierarchy(true).AsSingle().NonLazy();
    }
}
