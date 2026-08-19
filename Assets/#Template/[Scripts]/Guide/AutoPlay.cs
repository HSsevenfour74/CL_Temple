using UnityEngine;
using UnityEngine.UI;
namespace ConceptGames.ConceptLineOrion.Level.Editor
{
    public class AutoPlay : MonoBehaviour
    {
        public GameObject autoPlay;
        public Toggle toggle;
        void Start()
        {
            autoPlay = GameObject.Find("AutoPlay");
            if (autoPlay && PlayerPrefs.GetString("UserGroup") == "2")
                autoPlay.SetActive(false);
            else
                this.gameObject.SetActive(false);
            toggle = this.GetComponent<Toggle>();
            toggle.isOn = false;
        }
        public void SetAutoPlay()
        {
            if (autoPlay != null)
                autoPlay.SetActive(toggle.isOn);
        }
    }
}