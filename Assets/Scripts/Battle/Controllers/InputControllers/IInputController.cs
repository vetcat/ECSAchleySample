namespace Battle.Controllers.InputControllers
{
    public interface IInputController
    {
        float GetHorizontal();
        float GetVertical();
        bool IsFireProcess();
        bool SwitchWeaponNext();
        bool SwitchWeaponPrevious();
    }
}