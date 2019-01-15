using anygames.ashley.core;
using Battle.Components;
using Battle.Controllers;
using Battle.Controllers.InputControllers;
using Battle.Factories;
using Battle.Signals;
using Battle.ViewsUi;
using NSubstitute;
using NUnit.Framework;
using UiCore;
using UnityEngine;
using Zenject;

public class PresenterWeaponsTest
{
    private Engine _engine;
    private DiContainer _container;             
    private SignalBus _signalBus;
    private ViewUiWeapons _viewUi;
    private PresenterWeapons _presenter;
    private PlayerFactory _playerFactory;
    private IInputController _inputController;

    [SetUp]
    public void SetUp()
    {        
        _container = new DiContainer();
        SignalBusInstaller.Install(_container);
        //signals
        _container.DeclareSignal<SignalPlayerCreated>();
        _container.DeclareSignal<SignalShotSpawn>();
        _container.DeclareSignal<SignalWeaponSwitch>();      
        
        _inputController = Substitute.For<IInputController>();
        _container.Bind<IInputController>().FromInstance(_inputController).AsSingle().NonLazy();
        
        _signalBus = _container.Resolve<SignalBus>();                
        _container.BindInterfacesAndSelfTo<Engine>().AsSingle().NonLazy();        
        
        _engine = _container.Resolve<Engine>();    
        
        _container.BindInterfacesAndSelfTo<PlayerFactory>().AsSingle().NonLazy();
        //_container.BindInterfacesAndSelfTo<HealthComponent>().AsSingle().NonLazy();
        _container.BindInterfacesAndSelfTo<WeaponsController>().AsSingle().NonLazy();
        
        _playerFactory = _container.Resolve<PlayerFactory>(); 
        
        //ui
        var canvas = new GameObject();
        var viewResource = _container.InstantiatePrefabResourceForComponent<ViewUiWeapons>("BattleViewsUi/ViewUiWeapons");
        _container.BindViewController<ViewUiWeapons, PresenterWeapons>(viewResource, canvas.transform);
        _viewUi = _container.Resolve<ViewUiWeapons>();
        _presenter = _container.Resolve<PresenterWeapons>();
        _presenter.Initialize();
    }

    [Test]
    public void CreatePlayer_AddWeaponItems()
    {        
        _playerFactory.Create(Vector3.zero, Vector3.forward);
                
        var items = _viewUi.CollectionWeaponItem.GetItems();
        Assert.Greater(items.Count, 0);
    }                    
}