using UnityEngine;
namespace ConceptGames.ConceptLineOrion.Trigger
{
    public class ClickSend : MonoBehaviour
    {
        private GameObject player;
        public Vector3 finishPosition;
        void Start()
        {
            player = GameObject.Find("LevelHolder/Objects/PlayerHolder/Player");
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
                player.transform.position = finishPosition;
        }
    }
}
