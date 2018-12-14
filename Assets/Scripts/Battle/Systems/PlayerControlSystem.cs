using System;
using anygames.ashley.core;
using anygames.ashley.systems;
using Battle.Components;
using Battle.InputControllers;
using UnityEngine;

namespace Battle.Systems
{
    public class PlayerControlSystem : IteratingSystem, IDisposable
    {
        private readonly float MovementSpeed = 5f;//Unit per second;
        private readonly float RotationSpeed = 180f;//Degrees per second;
        private IInputController _inputController;        
        
        private readonly ComponentMapper<Transformable> _transformMapper = ComponentMapper<Transformable>.getFor();

        public PlayerControlSystem(IInputController inputController) : base(Family.all(typeof(Transformable), typeof(PlayerControlled)).get())
        {
            _inputController = inputController;
        }

        public void Dispose()
        {            
        }

        protected override void ProcessEntity(Entity entity, float deltaTime)
        {            
            var transform = _transformMapper.get(entity).Value;

            var direction = transform.TransformDirection(new Vector3(0f, 0f, _inputController.GetVertical()));
            var velocity = direction * MovementSpeed;

            var rotation = _inputController.GetHorizontal() * RotationSpeed * deltaTime;

            transform.Position += velocity * deltaTime;
            transform.Rotate(0f, rotation, 0f);            
        }
    }
}