using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConceptGames.Beta
{
    public class ConceptBeta : MonoBehaviour
    {
        public GameObject betaUI;
        void Start()
        {
            DontDestroyOnLoad(Instantiate(betaUI));
        }
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Tab))
            {
                Destroy(betaUI);
            }
        }
    }
}