using anygames.ashley.core;
using UnityEngine;
using Zenject;
using Component = anygames.ashley.core.Component;

namespace Battle.Views
{    
    public class ShotOneView : MonoBehaviour, Component
    {
        public float Speed = 10f;
        public float LifeTime = 2f;
        public int Damage = 20;
        [HideInInspector] 
        public float Elapsed;
        
        public class Pool : MonoMemoryPool<Vector3, Vector3, ShotOneView>
        {
            protected override void Reinitialize(Vector3 position, Vector3 forward, ShotOneView item)
            {
                item.transform.position = position;
                item.transform.forward = forward;
                item.Elapsed = 0f;
            }
        }
    }
}