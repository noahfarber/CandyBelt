using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class SoundController : MonoBehaviour
    {
        // Master control
        private List<AudioSource> _ActiveSources = new List<AudioSource>(); // Stores all audio sources currently playing a clip. Can be helpful for master audio control.

        // One shot information
        private int _OneShotSourcesCount = 10; // Number of one-shot sources to create and add to pool on game start.
        private List<AudioSource> _OneShotSources = new List<AudioSource>(); // Stores generated one-shot sources.

        // Fade information
        private List<FadeData> _FadeInformation = new List<FadeData>(); // Stores information for fading audio sources.

        #region UNITY FUNCTIONS
        private void Awake()
        {
            CreateOneShotSources(_OneShotSourcesCount); // Creates a pool of audio sources to use for one-shot calls.
            SoundManager.Instance.Register(this); // Registers this controller with the SoundManager singleton
        }

        private void Update()
        {
            UpdateActiveSources(); // Checks if active audio sources are still playing a clip.
            ApplyFadeInformation(); // Applies volume changes to any fading audio sources.
        }
        #endregion


        #region PUBLIC CALLS
        //
        // Play any audio clip one time. Takes an AudioClip and (optionally) preferred volume.
        //
        public void PlayOneShot(AudioClip clip, float volume = 1f) 
        {
            AudioSource source = GetOneShotAudioSource();

            source.clip = clip;
            source.volume = volume;
            source.loop = false;
            source.mute = false;
            source.Play();

            _ActiveSources.Add(source); // Add one-shot source to active audio source pool.
        }

        //
        // Play any audio source from SoundManager so that it may be added to ActiveSounds. 
        //
        public void Play(AudioSource source, float volume = -1f)
        {
            source.mute = false;
            source.Stop();
            source.Play();

            if (!_ActiveSources.Contains(source))
            {
                _ActiveSources.Add(source); // Add source to active audio source pool.
            }
        }

        //
        // Play any audio source from SoundManager so that it may be added to ActiveSounds. 
        //
        public void Stop(AudioSource source)
        {
            source.Stop();

            if (!_ActiveSources.Contains(source))
            {
                _ActiveSources.Remove(source); // Add source to active audio source pool.
            }
        }

        //
        // Fades an audio source with optional parameters
        //
        public void Fade(AudioSource source, float targetVolume, float duration, float initialVolume)
        {
            initialVolume = initialVolume == -1f ? source.volume : initialVolume;

            CheckToRemoveFadeSource(source); // If source already has active fade information, remove this information so we can add a new fade info.

            _FadeInformation.Add(new FadeData(source, initialVolume, targetVolume, duration)); // Register new fade information with given data

            if (source.isPlaying && !_ActiveSources.Contains(source))
            {
                _ActiveSources.Add(source); // Only add to active sources if a clip is playing. --- (Technically you can fade a source that isn't playing a clip)
            }
        }

        //
        // Plays and fades an audio source with optional parameters
        //
        public void PlayAndFade(AudioSource source, float targetVolume, float duration, float initialVolume)
        {
            Play(source, initialVolume);
            Fade(source, targetVolume, duration, initialVolume);
        }

        //
        // Stops a fade on target audio source by removing it from the fade information list
        //
        public void StopFade(AudioSource source, bool killVolume)
        {
            source.volume = killVolume ? 0f : source.volume;
            for (int i = _FadeInformation.Count; i > 0; i--) // If fade is still in progress, remove fade data from active fade information
            {
                int index = i - 1;
                if (_FadeInformation[index].Source == source)
                {
                    _FadeInformation.RemoveAt(index);
                    break;
                }
            }
        }

        //
        // Fades all active audio sources to a target volume in target amount of seconds
        //
        public void FadeAllAudio(float volume, float duration, float startTime = 0f)
        {
            foreach (AudioSource source in _ActiveSources)
            {
                Fade(source, volume, duration, startTime);
            }
        }

        //
        // Enables or disables "mute" property on all audio sources
        //
        public void ToggleMasterMute(bool shouldMute)
        {
            foreach (AudioSource source in _ActiveSources)
            {
                source.mute = shouldMute;
            }
        }
        #endregion


        #region HELPER FUNCTIONS 
        //
        // Checks whether active sources are stil playing a clip, and removes them if they are not.
        //
        private void UpdateActiveSources() 
        {
            for (int i = _ActiveSources.Count; i > 0; i--)
            {
                int index = i - 1;
                if (!_ActiveSources[index].isPlaying)
                {
                    _ActiveSources.RemoveAt(index);
                }
            }
        }

        //
        // Applies fade data to audio source and removes specific fade data from collection once a fade is complete.
        //
        private void ApplyFadeInformation() 
        {
            for (int i = _FadeInformation.Count; i > 0; i--)
            {
                FadeData fadeInfo = _FadeInformation[i - 1];
                int index = i - 1;
                if (fadeInfo.ElapsedTime < fadeInfo.Duration)
                {
                    // Target volume multiplied by percentage of progress
                    fadeInfo.Source.volume = fadeInfo.InitialVolume + (fadeInfo.DeltaVolume * (fadeInfo.ElapsedTime / fadeInfo.Duration));

                    // Add to current elapsed time
                    fadeInfo.ElapsedTime += Time.deltaTime; 
                }
                else
                {
                    _FadeInformation.RemoveAt(index);
                }
            }
        }

        //
        // Checks and removes given audio source if it's a part of the active audio source list
        //
        private void CheckToRemoveFadeSource(AudioSource source)
        {
            for (int i = _FadeInformation.Count; i > 0; i--)
            {
                int index = i - 1;
                if (_FadeInformation[index].Source == source)
                {
                    _FadeInformation.RemoveAt(index);
                    break;
                }
            }
        }
        #endregion


        #region SETUP FUNCTIONS
        //
        // Creates a pool of audio sources for one-shot sounds to use
        //
        private void CreateOneShotSources(int count)
        {
            for (int i = 0; i < count; i++)
            {
                _OneShotSources.Add(CreateAudioSource());
            }
        }

        //
        // Creates and returns a new audio source attached to this script's GameObject
        //
        private AudioSource CreateAudioSource()
        {
            AudioSource newSource = gameObject.AddComponent<AudioSource>();
            newSource.playOnAwake = false;
            newSource.loop = false;
            newSource.volume = 1f;
            return newSource;
        }

        //
        // Gets an unused audio source for a one shot sound, or creates one if none are available
        //
        private AudioSource GetOneShotAudioSource()
        {
            AudioSource rtn = null;

            for (int i = 0; i < _OneShotSources.Count; i++)
            {
                if (!_OneShotSources[i].isPlaying) // Found a one-shot source that's not being used
                {
                    rtn = _OneShotSources[i];
                }
            }

            if(rtn == null) // All sources are being used
            {
                rtn = CreateAudioSource();
                _OneShotSources.Add(rtn);
            }

            return rtn;
        }
        #endregion
    }

    //
    // Data structure to store active fading information
    //
    public class FadeData
    {
        public AudioSource Source;
        public float InitialVolume;
        public float DeltaVolume;
        public float ElapsedTime; // ?Time that's passed during fade process.
        public float Duration; // Total time that the fade should take to complete.

        public FadeData(AudioSource source, float initialVolume, float targetVolume, float duration)
        {
            Source = source;
            InitialVolume = initialVolume;
            DeltaVolume = targetVolume - initialVolume;
            ElapsedTime = 0f;
            Duration = duration;
        }
    }
}