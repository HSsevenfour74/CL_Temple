using ConceptGames.ConceptLineOrion.Level;
using ConceptGames.ConceptLineOrion.Trigger;
using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using AssetKits.ParticleImage;
using System.Collections;
using System;

namespace ConceptGames.ConceptLineOrion.UI
{
    public class LevelUI : MonoBehaviour
    {
        public static LevelUI Instance { get; private set; }

        [Title("Normal")]
        [SerializeField] public Text title;
        [SerializeField] public Text percentage;
        [SerializeField] public Text block;
        [SerializeField] private Text coin;
        [SerializeField] private Text getcoin;
        [SerializeField] private Text perfect;
        [SerializeField] private Text newbest;
        [SerializeField] private CanvasGroup textSide;
        [SerializeField] private Image background;
        [SerializeField] private RectTransform barFill;
        [SerializeField] private RectTransform moveUpPart;
        [SerializeField] private RectTransform moveDownPart;
        [SerializeField] private List<CanvasGroup> normalAlpha = new List<CanvasGroup>();
        [SerializeField] private List<Image> crownInfill = new List<Image>();
        [SerializeField] public Vector2 m_Scale;
        [SerializeField] private List<AudioClip> crownSount = new List<AudioClip>();
        [SerializeField] private List<AudioClip> effectSount = new List<AudioClip>();
        [SerializeField] private List<ParticleImage> crownParticles = new List<ParticleImage>();
        [SerializeField] private List<Button> buttons = new List<Button>();
        [SerializeField] private List<Image> crownback = new List<Image>();
        [SerializeField] private List<ParticleImage> newBestParticles = new List<ParticleImage>();
        [SerializeField] private CanvasGroup getCoinPanel;
        [SerializeField] private Text blockPanel;
        [Title("Revive")]
        [SerializeField] private Text percentageRevive;
        [SerializeField] private RectTransform barFillRevive;
        [SerializeField] private Image hideScreenImage;
        [SerializeField] private CanvasGroup reviveAlpha;


        [Title("Other")]
        private Player player;
        private float progress;

        private void Start()
        {
            perfect.gameObject.SetActive(false);
            newbest.gameObject.SetActive(false);
            textSide.DOFade(0, 0);
        }
        private void Awake()
        {
            Instance = this;
            player = Player.Instance;
            moveUpPart.anchoredPosition = new Vector2(0f, -1080f);
            moveDownPart.anchoredPosition = new Vector2(0f, 1080f);
            foreach (var group in normalAlpha) group.alpha = 0f;
            reviveAlpha.alpha = 0f;
            reviveAlpha.interactable = false;
            reviveAlpha.blocksRaycasts = false;
            background.color = Color.clear;
            foreach (var b in buttons) b.interactable = false;
        }

        internal void NormalPage(float percent, int blockCount, int crownCount)
        {
            progress = percent;
            ShowPage(true, percent, blockCount, crownCount);
        }

        internal void RevivePage(float percent)
        {
            progress = percent;
            ShowPage(false, percent);
        }

