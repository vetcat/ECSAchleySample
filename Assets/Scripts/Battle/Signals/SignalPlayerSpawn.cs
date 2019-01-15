using UnityEngine;

namespace Battle.Signals
{
    public class SignalPlayerSpawn
    {        
        public readonly Vector3 Position;
        public readonly Vector3 Forward;

        public SignalPlayerSpawn(Vector3 position, Vector3 forward)
        {
            Forward = forward;
            Position = position;            
        }
    }
}