using System;
using UnityEngine;

namespace Uniject.Impl {
    public class UnityTransform : ITransform {

        private Transform transform { get; set; }

        public UnityTransform (UnityEngine.GameObject obj) {
            this.transform = obj.transform;
        }

//		public UnityTransform (Transform tr) {
//			this.transform = tr;
//		}
//

        public Vector3 Position {
			get { return transform != null ? transform.position : Vector3.zero; }
            set { transform.position = value; }
        }

		public Vector2 LocalPosition {
			get { return transform.localPosition; }
			set { transform.localPosition = value; }
		}


        public Vector3 localScale {
            get { return transform.localScale; }
            set { transform.localScale = value; }
        }

        public Quaternion Rotation {
            get { return transform.rotation; }
            set { transform.rotation = value; }
        }

		public Quaternion LocalRotation {
			get { return transform.localRotation; }
			set { transform.localRotation = value; }
		}

        public Vector3 Forward {
            get { return transform.forward; }
            set { transform.forward = value; }
        }

		public Vector3 ForwardDirection {
    		get {
				return transform.TransformDirection(Vector3.forward);
    		}
    	}

        public Vector3 Up {
            get { return transform.up; }
            set { transform.up = value;}
        }

		public Vector3 Right {
			get { return transform.right; }
			set { transform.right = value; }
		}

        private ITransform actualParent;
        public ITransform Parent {
            get { return actualParent; }
            set
            {
	            transform.parent = ((UnityTransform) value)?.transform;
//				transform.SetParent (value != null ? ((UnityTransform)value).transform : null);
                this.actualParent = value;
            }
        }

        public void Translate(Vector3 byVector) {
            transform.Translate(byVector);
        }

        public void LookAt(Vector3 point) {
            transform.LookAt(point);
        }

        public Vector3 TransformDirection(Vector3 dir) {
            return transform.TransformDirection(dir);
        }

		public Vector3 EulerAngles { get { return transform.eulerAngles; }  set { transform.eulerAngles = value; } }

		public ITransform FindChild (string name)
		{
			var transforms = transform.GetComponentsInChildren<Transform> ();
			foreach( var tr in transforms )
			{
				if( tr.name.Equals (name) )
				{
					return new UnityTransform(tr.gameObject);
				}
			}
			return null;
		}

		public Transform ToUnityTransform ()
    	{
			return transform;
    	}

		public void Rotate (Vector3 axis, float angle)
    	{
			transform.Rotate (axis, angle);
    	}

	    public void Rotate(float x, float y, float z)
	    {
		    transform.Rotate(x, y, z);
	    }

	    public Vector3 TransformPoint (Vector3 localPoint)
		{
			return transform.TransformPoint (localPoint);
		}
	}
}

