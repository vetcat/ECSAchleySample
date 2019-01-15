using System;
using System.Collections;
using anygames.ashley.core;
using Battle.Controllers;
using Battle.Enums;
using Battle.Factories;
using Battle.Signals;
using Battle.Systems;
using Battle.Views;
using UniRx;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Battle
{
    public class CoreClient : IInitializable, IDisposable
    {
        private IDisposable _disposable = Disposable.Empty;                
        private readonly Engine _engine;
        private PlayerFactory _playerFactory;
        private EnemyFactory _enemyFactory;

        public CoreClient(Engine engine, PlayerFactory playerFactory, EnemyFactory enemyFactory)
        {
            _enemyFactory = enemyFactory;
            _playerFactory = playerFactory;
            _engine = engine;
        }

        public void Initialize()
        {
            InitCore();
            //создать игрока в следующем кадре после инициализации зенджектом всех кто реализует IInitializable
            MainThreadDispatcher.StartUpdateMicroCoroutine(CreatePlayer());                     
            MainThreadDispatcher.StartUpdateMicroCoroutine(CreateEnemy(EEnemyType.One));
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }

        private void InitCore()
        {
            _disposable = Observable.EveryUpdate().Subscribe(_ => _engine.update(Time.deltaTime));
        }
        
        private IEnumerator CreatePlayer()
        {
            yield return null;
            _playerFactory.Create(Vector3.zero, Vector3.zero);
        }
        
        private IEnumerator CreateEnemy(EEnemyType type)
        {
            yield return null;
            _enemyFactory.Create(type, Vector3.zero, Vector3.zero);
        }
    }
}