using System;
using anygames.ashley.core;
using Battle.Components;
using Battle.Controllers.InputControllers;
using Battle.Enums;
using Battle.Signals;
using Battle.Views;
using UniRx;
using Zenject;

namespace Battle.Controllers
{
    public class WeaponsController : IInitializable, IDisposable, ITickable
    {
        private readonly SignalBus _signalBus;
        readonly CompositeDisposable _disposables = new CompositeDisposable();
        private readonly IInputController _inputController;
        private Entity _playerEntity;
        
        public WeaponsController(SignalBus signalBus, IInputController inputController)
        {
            _inputController = inputController;
            _signalBus = signalBus;
        }
        
        public void Initialize()
        {
            _signalBus.GetStream<SignalPlayerCreated>()
                .Subscribe(x=>OnPlayerCreated(x.Entity)).AddTo(_disposables);       
        }
        
        public void Dispose()
        {
            _disposables.Dispose();
        }

        private void OnPlayerCreated(Entity playerEntity)
        {
            _playerEntity = playerEntity;
        }

        public void Tick()
        {
            if (_inputController.SwitchWeaponNext())                
                SwitchWeaponNext(_playerEntity);

            if (_inputController.SwitchWeaponPrevious())
                SwitchWeaponPrevious(_playerEntity);
        }

        public void SwitchWeapon(EWeaponType weaponType)
        {            
            SwitchWeapon(_playerEntity, weaponType);
        }
        
        public void AddWeapon(Entity player, WeaponView weaponView)
        {
            var weaponsComponent = player.getComponent<WeaponsComponent>();
            weaponsComponent.Weapons.Add(weaponView);
        }

        public void SetCurrentWeapon(Entity player, EWeaponType weaponType)
        {
            var weaponsComponent = player.getComponent<WeaponsComponent>();
            
            foreach (var weapon in weaponsComponent.Weapons)
            {
                if (weapon.Type != weaponType)
                    weapon.gameObject.SetActive(false);
                else
                {
                    weapon.gameObject.SetActive(true);
                    weaponsComponent.CurrentWeaponView = weapon;                    
                }
            }                        
        }
        
        private void SwitchWeaponNext(Entity player)
        {
            var weaponsComponent = player.getComponent<WeaponsComponent>();
            
            var index = weaponsComponent.Weapons.IndexOf(weaponsComponent.CurrentWeaponView);
            if (index + 1 < weaponsComponent.Weapons.Count)
                SwitchWeapon(player, weaponsComponent.Weapons[index + 1].Type);
        }

        private void SwitchWeaponPrevious(Entity player)
        {
            var weaponsComponent = player.getComponent<WeaponsComponent>();
            
            var index = weaponsComponent.Weapons.IndexOf(weaponsComponent.CurrentWeaponView);
            if (index - 1 >= 0)
                SwitchWeapon(player, weaponsComponent.Weapons[index - 1].Type);
        }

        private void SwitchWeapon(Entity player, EWeaponType weaponType)
        {                                    
            SetCurrentWeapon(player, weaponType);
            _signalBus.Fire(new SignalWeaponSwitch(weaponType));            
        }
    }
}