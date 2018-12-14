using System;
using UnityEngine;

namespace Uniject.Impl {
    public class UnityAudioListener : TestableComponent, IAudioListener {

        public UnityAudioListener(ITestableGameObject parent, GameObject obj) : base(parent) {
            AudioListener listener = obj.GetComponent<AudioListener>();
            if (null == listener) {
                obj.AddComponent<AudioListener>();
            }
        }

        public void noOp() {
        }
    }
}

