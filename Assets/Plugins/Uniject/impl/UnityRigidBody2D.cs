using System;
using UnityEngine;

namespace Uniject.Impl {
    public class UnityRigidBody2D : IRigidBody2D {
		private Rigidbody2D body;

		public UnityRigidBody2D (GameObject obj)
		{
			this.body = obj.GetComponent<Rigidbody2D> ();
			if (this.body == null) {
				this.body = obj.AddComponent<Rigidbody2D> ();
			}
		}

		public void AddForce (Vector3 force)
		{
			this.body.AddForce (force);
		}

		public void AddTorque (float torque, ForceMode2D mode)
		{
			this.body.AddTorque (torque, mode);
		}

		public void Freeze ()
		{
			body.velocity = Vector2.zero;
			body.angularVelocity = 0;
		}

		public float drag {
			get { return body.drag; }
			set { body.drag = value; }
		}

		public float mass {
			get { return body.mass; }
			set { body.mass = value; }
		}

		public Vector2 velocity {
			get { return body.velocity; }
			set { body.velocity = value; }
		}

		public float angularVelocity {
			get { return body.angularVelocity; }
			set { body.angularVelocity = value; }
		}

		public bool enabled {
			get { return !body.isKinematic; }
			set { body.isKinematic = !value; }
		}

		public float Rotation {
			get { return body.rotation; }
			set { this.body.rotation = value; }
		}

		public Vector3 Position {
			get { return body.position; }
			set { this.body.position = value; }
		}

		public Vector3 Forward {
			get { return body.transform.forward; }
		}

		public RigidbodyConstraints2D constraints {
			get { return body.constraints; }
			set { body.constraints = value; }
		}

		public float gravityScale {
			get { return body.gravityScale; }
			set { body.gravityScale = value; }
		}

		public bool isKinematic {
			get { return body.isKinematic; }
			set { body.isKinematic = value; }
		}

		public RigidbodyType2D bodyType {
			get { return body.bodyType; }
			set { body.bodyType = value; }
		}

    }
}