        internal void ShowPage(bool normal, float percent, int blockCount = 0, int crownCount = 0)
        {
            Ease movementCurve = Ease.InCubic;
            float movementY = 120F;
            Cursor.visible = true;
            if (normal)
            {
                //动画
                moveUpPart.DOAnchorPos(Vector2.zero, 0.4f).SetEase(Ease.OutSine);
                moveDownPart.DOAnchorPos(Vector2.zero, 0.4f).SetEase(Ease.OutSine);
                background.DOFade(0.64f, 0.4f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    foreach (var b in buttons) b.interactable = true;
                });
                foreach (var c in normalAlpha) c.DOFade(1f, 0.4f).SetEase(Ease.Linear);
                barFill.sizeDelta = new Vector2(0f, 18f) + new Vector2(490f * percent, 0f);

                //完美通关
                if (crownCount == player.levelData.crownnum && blockCount == player.levelData.blocknum && (int)(percent * 100f) == 100)
                {
                    textSide.DOFade(1, 0.5f);
                    newbest.gameObject.SetActive(false);
                    perfect.gameObject.SetActive(true);
                    foreach (var i in newBestParticles)
                        i.Play();
                }
                //UI文字
                percentage.text = (int)(percent * 100f) + "%";
                block.text = $"{blockCount}/{player.levelData.blocknum}";
                title.text = player.levelData.levelTitle;
                getcoin.text = "+" + ((int)(percent * 200) + (crownCount * 100) + (blockCount * 50)).ToString("N0");
                blockPanel.text = PlayerPrefs.GetInt("PlayerBlock").ToString("N0");

                //货币
                int currentNumber = PlayerPrefs.GetInt("PlayerCoin");
                DOTween.To(() => currentNumber, x => currentNumber = x, currentNumber + (int)(percent * 200) + (crownCount * 100) + (blockCount * 50), 2.5f)
                .OnUpdate(() =>
                {
                    coin.text = currentNumber.ToString("N0");
                    PlayerPrefs.SetInt("PlayerCoin", currentNumber);
                })
                .SetEase(Ease.OutQuad).OnComplete(() =>
                {
                    getCoinPanel.DOFade(0f, 2.5f);
                });

                //皇冠
                AudioManager.PlayClip(effectSount[crownCount]);
                if (Player.Instance.levelData.crownnum == 0)
                {
                    foreach (Image a in crownback)
                    {
                        a.enabled = false;
                    }
                }
                else if (crownCount >= 1)
                {
                    crownInfill[0].DOFade(1f, 0.6f).SetEase(Ease.Linear);
                    (crownInfill[0].transform as RectTransform).anchoredPosition = new(-220, movementY);
                    (crownInfill[0].transform as RectTransform).DOAnchorPos(new(-150, 0), 0.6f).SetEase(movementCurve);
                    crownParticles[0].Play();

                    crownInfill[0].transform.DOScale(Vector3.one, 0.6f).SetEase(Ease.InCubic).OnComplete(() =>
                    {
                        AudioManager.PlayClip(crownSount[crownCount - 1]);
                        if (crownCount >= 2)
                        {
                            crownInfill[1].DOFade(1f, 0.6f).SetEase(Ease.Linear);
                            (crownInfill[1].transform as RectTransform).anchoredPosition = new(0, movementY);
                            (crownInfill[1].transform as RectTransform).DOAnchorPos(Vector2.zero, 0.6f).SetEase(movementCurve);
                            crownParticles[1].Play();
                            crownInfill[1].transform.DOScale(Vector3.one, 0.6f).SetEase(Ease.InCubic).OnComplete(() =>
                            {
                                if (crownCount >= 3)
                                {
                                    crownInfill[2].DOFade(1f, 0.6f).SetEase(Ease.Linear);
                                    (crownInfill[2].transform as RectTransform).anchoredPosition = new(220, movementY);
                                    (crownInfill[2].transform as RectTransform).DOAnchorPos(new(150, 0), 0.6f).SetEase(movementCurve);
                                    crownParticles[2].Play();
                                    crownInfill[2].transform.DOScale(Vector3.one, 0.6f).SetEase(Ease.InCubic).OnComplete(() =>
                                    {
                                    });
                                }
                            });
                        }
                    });
                }
            }
            else           //复活
            {
                background.DOFade(0.64f, 0.4f).SetEase(Ease.Linear);
                reviveAlpha.DOFade(1f, 0.4f).SetEase(Ease.Linear);
                reviveAlpha.blocksRaycasts = true;
                reviveAlpha.interactable = true;
                barFillRevive.sizeDelta = new Vector2(0f, 18f) + new Vector2(490f * percent, 0f);
                percentageRevive.text = ((int)(percent * 100f)).ToString() + "%";
            }
        }

        public void ReloadScene()
        {
            foreach (var b in buttons) b.interactable = false;
            if (LoadingPage.Instance) LoadingPage.Instance.Load(SceneManager.GetActiveScene().name);
            else SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void RevivePlayer()
        {
            if (player.currentCheckpoint.GetComponent<Checkpoint>()) player.currentCheckpoint.GetComponent<Checkpoint>().Revival();
            else if (player.currentCheckpoint.GetComponent<Crown>()) player.currentCheckpoint.GetComponent<Crown>().Revival();
        }

        public void CancelRevive()
        {
            reviveAlpha.DOFade(0f, 0.4f).SetEase(Ease.Linear);
            reviveAlpha.blocksRaycasts = false;
            reviveAlpha.interactable = false;
            NormalPage(progress, player.BlockCount, player.CrownCount);
            foreach (var c in normalAlpha) c.DOFade(1f, 0.4f).SetEase(Ease.Linear);
        }

        internal void HideScreen(Color color, float duration, UnityAction fadeIn, UnityAction fadeOut)
        {
            foreach (var b in buttons) b.interactable = false;
            hideScreenImage.color = new Color(color.r, color.g, color.b, 0f);
            hideScreenImage.DOFade(1f, duration).SetEase(Ease.Linear).OnComplete(() =>
            {
                ResetUI();
                try
                {
                    fadeIn.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogError($"FadeIn回调执行出错: {e}");
                    Debug.LogError($"错误信息: {e.Message}");
                    Debug.LogError($"堆栈跟踪: {e.StackTrace}");
                    hideScreenImage.DOFade(0f, duration).SetEase(Ease.Linear).OnComplete(fadeOut.Invoke);
                    SystemPrompt.Instance.TipPos("ReviveError(" + e.StackTrace + ")", 2);
                }
                hideScreenImage.DOFade(0f, duration).SetEase(Ease.Linear).OnComplete(fadeOut.Invoke);
            });
        }
        private void ResetUI()
        {
            moveUpPart.anchoredPosition = new Vector2(0f, -250f);
            moveDownPart.anchoredPosition = new Vector2(0f, 430f);
            foreach (var group in normalAlpha) group.alpha = 0f;
            reviveAlpha.alpha = 0f;
            reviveAlpha.blocksRaycasts = false;
            reviveAlpha.interactable = false;
            background.color = Color.clear;
            foreach (var b in buttons) b.interactable = false;
        }
        public void MainMenu()
        {
            if (LoadingPage.Instance)
                LoadingPage.Instance.Load("Choose Level");
        }
    }
}