using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ConceptGames.ConceptLineOrion.UI
{
    [DisallowMultipleComponent]
    public class LoadingPage : MonoBehaviour
    {
        public static LoadingPage Instance { get; private set; }

        [SerializeField] private Text tip;
        [SerializeField] private Image ads;
        

        private CanvasGroup canvasGroup;
        private AsyncOperation operation;

        public Sprite[] randomObjects;
        public String[] tips;

        private void Awake()
        {
            Instance = this;
            canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Start()
        {
            canvasGroup.alpha = 0f;
        }

        public Tween Fade(float alpha, float duration, Ease ease = Ease.Linear)
        {
            return canvasGroup.DOFade(alpha, duration).SetEase(ease);
        }

        public void Load(string sceneName)
        {
            BackGround();
            Tips();
            Fade(1f, 0.4f).OnComplete(() =>
            {
                operation = SceneManager.LoadSceneAsync(sceneName);
                if (operation.isDone) operation = null;
                else operation = null;
            });
        }
        public void BackGround()
        {
            int index = UnityEngine.Random.Range(0, randomObjects.Length);
            ads.sprite = randomObjects[index];
        }

        public void Tips()
        {
            int index = UnityEngine.Random.Range(0, tips.Length);
            tip.text = "Tips:" + tips[index];
        }
    }
}