using UnityEngine;

namespace Battle.Signals
{
    public class SignalShotFXSpawn
    {        
        public readonly Vector3 Position;
        public readonly Vector3 Forward;

        public SignalShotFXSpawn(Vector3 position, Vector3 forward)
        {
            Forward = forward;
            Position = position;            
        }
    }
}