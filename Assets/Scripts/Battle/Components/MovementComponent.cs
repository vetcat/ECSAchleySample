using anygames.ashley.core;
using UniRx;

namespace Battle.Components
{
    public class MovementComponent : Component
    {
        public ReactiveProperty<float> Speed { get; }
        public float SpeedStartValue { get; private set; }

        public MovementComponent()
        {
            SpeedStartValue = 0;
            Speed = new ReactiveProperty<float>(SpeedStartValue);
        }

        public void SetSpeed(float speed)
        {            
            SpeedStartValue = Speed.Value = speed;
        }

        public void ChangeSpeed(float speed)
        {
            Speed.Value = speed;
        }
    }
}