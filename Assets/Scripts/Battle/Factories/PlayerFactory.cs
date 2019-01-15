using System;
using anygames.ashley.core;
using Battle.Components;
using Battle.Controllers;
using Battle.Enums;
using Battle.Signals;
using Battle.Systems;
using Battle.Views;
using UniRx;
using UnityEngine;
using Zenject;

namespace Battle.Factories
{
    public class PlayerFactory : IInitializable, IDisposable
    {
        public Entity PlayerEntity => _playerEntity;
        private Entity _playerEntity;
        
        //todo : вынести в сеттинги
        private const int PollCount = 20;

        private readonly PlayerView.Pool _pool;        
        private readonly Engine _engine;
        private readonly SignalBus _signalBus;
        readonly CompositeDisposable _disposables = new CompositeDisposable();
        private DiContainer _container;
        private WeaponsController _weaponsController;

        private const float MovementSpeed = 5f;
        private const float RotationSpeed = 180f;
        private const int StartHealth = 100;
        private const int StartArmor = 50;

        public PlayerFactory(Engine engine, SignalBus signalBus, DiContainer container, WeaponsController weaponsController)
        {            
            _weaponsController = weaponsController;
            _container = container;
            _signalBus = signalBus;
            _engine = engine;            
            container.BindMemoryPool<PlayerView, PlayerView.Pool>()
                .WithInitialSize(PollCount)
                .FromComponentInNewPrefabResource("PlayerEntity")
                .UnderTransformGroup("Pool_PlayerEntity");                        
            
            _pool = container.Resolve<PlayerView.Pool>();                                    
        }
        
        public void Initialize()
        {                                    
            _signalBus.GetStream<SignalPlayerSpawn>()
                .Subscribe(x=>Create(x.Position, x.Forward)).AddTo(_disposables);            
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }         
                
        public Entity Create(Vector3 position, Vector3 forward)
        {                        
            var view = _pool.Spawn(position, forward);                        
            var entity = CreateEntity(view);
            AddWeapons(entity);
            _playerEntity = entity;
            
            _signalBus.Fire(new SignalPlayerCreated(entity));
            return entity;
        }

        private Entity CreateEntity(PlayerView view)
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
            
            var playerViewComponent = _engine.createComponent<PlayerViewComponent>();
            playerViewComponent.View = view;
            entity.add(playerViewComponent);
            
            entity.add(_engine.createComponent<ShootingComponent>());            
            entity.add(_engine.createComponent<WeaponsComponent>());
            
            _engine.addEntity(entity);
            return entity;
        }

        private void AddWeapons(Entity entity)
        {
            var view = entity.getComponent<PlayerViewComponent>().View;            
            _weaponsController.AddWeapon(entity, CreateWeapon(EWeaponType.WeaponOne, view.MountPointWeaponOne));
            _weaponsController.AddWeapon(entity, CreateWeapon(EWeaponType.WeaponTwo, view.MountPointWeaponTwo));
            
            _weaponsController.SetCurrentWeapon(entity, EWeaponType.WeaponOne);
        }
        
        private WeaponView CreateWeapon(EWeaponType weaponType, Transform root)
        {            
            var weaponView = _container.InstantiatePrefabResourceForComponent<WeaponView>("Weapons/" + weaponType);
            weaponView.transform.parent = root;
            weaponView.transform.localPosition = Vector3.zero;
            weaponView.transform.localRotation = Quaternion.identity;
            return weaponView;
        }
    }
}