using ConceptGames.ConceptLineOrion.Animated;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ConceptGames.ConceptLineOrion.Trigger
{
    public class LocalPosTrigger : MonoBehaviour
    {
        public GameObject aniobj;

        public void OnTriggerEnter(Collider other)
        {
            if (aniobj.GetComponent<LocalPosAnimator>())
                aniobj.GetComponent<LocalPosAnimator>().Trigger();
        }
    }
}