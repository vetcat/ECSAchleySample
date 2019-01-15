using UnityEngine;

namespace Battle.Controllers.InputControllers
{
    public class StandardInputController : IInputController
    {
        public float GetHorizontal()
        {
            return Input.GetAxis("Horizontal");
        }

        public float GetVertical()
        {
            return Input.GetAxis("Vertical");
        }

        public bool IsFireProcess()
        {
            return Input.GetButton("Fire1");
        }

        public bool SwitchWeaponNext()
        {
            return Input.GetButtonDown("SwitchWeaponNext");
        }

        public bool SwitchWeaponPrevious()
        {
            return Input.GetButtonDown("SwitchWeaponPrevious");
        }
    }
}