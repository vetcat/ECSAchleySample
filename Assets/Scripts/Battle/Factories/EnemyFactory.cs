using System;
using anygames.ashley.core;
using Battle.Components;
using Battle.Enums;
using Battle.Signals;
using Battle.Views;
using UniRx;
using UnityEngine;
using Zenject;

namespace Battle.Factories
{
    public class EnemyFactory : IInitializable, IDisposable
    {        
        //todo : вынести в сеттинги
        private const int PollCount = 20;

        private Engine _engine;
        private SignalBus _signalBus;
        private DiContainer _container;
        readonly CompositeDisposable _disposables = new CompositeDisposable();
        private EnemyView.Pool _pool;
        
        private const float MovementSpeed = 5f;
        private const float RotationSpeed = 180f;
        private const int StartHealth = 100;
        private const int StartArmor = 50;

        public EnemyFactory(Engine engine, SignalBus signalBus, DiContainer container)
        {
            _container = container;
            _signalBus = signalBus;
            _engine = engine;
            
            container.BindMemoryPool<EnemyView, EnemyView.Pool>()
                .WithInitialSize(PollCount)
                .FromComponentInNewPrefabResource("EnemyView_One")
                .UnderTransformGroup("Pool_EnemyView_One");                        
            
            _pool = container.Resolve<EnemyView.Pool>();                                    
        }

        public void Initialize()
        {
            _signalBus.GetStream<SignalEnemySpawn>()
                .Subscribe(x=>Create(x.Type, x.Position, x.Forward)).AddTo(_disposables);            
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        public Entity Create(EEnemyType type, Vector3 position, Vector3 forward)
        {
            var view = _pool.Spawn(position, forward);                        
            var entity = CreateEntity(view);                     
                        
            return entity;            
        }
        
        private Entity CreateEntity(EnemyView view)
        {
            var entity = _engine.createEntity();
            
            var movementComponent = _engine.createComponent<MovementComponent>();
            movementComponent.SetSpeed(MovementSpeed);            
            entity.add(movementComponent);
                        
            var rotationComponent = _engine.createComponent<RotationComponent>();
            rotationComponent.Speed = RotationSpeed;            
            entity.add(rotationComponent);
            
            var healthComponent = _engine.createComponent<HealthComponent>();
            healthComponent.SetHealth(StartHealth);
            entity.add(healthComponent);
            
            var armorComponent = _engine.createComponent<ArmorComponent>();
            armorComponent.SetArmor(StartArmor);
            entity.add(armorComponent);
            
            var viewComponent = _engine.createComponent<EnemyViewComponent>();
            viewComponent.View = view;
            entity.add(viewComponent);
            
            _engine.addEntity(entity);
            return entity;
        }
    }
}