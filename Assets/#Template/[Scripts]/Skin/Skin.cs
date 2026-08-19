using UnityEngine;
using UnityEngine.UI;
using ConceptGames.ConceptLineOrion.Level;
using ConceptGames.ConceptLineOrion.Trigger;

namespace ConceptGames.ConceptLineOrion.UI
{
    public class Skin : MonoBehaviour
    {
        private GameObject headphone;

        public Text skintext;
        public bool linebody;

        public static Skin Instance { get; private set; }
        private void Start()
        {
            headphone = GameObject.Find("LevelHolder/Objects/PlayerHolder/Player/Headphone");
            SetSkin();
        }
        public void SetSkin()
        {
            switch (PlayerPrefs.GetString("SkinID"))
            {
                case "Normal":
                    OpenNormal();
                    break;
                case "Headphone":
                    OpenHeadphone();
                    break;
                default:
                    OpenNormal();
                    break;
            }
        }
        public void OffAllSkin()
        {
            headphone.SetActive(false);
        }
        public void OpenNormal()
        {
            OffAllSkin();
            ShowLineBody(true);     
            PlayerPrefs.SetString("SkinID", "Normal");
            skintext.text = "Normal";   
        }
        public void OpenHeadphone()
        {
            OffAllSkin();
            ShowLineBody(true);
            headphone.SetActive(true);  
            PlayerPrefs.SetString("SkinID", "Headphone");
            skintext.text = "Headphone";
        }
        

        public void ShowLineBody(bool active)
        {
            Player.Instance.tailPrefab = Resources.Load<GameObject>("Prefabs/Tail");
        }
    }
}
