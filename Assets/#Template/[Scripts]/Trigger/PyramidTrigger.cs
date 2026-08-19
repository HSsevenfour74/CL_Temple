using ConceptGames.ConceptLineOrion.Level;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

namespace ConceptGames.ConceptLineOrion.Trigger
{
    [DisallowMultipleComponent, RequireComponent(typeof(Collider))]
    public class PyramidTrigger : MonoBehaviour
    {
        [SerializeField, EnumToggleButtons] private TriggerType type = TriggerType.Open;

        [SerializeField, ShowIf("@type == TriggerType.Final")] private bool changeDirection = false;
        [SerializeField, ShowIf("@type == TriggerType.Final && changeDirection")] private Vector3 finalDirection = Vector3.zero;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                transform.parent.GetComponent<Pyramid>().Trigger(type);
                if (type == TriggerType.Final && changeDirection)
                {
                    Player.Instance.firstDirection = finalDirection;
                    Player.Instance.secondDirection = finalDirection;
                    Player.Instance.Turn();
                }
            }
        }
    }
}