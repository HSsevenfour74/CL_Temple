using UnityEngine;

namespace ConceptGames.ConceptLineOrion.Level.Editor
{
    public class EnterTriggerTime : MonoBehaviour
    {

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log(Player.Instance.gameTime + " "+ this.gameObject.name);
            }
        }
    }
}