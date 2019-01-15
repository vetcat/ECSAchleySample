using anygames.ashley.core;
using anygames.ashley.systems;
using Battle.Components;
using Battle.Controllers.InputControllers;
using Battle.Views;
using UnityEngine;

namespace Battle.Systems
{
    public class PlayerMovementSystem : IteratingSystem
    {
        private IInputController _inputController;        
                
        private readonly ComponentMapper<PlayerViewComponent> _viewMapper = ComponentMapper<PlayerViewComponent>.getFor();
        private readonly ComponentMapper<MovementComponent> _movementMapper = ComponentMapper<MovementComponent>.getFor();

        public PlayerMovementSystem(IInputController inputController) : base(Family.all(typeof(PlayerViewComponent), typeof(MovementComponent)).get())
        {            
            _inputController = inputController;
        }

        protected override void ProcessEntity(Entity entity, float deltaTime)
        {                                                                
            var view = _viewMapper.get(entity);
            var movementComponent = _movementMapper.get(entity);
            
            Move(view.View, movementComponent.Speed.Value, deltaTime);                                          
//            view.View.transform.position += velocity * deltaTime;
        }

        private void Move(PlayerView view, float movementSpeed, float deltaTime)
        {
            var direction = view.transform.TransformDirection(new Vector3(0f, 0f, _inputController.GetVertical()));
            //var velocity = direction * MovementSpeed * deltaTime;            
            view.SimpleMove(direction * movementSpeed);                        
        }        
    }
}