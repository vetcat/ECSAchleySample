using Battle.ViewsUi;
using UiCore;
using UnityEngine;
using Zenject;

public class BattleUiInstaller : MonoInstaller
{
    public Transform Canvas;

    public ViewUiWeapons ViewUiWeapons;
    public ViewUiPlayerInfo ViewUiPlayerInfo;     
    
    public override void InstallBindings()
    {
        Container.BindViewController<ViewUiWeapons, PresenterWeapons>(ViewUiWeapons, Canvas);
        Container.BindViewController<ViewUiPlayerInfo, PresenterPlayerInfo>(ViewUiPlayerInfo, Canvas);
    }
}