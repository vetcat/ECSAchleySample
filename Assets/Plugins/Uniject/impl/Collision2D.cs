using System;
using UnityEngine;

namespace Uniject {
    public struct Collision2D {
        public Vector3 relativeVelocity { get; private set; }
        public ITransform transform { get; private set; }
        public ITestableGameObject gameObject { get; private set; }
        public Collider2D collider { get; set; }
        public Collider2D otherCollider { get; set; }
        public ContactPoint2D[] contacts { get; private set; }

        public Collision2D(Vector3 relativeVelocity,
                         ITransform transform,
                         ITestableGameObject gameObject, Collider2D collider, Collider2D otherCollider,
		                   ContactPoint2D[] contacts) : this() {
            this.relativeVelocity = relativeVelocity;
            this.transform = transform;
            this.gameObject = gameObject;
            this.collider = collider;
            this.otherCollider = otherCollider;
            this.contacts = contacts;
        }
    }
}

