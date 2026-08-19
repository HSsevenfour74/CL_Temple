using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConceptGames.ConceptLineOrion.UI
{
    public class RandomAudio : MonoBehaviour
    {
        public AudioClip[] audioClips;
        private AudioSource audioSource;
        private void Start()
        {
            int index = Random.Range(0, audioClips.Length);
            audioSource = GetComponent<AudioSource>();
            audioSource.clip = audioClips[index];
            audioSource.Play();
        }

        public void ChangeMusic()
        {

        }
    }
}
