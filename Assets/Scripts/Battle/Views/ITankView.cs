using UnityEngine;

namespace Battle.Views
{
    public interface ITankView
    {
        void SimpleMove(Vector3 velocity);
        void SetRotation(Vector3 rotation);
    }
}