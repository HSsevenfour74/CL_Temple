using DG.Tweening;
using ConceptGames.ConceptLineOrion.Level;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace ConceptGames.ConceptLineOrion.Trigger
{
    [DisallowMultipleComponent, RequireComponent(typeof(Collider))]
    public class SetMaterialColor : MonoBehaviour
    {
        [SerializeField, TableList] private List<SingleColor> colors = new List<SingleColor>();
        [SerializeField] private float duration = 2f;
        [SerializeField] private Ease ease = Ease.Linear;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player")) foreach (SingleColor s in colors) s.SetColor(duration,ease);
        }
    }
}