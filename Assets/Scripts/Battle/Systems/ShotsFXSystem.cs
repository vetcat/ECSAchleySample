using anygames.ashley.core;
using anygames.ashley.systems;
using Battle.Components;
using Battle.Signals;
using Battle.Views;
using UnityEngine;
using Zenject;
using Entity = anygames.ashley.core.Entity;

namespace Battle.Systems
{
    public class ShotsFXSystem : IteratingSystem
    {
        private readonly ComponentMapper<ShotFXView> _viewMapper = ComponentMapper<ShotFXView>.getFor();
        private readonly ComponentMapper<ShotFXComponent> _shotFXMapper = ComponentMapper<ShotFXComponent>.getFor();
        private readonly SignalBus _signalBus;
        private RaycastHit _hit;

        public ShotsFXSystem(SignalBus signalBus) : base(Family.all(typeof(ShotFXComponent)).get())
        {
            _signalBus = signalBus;
        }

        protected override void ProcessEntity(Entity entity, float deltaTime)
        {            
            var view = _viewMapper.get(entity);
            var shot = _shotFXMapper.get(entity);
            
            view.Elapsed += deltaTime;                        

            if (view.Elapsed >= view.LifeTime)
            {
                _signalBus.Fire(new SignalShotFXDestroy(view));
            }
        }
    }
}