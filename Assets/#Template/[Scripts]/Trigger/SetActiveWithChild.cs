using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConceptGames.ConceptLineOrion.Trigger
{
    public class SetActiveWithChild : MonoBehaviour
    {
        public List<GameObject> gameObjects = new List<GameObject>();
        public void Trigger()
        {
            foreach (GameObject go in gameObjects)
            {
                foreach (Transform child in go.transform)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }
    }
}