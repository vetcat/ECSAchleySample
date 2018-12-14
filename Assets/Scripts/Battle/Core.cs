using System;
using anygames.ashley.core;
using Battle.Factories;
using Battle.Systems;
using Battle.Views;
using UniRx;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Battle
{
    public class Core : IInitializable, IDisposable
    {
        private Engine _engine;
        private IDisposable _disposable = Disposable.Empty;
        private TankView.Pool _poolTank;
        private PlayerControlSystem _playerControlSystem;

        public Core(TankView.Pool poolTank, PlayerControlSystem playerControlSystem)
        {
            _playerControlSystem = playerControlSystem;
            _poolTank = poolTank;
        }

        public void Initialize()
        {
            InitCore();
            
            //TankFactory.Create(_poolTank.Spawn(Vector3.zero), _engine);

            CreateSeveralControlledItem(5, _poolTank);
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }

        private void InitCore()
        {
            _disposable = Observable.EveryUpdate().Subscribe(_ => _engine.update(Time.deltaTime));
            
            _engine = new Engine();
            _engine.addSystem(_playerControlSystem);            
        }

        private void CreateSeveralControlledItem(int count, TankView.Pool pool)
        {
            for (var i = 1; i <= count; i++)
                TankFactory.Create(pool.Spawn(new Vector3(Random.Range(0f,5f), 0.5f, Random.Range(0f,5f))), _engine);
        }
    }
}