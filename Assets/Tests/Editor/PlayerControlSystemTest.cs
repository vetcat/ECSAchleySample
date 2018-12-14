using anygames.ashley.core;
using Battle.Factories;
using Battle.InputControllers;
using Battle.Systems;
using Battle.Views;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using Zenject;

public class PlayerControlSystemTest
{
    private Engine _engine;
    private DiContainer _container;
    private IInputController _inputController;
    
    [SetUp]
    public void SetUp()
    {
        _engine = new Engine();
        _container = new DiContainer();
        _container.BindInterfacesAndSelfTo<StandardInputController>().AsSingle();
        _container.BindInterfacesAndSelfTo<PlayerControlSystem>().AsSingle().NonLazy();
        
        var system = _container.Resolve<PlayerControlSystem>();
        _engine.addSystem(system);
                
        _inputController = Substitute.For<IInputController>();            
    }

    [Test]
    public void Test_1()
    {
        var view = Substitute.For<ITankView>();
        //view.Transform.Position.Returns(new Vector3(0, 0, 0));
        
        //TankFactory.Create(view, _engine);        
        //_engine.update(1f);
        Assert.AreEqual(view.Transform.Position, new Vector3(0, 0, 0));
    }
}
