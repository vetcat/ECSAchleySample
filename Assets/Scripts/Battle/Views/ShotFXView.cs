using UnityEngine;
using Zenject;
using Component = anygames.ashley.core.Component;

namespace Battle.Views
{    
    public class ShotFXView : MonoBehaviour, Component
    {        
        public float LifeTime = 0.5f;
        [HideInInspector] 
        public float Elapsed;
        
        public class Pool : MonoMemoryPool<Vector3, Vector3, ShotFXView>
        {
            protected override void Reinitialize(Vector3 position, Vector3 forward, ShotFXView item)
            {
                item.transform.position = position;
                item.transform.forward = forward;
                item.Elapsed = 0f;
            }
        }
    }
}