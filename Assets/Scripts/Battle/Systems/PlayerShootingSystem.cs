using anygames.ashley.core;
using anygames.ashley.systems;
using Battle.Components;
using Battle.Controllers.InputControllers;
using Battle.Signals;
using UnityEngine;
using Zenject;

namespace Battle.Systems
{
    public class PlayerShootingSystem : IteratingSystem
    {                
        public bool IsFireProcess => _inputController.IsFireProcess();
        
        private IInputController _inputController;        
        
        private readonly ComponentMapper<WeaponsComponent> _weaponsMapper = ComponentMapper<WeaponsComponent>.getFor();
        private readonly ComponentMapper<ShootingComponent> _shootingMapper = ComponentMapper<ShootingComponent>.getFor();
        private readonly SignalBus _signalBus;

        public PlayerShootingSystem(SignalBus signalBus, IInputController inputController) : base(Family.all(typeof(WeaponsComponent), typeof(ShootingComponent)).get())
        {            
            _signalBus = signalBus;
            _inputController = inputController;
        }
        
        protected override void ProcessEntity(Entity entity, float deltaTime)
        {                        
            //Debug.Log("PlayerShootingSystem ProcessEntity deltaTime = " + deltaTime);
            
            var model = _weaponsMapper.get(entity);
            var shooting = _shootingMapper.get(entity);                                              
            
            if (IsFireProcess)
            {                                
                if (shooting.IsFirstTimeShot)
                {
                    var firePoint = model.CurrentWeaponView.GetFirePoint();
                    Fire(model.CurrentWeaponView.FireRate, firePoint.position, firePoint.forward);
                    shooting.IsFirstTimeShot = false;
                    shooting.DurationTime = 0f;
                }
                
                shooting.DurationTime += deltaTime;
                if (shooting.DurationTime >= model.CurrentWeaponView.FireRate)
                {
                    var firePoint = model.CurrentWeaponView.GetFirePoint();
                    Fire(model.CurrentWeaponView.FireRate, firePoint.position, firePoint.forward);
                    shooting.DurationTime -= model.CurrentWeaponView.FireRate;
                }
            }
            else
            {
                if (shooting.DurationTime < model.CurrentWeaponView.FireRate)
                    shooting.DurationTime += deltaTime;
                    
                if (!shooting.IsFirstTimeShot && shooting.DurationTime > model.CurrentWeaponView.FireRate)
                    shooting.IsFirstTimeShot = true;
            }            
        }

        private void Fire(float fireRate, Vector3 position, Vector3 forward)
        {
            //Debug.Log("Shot");
            _signalBus.Fire(new SignalShotSpawn(fireRate, position, forward));                                                
        }              
    }
}