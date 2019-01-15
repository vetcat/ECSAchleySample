using System.Collections.Generic;
using Battle.Views;
using Component = anygames.ashley.core.Component;

namespace Battle.Components
{
    public class WeaponsComponent : Component
    {
        public WeaponView CurrentWeaponView;
        public List<WeaponView> Weapons => _weapons;
        
        private List<WeaponView> _weapons = new List<WeaponView>();       
    }
}