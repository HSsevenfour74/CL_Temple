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

        public Text block;
        public CanvasGroup _1;
        public CanvasGroup skinchoose;
        public GameObject skinButton;

        public static StartPage Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }
        public void Hide()
        {
            block.text = block.text = (PlayerPrefs.GetInt("PlayerBlock") - 1).ToString("N0");
            PlayerPrefs.SetInt("PlayerBlock", PlayerPrefs.GetInt("PlayerBlock") - 1);
            _1.DOFade(1f, 1f);
            foreach (RectTransform l in moveLeft)
            {
                if (l.GetComponent<Button>()) l.GetComponent<Button>().interactable = false;
                l.DOAnchorPos(new Vector2(-300f, 0f), 0.4f).SetEase(Ease.InSine).OnComplete(() => { Destroy(gameObject); });
            }
            foreach (RectTransform l in moveRight)
            {
                if (l.GetComponent<Button>()) l.GetComponent<Button>().interactable = false;
                l.DOAnchorPos(new Vector2(300f, 0f), 0.4f).SetEase(Ease.InSine).OnComplete(() => { Destroy(gameObject); });
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
        public void SkinPanel()
        {
            foreach (RectTransform l in moveLeft)
            {
                l.DOAnchorPos(new Vector2(-300f, 0f), 0.5f).SetEase(Ease.InSine);
            }
            foreach (RectTransform d in moveDown)
            {
                d.DOAnchorPos(new Vector2(0f, -600f), 0.5f).SetEase(Ease.InSine).OnComplete(() =>
                {
                    skinchoose.DOFade(1f, 0.5f);
                    skinchoose.interactable = true;
                    skinchoose.blocksRaycasts = true;
                });
            }
            foreach (RectTransform u in moveRight)
            {
                u.DOAnchorPos(new Vector2(300f, 0f), 0.4f).SetEase(Ease.InSine);
            }
            foreach (RectTransform l in moveUp)
            {
                l.DOAnchorPos(new Vector2(0f, 200f), 0.4f).SetEase(Ease.InSine);
            }
        }
        public void BackSkinPanel()
        {
            skinchoose.interactable = false;
            skinchoose.blocksRaycasts = false;
            skinchoose.DOFade(0f, 0.5f).OnComplete(() =>
            {
                foreach (RectTransform l in moveLeft)
                {
                    l.DOAnchorPos(new Vector2(0f, 0f), 0.5f).SetEase(Ease.InSine);
                }
                foreach (RectTransform d in moveDown)
                {
                    d.DOAnchorPos(new Vector2(0f, 0f), 0.5f).SetEase(Ease.InSine);
                }
                foreach (RectTransform u in moveRight)
                {
                    u.DOAnchorPos(new Vector2(00f, 0f), 0.4f).SetEase(Ease.InSine);
                }
                foreach (RectTransform l in moveUp)
                {
                    l.DOAnchorPos(new Vector2(0f, 0f), 0.4f).SetEase(Ease.InSine);
                }
            });
        }
        void Start()
        {
            skinchoose.alpha = 0f;
            skinchoose.interactable = false;
            skinchoose.blocksRaycasts = false;
            foreach (Image a in uiimage)
                a.color = Player.Instance.uicolor;
            block.text = PlayerPrefs.GetInt("PlayerBlock").ToString("N0");
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