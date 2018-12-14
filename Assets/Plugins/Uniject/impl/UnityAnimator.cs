using System;
using UnityEngine;

namespace Uniject.Impl 
{
	public class UnityAnimator : IAnimator
	{
		readonly Animator _animator;
		public Animator Animator { get { return _animator; } }

		public UnityAnimator (GameObject obj)
		{
			_animator = obj.GetComponent<Animator>();
			if (_animator == null) {
				_animator = obj.AddComponent<Animator>();
			}
		}
		
		public UnityAnimator ()
		{
		}


		#region IAnimator implementation
        public bool IsValid
        {
            get { return _animator != null; }
        }
        
        public void SetBool (string name, bool value)
		{
			_animator.SetBool (name, value);
		}
		public void SetFloat (string name, float value)
		{
			_animator.SetFloat (name, value);
		}

		public void SetInteger (string name, int value)
		{
			_animator.SetInteger (name, value);
		}

		public void SetTrigger (string name)
		{
			_animator.SetTrigger (name);
		}

		public void ResetTrigger (string name)
		{
			_animator.ResetTrigger (name);
		}

		public bool GetBool (string name)
		{
			return _animator.GetBool (name);
		}


		public float GetFloat (string name)
		{
			return _animator.GetFloat (name);
		}


		public int GetInteger (string name)
		{
			return _animator.GetInteger (name);
		}

		public AnimatorStateInfo GetCurrentAnimatorStateInfo (int layerIndex)
		{
			return _animator.GetCurrentAnimatorStateInfo (layerIndex);
		}

		public AnimatorTransitionInfo GetAnimatorTransitionInfo (int layerIndex)
		{
			return _animator.GetAnimatorTransitionInfo (layerIndex);
		}

		public void SetLayerWeight (int layer, float weight)
		{
			_animator.SetLayerWeight (layer, weight);
		}

		public bool IsInTransition (int layerIndex)
		{
			return  _animator != null && _animator.IsInTransition (layerIndex);
		}

		public int LayerCount {
			get {
				return _animator != null ? _animator.layerCount : 0;
			}
		}

		public StateMachineBehaviour[] GetBehaviours ()
		{
			return _animator.GetBehaviours<StateMachineBehaviour> ();
		}

		#endregion
	}
}

