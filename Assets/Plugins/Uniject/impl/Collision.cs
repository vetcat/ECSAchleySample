using System;
using UnityEngine;

namespace Uniject {
    public struct Collision {
        public Vector3 relativeVelocity { get; private set; }
        public ITransform transform { get; private set; }
        public ITestableGameObject gameObject { get; private set; }
        public Collider collider { get; private set; }
        public ContactPoint[] contacts { get; private set; }

        public Collision(Vector3 relativeVelocity,
                         ITransform transform,
                         ITestableGameObject gameObject, Collider collider,
                         ContactPoint[] contacts) : this() {
            this.relativeVelocity = relativeVelocity;
            this.transform = transform;
            this.gameObject = gameObject;
            this.collider = collider;
            this.contacts = contacts;
        }
    }
}

