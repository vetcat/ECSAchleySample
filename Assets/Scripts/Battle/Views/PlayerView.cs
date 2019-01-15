using UnityEngine;
using Zenject;

namespace Battle.Views
{    
    public class PlayerView : MonoBehaviour, ITankView
    {
        public Transform MountPointWeaponOne;
        public Transform MountPointWeaponTwo;
                
        public CharacterController Controller
        {
            get
            {
                if (_controller == null)
                    _controller = GetComponent<CharacterController>();
                return _controller;
            }
        }
        
        private CharacterController _controller;        
        
        public class Pool : MonoMemoryPool<Vector3, Vector3, PlayerView>
        {         
            protected override void Reinitialize(Vector3 position, Vector3 forward, PlayerView item)
            {
                item.transform.position = position;
                item.transform.forward = forward;
            }
        }

        public void SimpleMove(Vector3 velocity)
        {
            Controller.SimpleMove(velocity);
        }

        public void SetRotation(Vector3 rotation)
        {
            transform.Rotate(rotation);
        }
    }
}