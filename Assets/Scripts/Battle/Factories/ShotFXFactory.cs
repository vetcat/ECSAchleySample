using System;
using System.Collections.Generic;
using System.Linq;
using anygames.ashley.core;
using Battle.Components;
using Battle.Signals;
using Battle.Views;
using UniRx;
using UnityEngine;
using Zenject;
using Entity = anygames.ashley.core.Entity;

namespace Battle.Factories
{
    public class ShotFXFactory : IInitializable, IDisposable
    {
        //todo : вынести в сеттинги
        private const int PollCount = 50;

        private readonly ShotFXView.Pool _pool;
        private readonly Engine _engine;
        private readonly SignalBus _signalBus;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();        
        private readonly Dictionary<ShotFXView, Entity> _entities = new Dictionary<ShotFXView, Entity>();
        
        public ShotFXFactory(Engine engine, SignalBus signalBus, DiContainer container)
        {            
            _signalBus = signalBus;
            _engine = engine;
            
            container.BindMemoryPool<ShotFXView, ShotFXView.Pool>()
                .WithInitialSize(PollCount)
                .FromComponentInNewPrefabResource("WFXMR_BImpact Concrete")
                .UnderTransformGroup("Pool_WFXMR_BImpact Concrete");            
            
            _pool = container.Resolve<ShotFXView.Pool>();
        }
        
        public void Initialize()
        {            
            _signalBus.GetStream<SignalShotFXSpawn>()
                .Subscribe(x=>Create(x.Position, x.Forward)).AddTo(_disposables);
            
            _signalBus.GetStream<SignalShotFXDestroy>()
                .Subscribe(x=>Destroy(x.View)).AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
                
        public ShotFXView Create(Vector3 position, Vector3 forward)
        {            
            return CreateEntity(_pool.Spawn(position, forward));                        
        }

        public void Destroy(ShotFXView view)
        {                              
            _pool.Despawn(view);
            _engine.removeEntity(_entities[view]);
            _entities.Remove(view);
        }

        private ShotFXView CreateEntity(ShotFXView view)
        {
            var entity = _engine.createEntity();
            entity.add(view);                                                                                                
            entity.add(_engine.createComponent<ShotFXComponent>());            
            _engine.addEntity(entity);
            _entities.Add(view, entity);
            return view;
        }
    }
}