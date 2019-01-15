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
    public class ShotsMoveSystem : IteratingSystem
    {
        private readonly ComponentMapper<ShotOneView> _viewMapper = ComponentMapper<ShotOneView>.getFor();
        private readonly ComponentMapper<ShotComponent> _shotMapper = ComponentMapper<ShotComponent>.getFor();
        private readonly SignalBus _signalBus;
        private RaycastHit _hit;

        public ShotsMoveSystem(SignalBus signalBus) : base(Family.all(typeof(ShotComponent)).get())
        {
            _signalBus = signalBus;
        }

        protected override void ProcessEntity(Entity entity, float deltaTime)
        {            
            var view = _viewMapper.get(entity);
            var shot = _shotMapper.get(entity);
            
            view.Elapsed += deltaTime;                        

            if (view.Elapsed >= view.LifeTime)
            {
                _signalBus.Fire(new SignalShotDestroy(view));
            }
            else
            {
                var velocity = view.transform.forward * view.Speed * deltaTime;
                var nextPosition = view.transform.position + velocity;
                var distance = Vector3.Distance(view.transform.position, nextPosition);                                                                                
                                
                if (Physics.Raycast(view.transform.position, view.transform.forward, out _hit, distance))
                {                                        
                    _signalBus.Fire(new SignalShotFXSpawn(_hit.point, _hit.normal));
                    _signalBus.Fire(new SignalShotDestroy(view));
                }
                else
                {
                    view.transform.position = nextPosition;            
                }
            }
        }
    }
}