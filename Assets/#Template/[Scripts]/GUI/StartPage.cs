using ConceptGames.ConceptLineOrion.Level;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ConceptGames.ConceptLineOrion.UI
{
    [DisallowMultipleComponent]
    public class StartPage : MonoBehaviour
    {
        [SerializeField] private List<RectTransform> moveDown;
        [SerializeField] private List<RectTransform> moveLeft;
        [SerializeField] private List<RectTransform> moveRight;
        [SerializeField] private List<RectTransform> moveUp;
        [SerializeField] private List<Image> uiimage = new List<Image>();

        [SerializeField]
        private float upY = 100f;
        [SerializeField]
        private float speed = 1f;
        private bool state;
        [SerializeField]
        private RectTransform RectT;
        [SerializeField]
        private CanvasGroup CanvasG;

        public static StartPage Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }
        public void Hide()
        {
            foreach (RectTransform l in moveLeft)
            {
                if (l.GetComponent<Button>()) l.GetComponent<Button>().interactable = false;
                l.DOAnchorPos(new Vector2(-300f, 0f), 0.4f).SetEase(Ease.InSine);
            }
            foreach (RectTransform l in moveRight)
            {
                if (l.GetComponent<Button>()) l.GetComponent<Button>().interactable = false;
                l.DOAnchorPos(new Vector2(300f, 0f), 0.4f).SetEase(Ease.InSine);
            }
            foreach (RectTransform d in moveDown)
            {
                if (d.GetComponent<Button>()) d.GetComponent<Button>().interactable = false;
                d.DOAnchorPos(new Vector2(0f, -600f), 0.4f).SetEase(Ease.InSine);
            }
            foreach (RectTransform u in moveUp)
            {
                if (u.GetComponent<Toggle>()) u.GetComponent<Toggle>().interactable = false;
                u.DOAnchorPos(new Vector2(0f, 200f), 0.4f).SetEase(Ease.InSine);
            }
        }
        void Start()
        {
            foreach (Image a in uiimage)
                a.color = Player.Instance.uicolor;
        }
        private void Update()
        {
            Vector2 anchoredPosition = RectT.anchoredPosition;
            if (state)
            {
                if (anchoredPosition.y < upY)
                {
                    anchoredPosition.y += 1f * speed;
                }
                else
                {
                    state = !state;
                }
            }
            else if (anchoredPosition.y > 0f)
            {
                anchoredPosition.y -= 1f * speed;
            }
            else
            {
                state = !state;
            }
            CanvasG.alpha = (upY - anchoredPosition.y) / upY;
            RectT.anchoredPosition = anchoredPosition;
        }
    }
}