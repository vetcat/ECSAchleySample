using UnityEngine;

namespace Battle.InputControllers
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
    }
}