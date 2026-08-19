using DG.Tweening;
using ConceptGames.ConceptLineOrion.Level;
using UnityEngine;

namespace ConceptGames.ConceptLineOrion.Trigger
{
    [DisallowMultipleComponent, RequireComponent(typeof(Collider))]
    public class SetAmbient : MonoBehaviour
    {
        [SerializeField] private AmbientSettings ambient;
        [SerializeField] private float duration = 2f;
        [SerializeField] private Ease ease = Ease.Linear;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player")) ambient.SetAmbient(duration, ease);
        }
    }
}