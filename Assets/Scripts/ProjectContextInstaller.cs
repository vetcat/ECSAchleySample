using Zenject;

public class ProjectContextInstaller : MonoInstaller
{
    public override void InstallBindings()
    {        
        SignalBusInstaller.Install(Container);
    }
}