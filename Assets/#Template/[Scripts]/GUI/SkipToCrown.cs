using ConceptGames.ConceptLineOrion.Trigger;
using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ConceptGames.ConceptLineOrion.Level 
{
    public class SkipToCrown : MonoBehaviour
    {
        public List<Crown> crown = new List<Crown>();
        public List<GameObject> button = new List<GameObject>();

        public void Start()
        {
            crown = Player.Instance.crowns;
            if (crown.Count == 2)
                button[2].SetActive(false);
            if (crown.Count == 1)
            {
                button[1].SetActive(false);
                button[2].SetActive(false);
            }
        }

        public void Skip(int i)
        {
            crown[i].JumpToHere();
        }
    }
}