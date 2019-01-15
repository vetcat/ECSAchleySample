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

public class PlayerRotationSystemTest
{
    private Engine _engine;
    private DiContainer _container; 
    private IInputController _inputController;        
    private SignalBus _signalBus;
    private Entity _playerEntity;
    
    [SetUp]
    public void SetUp()
    {        
        _container = new DiContainer();
        SignalBusInstaller.Install(_container);

        _container.DeclareSignal<SignalPlayerCreated>();
        
        _inputController = Substitute.For<IInputController>();
        _container.Bind<IInputController>().FromInstance(_inputController).AsSingle().NonLazy();
        
        _container.BindInterfacesAndSelfTo<PlayerRotationtSystem>().AsSingle().NonLazy();
        _container.BindInterfacesAndSelfTo<WeaponsController>().AsSingle().NonLazy();        
        _container.BindInterfacesAndSelfTo<PlayerFactory>().AsSingle().NonLazy();        
        _container.BindInterfacesAndSelfTo<Engine>().AsSingle().NonLazy();
        
        _engine = _container.Resolve<Engine>();                 
                
        var playerFactory = _container.Resolve<PlayerFactory>();
        _playerEntity = playerFactory.Create(Vector3.zero, Vector3.forward);
    }
    
    [Test]
    public void RotationAngle_FromInput()
    {
        var view = _playerEntity.getComponent<PlayerViewComponent>().View;
        var rotationComponent = _playerEntity.getComponent<RotationComponent>();
        view.transform.rotation = Quaternion.identity;

        var deltaTime = 0.5f; 

        _inputController.GetHorizontal().Returns(1);        
        _engine.update(deltaTime);

        var finalRotationY = rotationComponent.Speed * deltaTime;        
        Assert.AreEqual(view.transform.eulerAngles, new Vector3(0f, finalRotationY, 0f));                                
    }
}
