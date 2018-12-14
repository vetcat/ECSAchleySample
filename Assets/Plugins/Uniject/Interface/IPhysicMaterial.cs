using System;
using UnityEngine;

namespace Uniject {
    public interface IPhysicMaterial {
        float bounciness { get; set; }
        PhysicMaterialCombine frictionCombine { get; set; }
        PhysicMaterialCombine bounceCombine { get; set; }
    }
}

