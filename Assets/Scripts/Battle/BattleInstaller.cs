using anygames.ashley.core;
using Battle.Components;
using Battle.Controllers;
using Battle.Controllers.InputControllers;
using Battle.Factories;
using Battle.Signals;
using Battle.Systems;
using Zenject;

namespace Battle
{
    public class BattleInstaller : MonoInstaller
    {        
        public override void InstallBindings()
        {                        
            //engine 
            Container.BindInterfacesAndSelfTo<Engine>().AsSingle().NonLazy();
            
            //models
            Container.BindInterfacesAndSelfTo<HealthComponent>().AsSingle().NonLazy();
                        
            //core
            Container.BindInterfacesAndSelfTo<CoreClient>().AsSingle().NonLazy();
            
            //controllers
            Container.BindInterfacesAndSelfTo<StandardInputController>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<WeaponsController>().AsSingle().NonLazy();

            //systems            
            Container.BindInterfacesAndSelfTo<PlayerMovementSystem>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<PlayerRotationtSystem>().AsSingle().NonLazy();            
            Container.BindInterfacesAndSelfTo<PlayerShootingSystem>().AsSingle().NonLazy();            
            
            Container.BindInterfacesAndSelfTo<ShotsMoveSystem>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<ShotsFXSystem>().AsSingle().NonLazy();            
                        
            //factories
            Container.BindInterfacesAndSelfTo<PlayerFactory>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<EnemyFactory>().AsSingle().NonLazy();            
            Container.BindInterfacesAndSelfTo<ShotFactory>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<ShotFXFactory>().AsSingle().NonLazy();                                    
                                    
            //signals
            Container.DeclareSignal<SignalPlayerSpawn>();
            Container.DeclareSignal<SignalPlayerCreated>();                            
            Container.DeclareSignal<SignalShotSpawn>();                    
            Container.DeclareSignal<SignalShotDestroy>();
            Container.DeclareSignal<SignalShotFXSpawn>();
            Container.DeclareSignal<SignalShotFXDestroy>();
            Container.DeclareSignal<SignalWeaponSwitch>();    
            Container.DeclareSignal<SignalEnemySpawn>();            
        }
    }        
}
