using System;
using UnityEngine;

namespace Uniject
{
	public interface IAnimator
	{
        bool IsValid { get; }
        
        void SetBool (string name, bool value);
		void SetFloat (string name, float value);
		void SetInteger (string name, int value);
		void SetTrigger (string name);
		void ResetTrigger (string name);

		bool GetBool(string name);
		float GetFloat(string name);
		int GetInteger(string name);

		AnimatorStateInfo GetCurrentAnimatorStateInfo(int layerIndex);
		AnimatorTransitionInfo GetAnimatorTransitionInfo(int layerIndex);
		void SetLayerWeight(int layer, float weight);
		bool IsInTransition(int layerIndex);
		int LayerCount { get; }
		StateMachineBehaviour[] GetBehaviours ();
	}
}

