using System;
using UnityEngine;

namespace Uniject.Impl 
{
	public class UnityCharacterController : ICharacterController
	{
		readonly CharacterController _controller;
		public CharacterController Controller { get { return _controller; } }

		public UnityCharacterController(GameObject obj) {
			_controller = obj.GetComponent<CharacterController>();
			if (_controller == null) {
				_controller = obj.AddComponent<CharacterController>();
			}
		}

		#region ICharacterController implementation

		public CollisionFlags Move (Vector3 motion)
		{
			return _controller.Move (motion);
		}

		public bool IsGrounded { get { return _controller.isGrounded; } }

		public Vector3 Velocity {	get { return _controller.velocity; } }

		public Vector3 Center {
			get { return _controller.center; } 
			set { _controller.center = value; }
		}

		public float Radius {
			get { return _controller.radius; } 
			set { _controller.radius = value; }
		}

		public float Height {
			get { return _controller.height; } 
			set { _controller.height = value; }
		}

		public float SlopeLimit {
			get { return _controller.slopeLimit; } 
			set { _controller.slopeLimit = value; }
		}

		public float StepOffset {
			get { return _controller.stepOffset; } 
			set { _controller.stepOffset = value; }
		}

		public bool DetectCollisions {
			get { return _controller.detectCollisions; } 
			set { _controller.detectCollisions = value; }
		}

		public bool Enabled {
			get { return _controller.enabled; } 
			set { _controller.enabled = value; }
		}

		#endregion
	}
}

