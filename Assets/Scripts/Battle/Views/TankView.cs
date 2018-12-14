using Uniject;
using Uniject.Impl;
using UnityEngine;
using Zenject;

namespace Battle.Views
{    
    public class TankView : TestableComponent, ITankView
    {        
        public ITransform Transform => Obj.Transform;

        private readonly UnityGameObject _gameObject;
        
        public TankView(ITestableGameObject obj) : base(obj)
        {
            _gameObject = obj as UnityGameObject;            
        }
        
        public class Pool : MemoryPool<Vector3, TankView>
        {         
            protected override void Reinitialize(Vector3 position, TankView item)
            {
                item._gameObject.SetActive(true);
                item.Transform.Position = position;
            }

            protected override void OnCreated(TankView item)
            {
                item._gameObject.SetActive(false);
            }

            protected override void OnDespawned(TankView item)
            {
                item._gameObject.SetActive(false);
            }
        }
    }
}