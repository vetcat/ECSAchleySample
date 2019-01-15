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
    public class ShotFactory : IInitializable, IDisposable
    {
        //todo : вынести в сеттинги
        private const int PollCount = 50;

        private readonly ShotOneView.Pool _pool;
        private readonly Engine _engine;
        private readonly SignalBus _signalBus;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();        
        private Dictionary<ShotOneView, Entity> _entities = new Dictionary<ShotOneView, Entity>();
        
        public ShotFactory(Engine engine, SignalBus signalBus, DiContainer container)
        {            
            _signalBus = signalBus;
            _engine = engine;
            
            container.BindMemoryPool<ShotOneView, ShotOneView.Pool>()
                .WithInitialSize(PollCount)
                .FromComponentInNewPrefabResource("ShotEntity")
                .UnderTransformGroup("Pool_ShotEntity");            
            
            _pool = container.Resolve<ShotOneView.Pool>();
        }
        
        public void Initialize()
        {            
            _signalBus.GetStream<SignalShotSpawn>()
                .Subscribe(x=>Create(x.Position, x.Forward)).AddTo(_disposables);

            _signalBus.GetStream<SignalShotDestroy>()
                .Subscribe(x=>Destroy(x.OneView)).AddTo(_disposables);            
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
                
        public ShotOneView Create(Vector3 position, Vector3 forward)
        {
            //Debug.Log("[ShotFactory] Create");
            return CreateEntity(_pool.Spawn(position, forward));                        
        }

        public void Destroy(ShotOneView oneView)
        {
            //Debug.Log("[ShotFactory] Destroy");            
            _pool.Despawn(oneView);
            _engine.removeEntity(_entities[oneView]);
            _entities.Remove(oneView);
        }

        private ShotOneView CreateEntity(ShotOneView oneView)
        {
            var entity = _engine.createEntity();
            entity.add(oneView);                                                                                                
            entity.add(_engine.createComponent<ShotComponent>());            
            _engine.addEntity(entity);
            _entities.Add(oneView, entity);
            return oneView;
        }
    }
}