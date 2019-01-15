using System;
using anygames.ashley.core;
using Battle.Factories;
using Battle.Signals;
using Battle.Systems;
using NUnit.Framework;
using UnityEngine;
using Zenject;

public class ShotsMoveSystemTest
{
    private Engine _engine;
    private DiContainer _container;         
    private ShotsMoveSystem _shotsMoveSystem;
    private SignalBus _signalBus;    
    private ShotFactory _shotFactory;

    [SetUp]
    public void SetUp()
    {        
        _container = new DiContainer();
        SignalBusInstaller.Install(_container);
        _container.DeclareSignal<SignalShotDestroy>();
        _signalBus = _container.Resolve<SignalBus>();
                
        _container.BindInterfacesAndSelfTo<ShotsMoveSystem>().AsSingle().NonLazy();
        _container.BindInterfacesAndSelfTo<ShotFactory>().AsSingle().NonLazy();
        _container.BindInterfacesAndSelfTo<Engine>().AsSingle().NonLazy();           
        
        _engine = _container.Resolve<Engine>();
        _shotsMoveSystem = _container.Resolve<ShotsMoveSystem>();                        

        _shotFactory = _container.Resolve<ShotFactory>();        
    }

    [Test]
    public void Destroy_LifeTime()
    {
        var spawnPosition = -Vector3.up * 1000f;
        var shotView = _shotFactory.Create(spawnPosition, Vector3.forward);

        var deltaTime = 0.6f;
        shotView.LifeTime = 1f;
        
        bool received = false;
        Action callback = () => received = true;
        _signalBus.Subscribe<SignalShotDestroy>(callback);
        //first frame deltaTime < LifeTime 
        _engine.update(deltaTime);
        Assert.AreEqual(received, false);
        //second frame deltaTime * 2 > LifeTime : need destroy
        _engine.update(deltaTime);
        Assert.AreEqual(received, true);   
        
        _shotFactory.Destroy(shotView);
    }             
    
    [Test]
    public void Check_Speed()
    {
        var spawnPosition = -Vector3.up * 1000f;
        var shotView = _shotFactory.Create(spawnPosition, Vector3.forward);
                
        shotView.LifeTime = 5f;
        shotView.Speed = 1.75f;        
            
        var deltaTime = 0.35f;
        
        var velocity = shotView.transform.forward * shotView.Speed * deltaTime;
        var nextPosition = shotView.transform.position + velocity;

        _engine.update(deltaTime);
        Assert.AreEqual(shotView.transform.position, nextPosition);
    }
}