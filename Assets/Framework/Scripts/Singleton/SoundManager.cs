using UnityEngine;

// Global singleton to use functionality of SoundController
namespace Framework
{
    public class SoundManager : Singleton<SoundManager>
    {
        private SoundController Controller;

        public void Register(SoundController controller)
        {
            Controller = controller;
        }

        public void Play(AudioSource source, float volume = -1f)
        {
            if (Controller != null)
            {
                Controller.Play(source, volume);
            }
        }

        public void Stop(AudioSource source)
        {
            if (Controller != null)
            {
                Controller.Stop(source);
            }
        }

        public void PlayOneShot(AudioClip clip, float volume = -1f)
        {
            if (Controller != null)
            {
                Controller.PlayOneShot(clip, volume);
            }
        }

        public void PlayAndFade(AudioSource source, float targetVolume, float duration, float startVolume = -1f)
        {
            if (Controller != null)
            {
                Controller.PlayAndFade(source, targetVolume, duration, startVolume);
            }
        }

        public void Fade(AudioSource source, float targetVolume, float duration, float startVolume = -1f)
        {
            if (Controller != null)
            {
                Controller.Fade(source, targetVolume, duration, startVolume);
            }
        }

        public void StopFade(AudioSource source, bool killVolume = false)
        {
            if (Controller != null)
            {
                Controller.StopFade(source, killVolume);
            }
        }

        public void FadeAllAudio(float volume, float duration, float startTime = 0f)
        {
            if (Controller != null)
            {
                Controller.FadeAllAudio(volume, duration, startTime);
            }
        }

        public void ToggleMasterMute(bool shouldMute)
        {
            if (Controller != null)
            {
                Controller.ToggleMasterMute(shouldMute);
            }
        }
    }
}