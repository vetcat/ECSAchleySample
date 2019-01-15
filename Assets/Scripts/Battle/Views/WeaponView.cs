using Battle.Enums;
using UnityEngine;
using Component = anygames.ashley.core.Component;

namespace Battle.Views
{    
    public class WeaponView : MonoBehaviour, Component
    {        
        public EWeaponType Type;
        public float FireRate;
        public Transform[] FirePoints;

        private int _lastFpIndex;
        private int _lastFrame;

        public Transform GetFirePoint()
        {
            if (_lastFrame != Time.frameCount)
            {
                if (_lastFpIndex < FirePoints.Length - 1)
                    _lastFpIndex++;
                else
                    _lastFpIndex = 0;

                _lastFrame = Time.frameCount;
            }

            return FirePoints[_lastFpIndex];            
        }
    }
}