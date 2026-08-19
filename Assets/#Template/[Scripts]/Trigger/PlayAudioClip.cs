using ConceptGames.ConceptLineOrion.Level;
using UnityEngine;

namespace ConceptGames.ConceptLineOrion.Trigger
{
    [DisallowMultipleComponent]
    public class PlayAudioClip : MonoBehaviour
    {
        [SerializeField] private AudioClip clip;
        [SerializeField] private bool triggeredByTrigger = true;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && triggeredByTrigger) PlayClip(clip);
        }

        public void PlayClip(AudioClip a)
        {
            AudioManager.PlayClip(a);
        }
    }
}