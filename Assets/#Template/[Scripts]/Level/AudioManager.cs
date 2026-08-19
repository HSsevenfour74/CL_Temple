using DG.Tweening;
using System;
using UnityEngine;
using static UnityEngine.Object;

namespace ConceptGames.ConceptLineOrion.Level
{
    public static class AudioManager
    {
        public static void PlayClip(AudioClip clip)
        {
            var audioSource = new GameObject($"One shot sound: {clip.name}").AddComponent<AudioSource>();
            DontDestroyOnLoad(audioSource);
            audioSource.clip = clip;
            audioSource.volume = PlayerPrefs.GetFloat("Game_Sound_Effect");
            audioSource.Play();
            Destroy(audioSource.gameObject, clip.length);
        }

        public static AudioSource PlayTrack(AudioClip clip, float volume, bool playImmediately = true, float time = 0f)
        {
            AudioSource audioSource = new GameObject(clip.name).AddComponent<AudioSource>();
            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.time = time;
            if (SpeedSetting.Instance)
                audioSource.pitch = SpeedSetting.Instance.speed;
            else
                audioSource.pitch = 1f;
            audioSource.Play();
            if (!playImmediately) audioSource.Stop();
            return audioSource;
        }

        public static AudioSource CreateSoundtrack(AudioClip soundtrack, float volume)
        {
            var audioSource = new GameObject($"Level soundtrack: {soundtrack.name}").AddComponent<AudioSource>();
            audioSource.clip = soundtrack;
            audioSource.volume = volume;
            return audioSource;
        }

        public static float Time
        {
            get => Player.Instance.Soundtrack.time;
            set => Player.Instance.Soundtrack.time = value;
        }

        public static float Pitch
        {
            set => Player.Instance.Soundtrack.pitch = value;
        }

        public static float Volume
        {
            set => Player.Instance.Soundtrack.volume = value;
        }

        public static float Progress => Player.Instance.Soundtrack.time / Player.Instance.Soundtrack.clip.length;
        public static float GetProgress(float time) => time / Player.Instance.Soundtrack.clip.length;
        public static float Length => Player.Instance.Soundtrack.clip.length;

        public static void Stop()
        {
            Player.Instance.Soundtrack.Stop();
        }

        public static void Play()
        {
            Player.Instance.Soundtrack.Play();
        }

        public static Tween FadeOut(float volume, float duration)
        {
            return Player.Instance.Soundtrack.DOFade(volume, duration).SetEase(Ease.Linear).OnComplete(Stop);
        }
    }
}