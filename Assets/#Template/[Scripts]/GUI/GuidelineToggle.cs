using ConceptGames.ConceptLineOrion.Level;
using ConceptGames.ConceptLineOrion.UI;
using UnityEngine;
using UnityEngine.UI;

namespace ConceptGames.ConceptLineOrion.Guide
{
    [DisallowMultipleComponent]
    public class GuidelineToggle : MonoBehaviour
    {
        [SerializeField] private GameObject button;
        [SerializeField] private Image image;
        [SerializeField] private Sprite on;
        [SerializeField] private Sprite off;
        private GuidanceController manager;

        private void Start()
        {
            manager = FindObjectOfType<GuidanceController>();
            if (PlayerPrefs.GetInt("GuideLine") == 1)
            {
                manager.useGuideline = true;
                image.sprite = on;
                image.color = Color.white;
            }
            else
            {
                manager.useGuideline = false;
                image.sprite = off;
                image.color = Player.Instance.uicolor;
            }
            manager.SetUseGuideline();
            if (manager.boxHolder)
                return;
            GetComponent<Button>().interactable = false;
            foreach (var i in GetComponentsInChildren<Image>())
            {
                i.enabled = false;
                i.raycastTarget = false;
            }
        }
        public void SetGuideline()
        {
            if (PlayerPrefs.GetInt("GuideLine") == 0)
            {
                manager.useGuideline = true;
                PlayerPrefs.SetInt("GuideLine", 1);
                image.sprite = on;
                image.color = Color.white;
            }
            else
            {
                manager.useGuideline = false;
                PlayerPrefs.SetInt("GuideLine", 0);
                image.sprite = off;
                image.color = Player.Instance.uicolor;
            }
            manager.SetUseGuideline();
        }
    }
}