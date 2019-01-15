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

public class PlayerMovementSystemTest
{
    private Engine _engine;
    private DiContainer _container; 
    private IInputController _inputController;    
    private PlayerMovementSystem _movementSystem;
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
        
        _container.BindInterfacesAndSelfTo<PlayerMovementSystem>().AsSingle().NonLazy();
        _container.BindInterfacesAndSelfTo<WeaponsController>().AsSingle().NonLazy();
        _container.BindInterfacesAndSelfTo<HealthComponent>().AsSingle().NonLazy();
        _container.BindInterfacesAndSelfTo<PlayerFactory>().AsSingle().NonLazy();        
        _container.BindInterfacesAndSelfTo<Engine>().AsSingle().NonLazy();
        
        _engine = _container.Resolve<Engine>();         
        _movementSystem = _container.Resolve<PlayerMovementSystem>();
                
        var playerFactory = _container.Resolve<PlayerFactory>();
        _playerEntity = playerFactory.Create(Vector3.zero, Vector3.forward);
    }

    /* при использовании CharacterController.SimpleMove рассчитать корректные финальные значения трансформации затруднительно
    тест актуален только для прямого управления через transform - вараинт перевести на CharacterController.Move 
    [Test]
    public void Move()
    {        
        _tankView.transform.position = Vector3.zero;                

        var deltaTime = 0.5f; 

        _inputController.GetVertical().Returns(1);        
        _engine.update(deltaTime);

        var finalPositonZ = _controlSystem.MovementSpeed * deltaTime;                
        Assert.AreEqual(_tankView.transform.position, new Vector3(0f, 0f, finalPositonZ));
    }
    */
}
