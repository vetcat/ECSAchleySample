using UnityEngine;

namespace Battle.Signals
{
    public class SignalShotSpawn
    {
        public readonly float FireRate;
        public readonly Vector3 Position;
        public readonly Vector3 Forward;

        public SignalShotSpawn(float fireRate, Vector3 position, Vector3 forward)
        {
            Forward = forward;
            Position = position;
            FireRate = fireRate;
        }
    }
}