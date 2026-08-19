using UnityEngine;

namespace ConceptGames.ConceptLineOrion.UI
{
    public class BackMain : MonoBehaviour
    {
        public void MainMenu()
        {
            if (LoadingPage.Instance)
            {
                LoadingPage.Instance.Load("Choose Level");
            }
        }
    }
}
