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

public class PresenterPlayerInfoTest
{
    private Engine _engine;
    private DiContainer _container;             
    private SignalBus _signalBus;
    private ViewUiPlayerInfo _viewUi;
    private PresenterPlayerInfo _presenter;
    private PlayerFactory _playerFactory;
    private IInputController _inputController;
    private Entity _entity;
    
    private const string healthUiPrefix = "Health : ";
    private const string armorUiPrefix = "Armor : ";
    private const string speedUiPrefix = "Speed : ";

    [SetUp]
    public void SetUp()
    {        
        _container = new DiContainer();
        SignalBusInstaller.Install(_container);
        //signals
        _container.DeclareSignal<SignalPlayerCreated>();
        
        _inputController = Substitute.For<IInputController>();
        _container.Bind<IInputController>().FromInstance(_inputController).AsSingle().NonLazy();
        
        _signalBus = _container.Resolve<SignalBus>();                
        _container.BindInterfacesAndSelfTo<Engine>().AsSingle().NonLazy();        
        
        _engine = _container.Resolve<Engine>();    
        
        _container.BindInterfacesAndSelfTo<PlayerFactory>().AsSingle().NonLazy();        
        _container.BindInterfacesAndSelfTo<WeaponsController>().AsSingle().NonLazy();
        
        _playerFactory = _container.Resolve<PlayerFactory>(); 
        
        //ui        
        var viewResource = _container.InstantiatePrefabResourceForComponent<ViewUiPlayerInfo>("BattleViewsUi/ViewUiPlayerInfo");
        _container.BindViewController<ViewUiPlayerInfo, PresenterPlayerInfo>(viewResource, null);
        _viewUi = _container.Resolve<ViewUiPlayerInfo>();
        _presenter = _container.Resolve<PresenterPlayerInfo>();
        _presenter.Initialize();
        
        _entity = _playerFactory.Create(Vector3.zero, Vector3.forward);
    }

    [Test]
    public void OnPlayerCreated_HealthValue()
    {                                          
        Assert.AreEqual(_viewUi.TextHealth.text, healthUiPrefix + _entity.getComponent<HealthComponent>().Health);        
    }
    
    [Test]
    public void Health_EntityChangeValue()
    {                                          
        var healthComponent = _entity.getComponent<HealthComponent>();         
        const int damage = 10;        
        healthComponent.Damage(damage);
        Assert.AreEqual(_viewUi.TextHealth.text, healthUiPrefix + (healthComponent.HealthStartValue - damage));
    }
    
    [Test]
    public void OnPlayerCreated_ArmorValue()
    {        
        Assert.AreEqual(_viewUi.TextArmor.text, armorUiPrefix + _entity.getComponent<ArmorComponent>().Armor);
    }
    
    [Test]
    public void Armor_EntityChangeValue()
    {                                          
        var armorComponent = _entity.getComponent<ArmorComponent>();         
        const int damage = 10;        
        armorComponent.Damage(damage);
        Assert.AreEqual(_viewUi.TextArmor.text, armorUiPrefix + (armorComponent.ArmorStartValue - damage));
    }
    
    [Test]
    public void OnPlayerCreated_SpeedValue()
    {                                                  
        Assert.AreEqual(_viewUi.TextSpeed.text, speedUiPrefix + _entity.getComponent<MovementComponent>().Speed);
    }                    
    
    [Test]
    public void Speed_EntityChangeValue()
    {                                          
        var movementComponent = _entity.getComponent<MovementComponent>();         
        const int newSpeed = 10;        
        movementComponent.ChangeSpeed(newSpeed);
        Assert.AreEqual(_viewUi.TextSpeed.text, speedUiPrefix + movementComponent.Speed);
    }    
}