using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace ConceptGames.ConceptLineOrion.UI
{
    public class UIFadeManager : MonoBehaviour
    {
        public static UIFadeManager Instance { get; private set; }

        public Event OnOpenPanel;
        private Button back;
        private void Awake()
        {
            Instance = this;
        }
        public void OpenPanel(CanvasGroup a)
        {
            a.interactable = true;
            a.blocksRaycasts = true;
            a.DOFade(1f, 0.3f).SetEase(Ease.OutSine);
            var b = a.GetComponent<RectTransform>();
            DOOffsetTop(b, 0);
            DOOffsetBottom(b, 0);
            DOOffsetLeft(b, 0);
            DOOffsetRight(b, 0);
            back = FindChildRecursive(a.GetComponent<RectTransform>(), "Image - NEXTButton").GetComponent<Button>();
        }
        public void OffPanel(CanvasGroup a)
        {
            a.interactable = false;
            a.blocksRaycasts = false;
            a.DOFade(0f, 0.3f).SetEase(Ease.OutSine);
            var b = a.GetComponent<RectTransform>();
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (back)
                    back.onClick.Invoke();
            }
        }

        public void DOOffsetTop(RectTransform a,int value)
        {
            DOTween.To(() => -a.offsetMax.y, v => a.offsetMax = new Vector2(a.offsetMax.x, -v), value, 0f);
        }

        public void DOOffsetBottom(RectTransform a,int value)
        {
            DOTween.To(() => a.offsetMin.y, v => a.offsetMin = new Vector2(a.offsetMin.x, v), value, 0f);
        }

        public void DOOffsetLeft(RectTransform a, int value)
        {
            DOTween.To(
            () => a.offsetMin.x,
            v => a.offsetMin = new Vector2(v, a.offsetMin.y), 
            value, 0.2f);
        }

        public void DOOffsetRight(RectTransform a, int value)
        {
            DOTween.To(
            () => -a.offsetMax.x,
            v => a.offsetMax = new Vector2(-v, a.offsetMax.y), 
            value, 0.2f);
        }

        public static Transform FindChildRecursive(Transform parent, string name)
        {
            foreach (Transform child in parent)
            {
                if (child.name == name)
                    return child;

                Transform result = FindChildRecursive(child, name);
                if (result != null)
                    return result;
            }
            return null;
        }
    }
}
