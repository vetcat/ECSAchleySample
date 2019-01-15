using Battle.Enums;

namespace Battle.Signals
{
    public class SignalWeaponSwitch
    {
        public readonly EWeaponType WeaponType;

        public SignalWeaponSwitch(EWeaponType weaponType)
        {
            WeaponType = weaponType;
        }
    }
}