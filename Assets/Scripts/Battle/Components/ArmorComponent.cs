using UniRx;
using Component = anygames.ashley.core.Component;

namespace Battle.Components
{
    public class ArmorComponent : Component 
    {
        public ReactiveProperty<int> Armor { get; }     
        public int ArmorStartValue { get; private set; }

        public ArmorComponent()
        {
            ArmorStartValue = 0;
            Armor = new ReactiveProperty<int>(ArmorStartValue);                        
        }

        public void SetArmor(int armor)
        {
            ArmorStartValue = Armor.Value = armor;            
        }

        public void Damage(int armorDamage)
        {                        
            Armor.Value -= armorDamage;            
        }
    }
}