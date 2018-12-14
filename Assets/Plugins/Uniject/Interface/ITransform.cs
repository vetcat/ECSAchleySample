using System;
using UnityEngine;

namespace Uniject {
    public interface ITransform {
	    Vector3 Position {
		    get;
		    set;
	    }

		Vector2 LocalPosition {
			get;
			set;
		}

        Vector3 localScale {
            get;
            set;
        }

        Quaternion Rotation {
            get;
            set;
        }

		Quaternion LocalRotation {
			get;
			set;
		}

        Vector3 Forward {
            get;
            set;
        }

		Vector3 ForwardDirection {
			get;
		}


        Vector3 Up {
            get;
            set;
        }

		Vector3 Right {
			get;
			set;
		}

        ITransform Parent { get; set; }

        void Translate(Vector3 byVector);
        void LookAt(Vector3 point);
        Vector3  TransformDirection(Vector3 dir);
		Vector3 TransformPoint (Vector3 localPoint);

		Vector3 EulerAngles { get; set; }

		ITransform FindChild(string name);

		Transform ToUnityTransform();

		void Rotate(Vector3 axis, float angle);
	    void Rotate(float x, float y, float z);
    }
}

