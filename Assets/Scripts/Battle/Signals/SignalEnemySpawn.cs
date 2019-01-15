using Battle.Enums;
using UnityEngine;

namespace Battle.Signals
{
    public class SignalEnemySpawn
    {        
        public readonly Vector3 Position;
        public readonly Vector3 Forward;
        public readonly EEnemyType Type;

        public SignalEnemySpawn(EEnemyType type, Vector3 position, Vector3 forward)
        {
            Type = type;
            Forward = forward;
            Position = position;            
        }
    }
}