using System;
using anygames.ashley.core;
using Battle.Factories;
using Battle.Signals;
using Battle.Systems;
using NUnit.Framework;
using UnityEngine;
using Zenject;

public class ShotFXSystemTest
{
    private Engine _engine;
    private DiContainer _container;             
    private SignalBus _signalBus;    
    private ShotFXFactory _shotFxFactory;

    [SetUp]
    public void SetUp()
    {        
        _container = new DiContainer();
        SignalBusInstaller.Install(_container);
        _container.DeclareSignal<SignalShotFXDestroy>();
        _signalBus = _container.Resolve<SignalBus>();
                
        _container.BindInterfacesAndSelfTo<ShotsFXSystem>().AsSingle().NonLazy();
        _container.BindInterfacesAndSelfTo<ShotFXFactory>().AsSingle().NonLazy();
        _container.BindInterfacesAndSelfTo<Engine>().AsSingle().NonLazy();           
        
        _engine = _container.Resolve<Engine>();                                

        _shotFxFactory = _container.Resolve<ShotFXFactory>();        
    }

    [Test]
    public void Destroy_LifeTime()
    {        
        var shotFxView = _shotFxFactory.Create(Vector3.zero, Vector3.forward);

        var deltaTime = 0.6f;
        shotFxView.LifeTime = 1f;
        
        bool received = false;
        Action callback = () => received = true;
        _signalBus.Subscribe<SignalShotFXDestroy>(callback);
        //first frame deltaTime < LifeTime 
        _engine.update(deltaTime);
        Assert.AreEqual(received, false);
        //second frame deltaTime * 2 > LifeTime : need destroy
        _engine.update(deltaTime);
        Assert.AreEqual(received, true);   
        
        _shotFxFactory.Destroy(shotFxView);
    }                    
}