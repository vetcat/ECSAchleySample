using anygames.ashley.core;
using anygames.ashley.systems;
using Battle.Components;
using Battle.Controllers.InputControllers;
using Battle.Views;
using UnityEngine;

namespace Battle.Systems
{
    public class PlayerRotationtSystem : IteratingSystem
    {
        private IInputController _inputController;        
                
        private readonly ComponentMapper<PlayerViewComponent> _viewMapper = ComponentMapper<PlayerViewComponent>.getFor();
        private readonly ComponentMapper<RotationComponent> _rotationMapper = ComponentMapper<RotationComponent>.getFor();

        public PlayerRotationtSystem(IInputController inputController) : base(Family.all(typeof(PlayerViewComponent), typeof(MovementComponent)).get())
        {            
            _inputController = inputController;
        }

        protected override void ProcessEntity(Entity entity, float deltaTime)
        {                                                                
            var view = _viewMapper.get(entity);
            var rotationComponent = _rotationMapper.get(entity);
                               
            Rotate(view.View, rotationComponent.Speed, deltaTime);                       
        }

        private void Rotate(PlayerView view, float rotationSpeed, float deltaTime)
        {
            var rotation = _inputController.GetHorizontal() * rotationSpeed * deltaTime;
            view.SetRotation(new Vector3(0f, rotation, 0f));
        }
    }
}