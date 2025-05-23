using Zenject;

public class SystemsInstaller : MonoInstaller<SystemsInstaller>
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<NodesController>().AsSingle().NonLazy();
        Container.Bind<ISecurityService>().To<SecurityService>().AsSingle();
    }
}
