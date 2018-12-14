using System;
using UnityEngine;

namespace Uniject {
    public interface IRigidBody2D {
        void AddForce(UnityEngine.Vector3 force);
        void AddTorque(float torque, ForceMode2D mode);

		void Freeze ();

        bool enabled { get; set; }

        float Rotation {
            get;
            set;
        }

        float drag { get; set; }
        float mass { get; set; }

        Vector3 Position {
            get;
        }

        Vector3 Forward {
            get;
        }

        RigidbodyConstraints2D constraints {
            get;
            set;
        }

		float gravityScale {
            get;
            set;
        }

        bool isKinematic { get; set; }

		RigidbodyType2D bodyType { set; get; }

		Vector2 velocity { set; get; }

		float angularVelocity { set; get; }
    }
}
