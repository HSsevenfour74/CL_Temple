using UnityEngine;

namespace ConceptGames.ConceptLineOrion.Level
{
    public class ChangeRotato : MonoBehaviour
    {
        public float speed = 5000000;
        public GameObject player;
        void Update()
        {
            transform.rotation = Quaternion.Lerp(this.transform.rotation, player.transform.rotation, Time.deltaTime * speed);
        }
    }
}