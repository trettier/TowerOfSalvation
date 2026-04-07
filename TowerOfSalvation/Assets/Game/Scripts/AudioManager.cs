using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TowerOfSalvation.Audio
{
    public class AudioManager : Singleton<AudioManager>
    {
        [Header("Настройки")]
        [SerializeField] private AudioSource _sfxSource;
        [SerializeField] private AudioSource _musicSource;

        [Header("Громкость")]
        [Range(0f, 1f)]
        [SerializeField] private float _masterVolume = 1f;
        [Range(0f, 1f)]
        [SerializeField] private float _musicVolume = 0.8f;
        [Range(0f, 1f)]
        [SerializeField] private float _sfxVolume = 1f;

        [Header("Звуки")]
        [SerializeField] private AudioClip _buttonClick;
        [SerializeField] private AudioClip _coinCollect;
        [SerializeField] private AudioClip _gameMusic;

        [Header("Группы звуков")]
        [SerializeField] private List<AudioClip> _footstepSounds;

        private bool _isMuted;
        private float _cachedMasterVolume;
        private Dictionary<AudioClip, float> _lastPlayTimes = new Dictionary<AudioClip, float>();
        private float _cooldownTime = 0.05f;
        private Coroutine _musicFadeCoroutine;

        protected override void Awake()
        {
            base.Awake();
            InitializeSources();
            UpdateVolumes();
        }

        private void Start()
        {
            PlayMusic(_gameMusic, true);
        }

        private void InitializeSources()
        {
            if (_sfxSource == null)
            {
                GameObject sfxObj = new GameObject("SFXSource");
                sfxObj.transform.parent = transform;
                _sfxSource = sfxObj.AddComponent<AudioSource>();
                _sfxSource.playOnAwake = false;
            }

            if (_musicSource == null)
            {
                GameObject musicObj = new GameObject("MusicSource");
                musicObj.transform.parent = transform;
                _musicSource = musicObj.AddComponent<AudioSource>();
                _musicSource.loop = true;
                _musicSource.playOnAwake = false;
            }
        }

        private void UpdateVolumes()
        {
            if (_musicSource != null)
                _musicSource.volume = _masterVolume * _musicVolume;

            if (_sfxSource != null)
                _sfxSource.volume = _masterVolume * _sfxVolume;
        }

        public void PlayButtonClick() => PlaySFX(_buttonClick);
        public void PlayCoinCollect() => PlaySFX(_coinCollect);
        public void PlayFootstep() => PlayRandomSFX(_footstepSounds);

        public void PlaySFX(AudioClip clip)
        {
            if (clip == null || _sfxSource == null || _isMuted) return;

            float currentTime = Time.time;

            if (_lastPlayTimes.ContainsKey(clip) && currentTime - _lastPlayTimes[clip] < _cooldownTime)
                return;

            _sfxSource.PlayOneShot(clip);
            _lastPlayTimes[clip] = currentTime;
        }

        private void PlayRandomSFX(List<AudioClip> clips)
        {
            if (clips == null || clips.Count == 0) return;
            PlaySFX(clips[Random.Range(0, clips.Count)]);
        }

        public void PlayMusic(AudioClip clip, bool loop = true)
        {
            if (clip == null || _musicSource == null) return;

            _musicSource.clip = clip;
            _musicSource.loop = loop;
            _musicSource.Play();
        }

        public void StopMusic()
        {
            if (_musicSource != null)
                _musicSource.Stop();
        }

        public void SetMasterVolume(float volume)
        {
            _masterVolume = Mathf.Clamp01(volume);
            UpdateVolumes();
        }

        public void SetMusicVolume(float volume)
        {
            _musicVolume = Mathf.Clamp01(volume);
            UpdateVolumes();
        }

        public void SetSFXVolume(float volume)
        {
            _sfxVolume = Mathf.Clamp01(volume);
            UpdateVolumes();
        }

        public void ToggleMute()
        {
            _isMuted = !_isMuted;

            if (_isMuted)
            {
                _cachedMasterVolume = _masterVolume;
                _masterVolume = 0f;
            }
            else
            {
                _masterVolume = _cachedMasterVolume;
            }

            UpdateVolumes();
        }

        public static void PlayOneShot(AudioSource source, AudioClip clip)
        {
            if (source != null && clip != null && instance != null && !instance._isMuted)
            {
                source.PlayOneShot(clip);
            }
        }

        private void StartMusicFade(AudioClip newClip, bool loop, float fadeDuration)
        {
            if (_musicFadeCoroutine != null)
                StopCoroutine(_musicFadeCoroutine);

            _musicFadeCoroutine = StartCoroutine(FadeMusicCoroutine(newClip, loop, fadeDuration));
        }

        private IEnumerator FadeMusicCoroutine(AudioClip newClip, bool loop, float fadeDuration)
        {
            float startVolume = _musicSource.volume;
            float targetVolume = newClip != null ? _masterVolume * _musicVolume : 0f;
            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeDuration;
                _musicSource.volume = Mathf.Lerp(startVolume, 0f, t);
                yield return null;
            }

            _musicSource.volume = 0f;

            if (newClip != null)
            {
                _musicSource.clip = newClip;
                _musicSource.loop = loop;
                _musicSource.Play();

                elapsed = 0f;
                while (elapsed < fadeDuration)
                {
                    elapsed += Time.deltaTime;
                    float t = elapsed / fadeDuration;
                    _musicSource.volume = Mathf.Lerp(0f, targetVolume, t);
                    yield return null;
                }

                _musicSource.volume = targetVolume;
            }
            else
            {
                _musicSource.Stop();
            }

            _musicFadeCoroutine = null;
        }
    }
}