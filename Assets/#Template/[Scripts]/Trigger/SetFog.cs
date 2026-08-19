using DG.Tweening;
using ConceptGames.ConceptLineOrion.Level;
using UnityEngine;

namespace ConceptGames.ConceptLineOrion.Trigger
{
    [DisallowMultipleComponent, RequireComponent(typeof(Collider))]
    public class SetFog : MonoBehaviour
    {
        [SerializeField] private FogSettings fog;
        [SerializeField] private float duration = 2f;
        [SerializeField] private Ease ease = Ease.Linear;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player")) fog.SetFog(Player.Instance.sceneCamera, duration, ease);
        }
    }
}