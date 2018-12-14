using System;
using UnityEngine;

namespace Uniject.Impl {
    public class UnityPhysicsMaterial : IPhysicMaterial {
        public UnityEngine.PhysicMaterial material { get; private set; }
        public UnityPhysicsMaterial(UnityEngine.PhysicMaterial mat) {
            this.material = mat;
        }

        public float bounciness {
            get { return material.bounciness; }
            set { material.bounciness = value; }
        }

        public PhysicMaterialCombine frictionCombine {
            get { return material.frictionCombine; }
            set { material.frictionCombine = value; }
        }

        public PhysicMaterialCombine bounceCombine {
            get { return material.bounceCombine; }
            set { material.bounceCombine = value; }
        }
    }
}

