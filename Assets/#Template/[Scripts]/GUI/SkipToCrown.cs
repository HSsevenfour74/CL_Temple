using ConceptGames.ConceptLineOrion.Trigger;
using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConceptGames.ConceptLineOrion.Level 
{
    public class SkipToCrown : MonoBehaviour
    {
        private List<Crown> crown = new List<Crown>();
        public List<GameObject> button = new List<GameObject>();

        public void Start()
        {
            crown = Player.Instance.crowns;
        }

        public void Skip(int i)
        {
            crown[i].JumpToHere();
        }
    }
}