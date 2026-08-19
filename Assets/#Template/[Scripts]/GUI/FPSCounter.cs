using UnityEngine;
using UnityEngine.UI;

namespace ConceptGames.ConceptLineOrion.Level
{
    public class FPSCounter : MonoBehaviour
    {
        public Text counterText;
        void Update()
        {
            counterText.text = ((int)Player.Instance.fps).ToString() + "FPS";
        }
    }
}