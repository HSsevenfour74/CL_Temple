using ConceptGames.ConceptLineOrion.Level;
using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSound : MonoBehaviour
{
    public List<AudioClip> audioClipsSword = new List<AudioClip>();
    public List<AudioClip> audioClipsVoice = new List<AudioClip>();
    public void OnTouch()
    {
        AudioManager.PlayClip(audioClipsVoice[Random.Range(0, audioClipsVoice.Count)]);
        AudioManager.PlayClip(audioClipsSword[Random.Range(0, audioClipsSword.Count)]);
    }
}
