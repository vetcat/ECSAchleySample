using System;
using UnityEngine;

namespace Uniject {
    public interface IAudioSource {
        void Play();
        void loopSound(AudioClip clip);
        void playOneShot(AudioClip clip);
		void Stop();
		AudioClip Clip { set; }

        bool isPlaying { get; }
    }

}
