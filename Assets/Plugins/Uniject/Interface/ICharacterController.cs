using System;
using UnityEngine;

namespace Uniject
{
	public interface ICharacterController
	{
		CollisionFlags Move(Vector3 motion);
		Vector3 Velocity { get; }

		Vector3 Center { set; get; }
		float Radius { set; get; }
		float Height { set; get; }

		bool IsGrounded { get; }

		float SlopeLimit { set; get; }

		float StepOffset { set; get; }

		bool DetectCollisions { set; get; }

		bool Enabled { set; get; }
	}
}

