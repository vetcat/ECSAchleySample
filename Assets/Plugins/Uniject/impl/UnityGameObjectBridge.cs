using System;
using Uniject.Impl;
using UnityEngine;
using System.Collections;
using UniRx;

public class UnityGameObjectBridge : MonoBehaviour {

	IDisposable _updateDisposable = Disposable.Empty;
	IDisposable _lateUpdateDisposable = Disposable.Empty;

	void Awake()
	{
		_updateDisposable = Observable.EveryUpdate ().Where (v => wrapping.Active && wrapping.ComponentAmount > 0).Subscribe (_ => InternalUpdate ());
		_lateUpdateDisposable = Observable.EveryLateUpdate().Where (v => wrapping.Active && wrapping.ComponentAmount > 0 
		                                                         && wrapping.LateUpdateEnabled).Subscribe (_ => InternalLatelUpdate ());
	}

    public void OnDestroy() {
		_updateDisposable.Dispose ();
		_lateUpdateDisposable.Dispose ();
        wrapping.Destroy();
    }

    void InternalUpdate() {
        wrapping.Update();
    }

	void InternalLatelUpdate ()
	{
		wrapping.LateUpdate ();
	}

    public void OnCollisionEnter(Collision c) {
        UnityGameObjectBridge other = c.gameObject.GetComponent<UnityGameObjectBridge>();
        if (null != other) {
                Uniject.Collision testableCollision =
                new Uniject.Collision(c.relativeVelocity,
                                      other.wrapping.Transform,
                                      other.wrapping, c.collider,
                                      c.contacts);
            wrapping.OnCollisionEnter(testableCollision);
        }
    }

	public void OnCollisionEnter2D (Collision2D c)
	{
		UnityGameObjectBridge other = c.gameObject.GetComponent<UnityGameObjectBridge> ();
		if (null != other) {
			Uniject.Collision2D testableCollision =
			new Uniject.Collision2D (c.relativeVelocity,
								  other.wrapping.Transform,
								  other.wrapping, c.collider, c.otherCollider,
				                   c.contacts);
			wrapping.OnCollisionEnter2D (testableCollision);
		}

	}

    public UnityGameObject wrapping;

	internal void InvokeRepeating(float inTime, float repeatTime)
	{
		InvokeRepeating ("InternalInvoke", inTime, repeatTime);
	}

	internal void CancelInvoke()
	{
		CancelInvoke ("InternalInvoke");
	}

	void InternalInvoke()
	{
		for (int i = 0; i < wrapping.Repeatables.Count; i++) {
			wrapping.Repeatables [i].Repeate ();
		}
	}

	public bool IsRendererVisibleInCamera { private set; get; }

	void OnBecameVisible()
	{
		IsRendererVisibleInCamera = true;
	}

	void OnBecameInvisible()
	{
		IsRendererVisibleInCamera = false;
	}

	void OnMouseDown()
	{
		wrapping.OnMouseDown ();
	}
}

