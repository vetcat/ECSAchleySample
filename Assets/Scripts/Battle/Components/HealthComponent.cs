using UniRx;
using Component = anygames.ashley.core.Component;

namespace Battle.Components
{
    public class HealthComponent : Component 
    {
        public ReactiveProperty<int> Health { get; }
        public int HealthStartValue { get; private set; }

        public HealthComponent()
        {
            HealthStartValue = 0;
            Health = new ReactiveProperty<int>(HealthStartValue);                        
        }
        
        public void SetHealth(int health)
        {
            HealthStartValue = Health.Value = health;            
        }

        public void Damage(int healthDamage)
        {                        
            Health.Value -= healthDamage;            
        }
    }
}