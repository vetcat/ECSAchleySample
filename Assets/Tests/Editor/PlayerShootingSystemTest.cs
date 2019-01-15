using System;
using anygames.ashley.core;
using Battle.Components;
using Battle.Controllers;
using Battle.Controllers.InputControllers;
using Battle.Factories;
using Battle.Signals;
using Battle.Systems;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using Zenject;

public class PlayerShootingSystemTest
{
    private Engine _engine;
    private DiContainer _container; 
    private IInputController _inputController;    
    private PlayerShootingSystem _playerShootingSystem;
    private SignalBus _signalBus;
    private Entity _playerEntity;

    [SetUp]
    public void SetUp()
    {        
        _container = new DiContainer();
        SignalBusInstaller.Install(_container);
        _container.DeclareSignal<SignalShotSpawn>();
        _container.DeclareSignal<SignalPlayerCreated>();
        
        _signalBus = _container.Resolve<SignalBus>();
        
        _inputController = Substitute.For<IInputController>();
        _container.Bind<IInputController>().FromInstance(_inputController).AsSingle().NonLazy();
        
        _container.BindInterfacesAndSelfTo<PlayerShootingSystem>().AsSingle().NonLazy();
        _container.BindInterfacesAndSelfTo<WeaponsController>().AsSingle().NonLazy();        
        _container.BindInterfacesAndSelfTo<PlayerFactory>().AsSingle().NonLazy();        
        _container.BindInterfacesAndSelfTo<HealthComponent>().AsSingle().NonLazy();
        _container.BindInterfacesAndSelfTo<Engine>().AsSingle().NonLazy();           
        
        _engine = _container.Resolve<Engine>();
        _playerShootingSystem = _container.Resolve<PlayerShootingSystem>();                        

        var playerFactory = _container.Resolve<PlayerFactory>();     
        _playerEntity = playerFactory.Create(Vector3.zero, Vector3.forward);
    }

    [Test]
    public void ShotFireProcess_From_InputController_Fire()
    {
        var deltaTime = 0.5f; 

        _inputController.IsFireProcess().Returns(true);        
        _engine.update(deltaTime);
        Assert.AreEqual(_playerShootingSystem.IsFireProcess, true);
    }
    
    [Test]
    public void ShotFireProcess_From_InputController_NoFire()
    {
        var deltaTime = 0.5f; 

        _inputController.IsFireProcess().Returns(false);        
        _engine.update(deltaTime);
        Assert.AreEqual(_playerShootingSystem.IsFireProcess, false);
    }
    
    [Test]
    public void FireRate_SignalPlayerShot()
    {                
        bool received = false;
        Action callback = () => received = true;
        _signalBus.Subscribe<SignalShotSpawn>(callback);
        
        _playerEntity.getComponent<WeaponsComponent>().CurrentWeaponView.FireRate= 1f; 
        var deltaTime = 0.6f; 

        _inputController.IsFireProcess().Returns(true);
        
        //first frame deltaTime < FireRate
        _engine.update(deltaTime);        
        Assert.AreEqual(received, false);
        
        //second frame deltaTime*2 > FireRate
        received = false;
        _engine.update(deltaTime);                        
        Assert.AreEqual(received, true);
       
        //third frame deltaTime*3 < FireRate*2
        received = false;
        _engine.update(deltaTime);        
        Assert.AreEqual(received, false);
    }  
}