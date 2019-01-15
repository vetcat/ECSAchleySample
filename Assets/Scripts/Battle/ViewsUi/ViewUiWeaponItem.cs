using Battle.Enums;
using UiCore;
using UnityEngine;
using UnityEngine.UI;

namespace Battle.ViewsUi
{
    public class ViewUiWeaponItem : UiView 
    {
        public Text TextName;

        [Header("Icon Weapon")]                 
        public Image ImageIcon;
         
        [Header("Cooldown indicator")]
        public Image ImageCooldown;
        
        [HideInInspector] 
        public EWeaponType WeaponType;
        
        public Image SelectedBackGround;
        
        public Button ButtonSelect;
    }
}
