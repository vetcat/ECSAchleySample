using System.Collections.Generic;
using System;
using UnityEngine;
using Uniject;
using System.Collections;
using UnityClient;
using Object = UnityEngine.Object;

namespace Uniject.Impl {
    public class UnityGameObject : TestableGameObject {

        public GameObject obj { get; private set; }

		readonly UnityGameObjectBridge _bridge;

		public UnityGameObject (GameObject obj) : base(new UnityTransform(obj)) {
            this.obj = obj;
			_bridge = obj.AddComponent<UnityGameObjectBridge> ();
			_bridge.wrapping = this;
        }

        public override void Destroy() {
            base.Destroy();
            Object.Destroy (obj);
        }

        public override bool Active {
            get { return obj.active; }
            set { obj.active = value; }
        }

        public override string Name {
            get { return obj.name; }
            set { obj.name = value; }
        }

		public override string Tag {
			get { return obj.tag; }
			set { obj.tag = value; }
		}

        public override void SetActiveRecursively(bool active) {
            obj.SetActiveRecursively(active);
        }

	    public override void SetActive(bool active)
	    {
		    obj.SetActive(active);
	    }


	    public override int InstanceId {
			get { return obj.GetInstanceID (); }
		}


		public override int Layer {
            get { return obj.layer; }
            set { obj.layer = value; }
        }

		/// <summary>
		/// Dont destroy when new scene is loaded
		/// </summary>
		/// <value><c>true</c> if this instance is not destroy on load; otherwise, <c>false</c>.</value>
		public bool DontDestroyOnLoad
		{
			set {
				if (value) {
					UnityEngine.Object.DontDestroyOnLoad (obj);
				}
			}
		}


		public override void StartCoroutine(IEnumerator coroutine)
		{
			_bridge.StartCoroutine (coroutine);
		}

		public override void StopCoroutine(IEnumerator coroutine)
		{
			_bridge.StopCoroutine (coroutine);
		}

		public override void StopAllCoroutines ()
    	{
			_bridge.StopAllCoroutines ();
    	}

		readonly List<IInvokeRepeatable> _repeatables = new List<IInvokeRepeatable> ();
		internal List<IInvokeRepeatable> Repeatables { get { return _repeatables; } }

	    public override void AddInvokeRepeatable(IInvokeRepeatable repeatable)
		{
			_repeatables.Add (repeatable);
		}

	    public override bool RemoveInvokeRepeatable(IInvokeRepeatable repeatable)
		{
			return _repeatables.Contains (repeatable) && _repeatables.Remove (repeatable);
		}


		public override void StartInvokeRepeating(float inTime, float repeatTime)
		{
			_bridge.InvokeRepeating (inTime, repeatTime);
		}

		public override void StartInvokeRepeating(IInvokeRepeatable repeatable, float inTime, float repeatTime)
		{
			_repeatables.Add (repeatable);
			_bridge.InvokeRepeating (inTime, repeatTime);
		}

		public override void CancelInvoke()
		{
			_bridge.CancelInvoke ();
		}

		public void AddAnimatorEventListener<T>(IAnimatorEventHandler handler) where T : Component, IAnimatorEventListener 
		{
			IAnimatorEventListener listener = _bridge.gameObject.AddComponent<T> ();
			listener.Configure (handler);
		}

		public bool IsRendererVisibleInCamera { get { return _bridge.IsRendererVisibleInCamera; } }

	}
}
