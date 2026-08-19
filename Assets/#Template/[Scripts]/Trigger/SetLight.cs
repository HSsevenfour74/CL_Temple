using DG.Tweening;
using ConceptGames.ConceptLineOrion.Level;
using UnityEngine;

namespace ConceptGames.ConceptLineOrion.Trigger
{
    [DisallowMultipleComponent, RequireComponent(typeof(Collider))]
    public class SetLight : MonoBehaviour
    {
        [SerializeField] private new LightSettings light;
        [SerializeField] private float duration = 2f;
        [SerializeField] private Ease ease = Ease.Linear;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player")) light.SetLight(Player.Instance.sceneLight, duration, ease);
        }
    }
}