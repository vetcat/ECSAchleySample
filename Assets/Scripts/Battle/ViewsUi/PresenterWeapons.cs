using System.Collections;
using anygames.ashley.core;
using Battle.Components;
using Battle.Controllers;
using Battle.Enums;
using Battle.Signals;
using Battle.Systems;
using UiCore;
using UniRx;
using UnityEngine;
using Zenject;

namespace Battle.ViewsUi
{
    public class PresenterWeapons : UiPresenter<ViewUiWeapons>
    {
        private readonly SignalBus _signalBus;
        readonly CompositeDisposable _disposables = new CompositeDisposable();
        private ViewUiWeaponItem _currentWeaponItem;
        private WeaponsComponent _weapons;
        private WeaponsController _weaponsController;
        private const float MinTimeShowFireRate = .5f;

        public PresenterWeapons(SignalBus signalBus, WeaponsController weaponsController)
        {
            _weaponsController = weaponsController;
            _signalBus = signalBus;
        }
    
        public override void Initialize()
        {                    
            _signalBus.GetStream<SignalPlayerCreated>()
                .Subscribe(x=>OnPlayerCreated(x.Entity)).AddTo(_disposables);       

            _signalBus.GetStream<SignalShotSpawn>()
                .Subscribe(x=>OnShotSpawn(x.FireRate)).AddTo(_disposables);      

            _signalBus.GetStream<SignalWeaponSwitch>()
                .Subscribe(x=>OnWeaponSwitch(x.WeaponType)).AddTo(_disposables);                    
        }

        public override void Dispose()
        {
            _disposables.Dispose();
        }
        
        private void OnWeaponSwitch(EWeaponType weaponType)
        {            
            MarkSelect(_currentWeaponItem, false);
            _currentWeaponItem = GetItemByWeaponType(weaponType);
            MarkSelect(_currentWeaponItem, true);
        }

        private void OnPlayerCreated(Entity player)
        {
            _weapons = player.getComponent<WeaponsComponent>();
            
            foreach (var weapon in _weapons.Weapons)
                AddWeapon(weapon.Type);
            
            _currentWeaponItem = GetItemByWeaponType(_weapons.CurrentWeaponView.Type);
            MarkSelect(_currentWeaponItem, true);
        }

        private void AddWeapon(EWeaponType weaponType)
        {
            var item = View.CollectionWeaponItem.AddItem();
            item.TextName.text = weaponType.ToString();
            item.WeaponType = weaponType;
            item.ImageCooldown.fillAmount = 0f;
            MarkSelect(item, false);

            //for mobile version
            item.ButtonSelect.OnClickAsObservable()
                .Subscribe(x => OnClickButtonSelect(item.WeaponType))
                .AddTo(item.ButtonSelect);
        }

        private void OnClickButtonSelect(EWeaponType weaponType)
        {                             
            _weaponsController.SwitchWeapon(weaponType);
        }

        private void OnShotSpawn(float fireRate)
        {
            if (fireRate >= MinTimeShowFireRate)
                MainThreadDispatcher.StartUpdateMicroCoroutine(FireCooldown(fireRate, _currentWeaponItem));
        }  
        
        private IEnumerator FireCooldown(float fireRate, ViewUiWeaponItem item)
        {
            var endTime = Time.time + fireRate;
            while(endTime > Time.time)
            {                
                item.ImageCooldown.fillAmount = 1f - (endTime - Time.time) / fireRate;                
                yield return null;
            }
            item.ImageCooldown.fillAmount = 0f;
        }

        private ViewUiWeaponItem GetItemByWeaponType(EWeaponType weaponType)
        {
           return View.CollectionWeaponItem.GetItems().Find(f => f.WeaponType == weaponType);
        }

        private void MarkSelect(ViewUiWeaponItem item, bool value)
        {
            item.SelectedBackGround.gameObject.SetActive(value);
        }
    }
}
