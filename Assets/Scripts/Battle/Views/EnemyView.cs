using Battle.Enums;
using UnityEngine;
using Zenject;

namespace Battle.Views
{    
    public class EnemyView : MonoBehaviour
    {                
        public EEnemyType Type;
        
        public class Pool : MonoMemoryPool<Vector3, Vector3, EnemyView>
        {         
            protected override void Reinitialize(Vector3 position, Vector3 forward, EnemyView item)
            {
                item.transform.position = position;
                item.transform.forward = forward;
            }
        }
    }
}