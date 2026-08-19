using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ConceptGames.ConceptLineOrion.UI
{
    public class SystemPrompt : MonoBehaviour
    {
        public RectTransform achievement;
        public RectTransform prompt;
        public RectTransform clouddata;
        public RectTransform popup;
        public List<GameObject> promptResponse = new List<GameObject>();
        public List<GameObject> cloudResponse = new List<GameObject>();

        public static SystemPrompt Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        public void PopUp(string title,string content,Button yes)
        {
            Text titleText, contentText;
            titleText = popup.GetComponentInChildren<Text>();
            contentText = popup.GetComponentInChildren<Text>();
            titleText.text = title;
            contentText.text = content;
            yes.onClick.AddListener(() => { });

        }
        public void OnAchievementSucceed(string i)
        {
            Text achievementText = achievement.GetComponentInChildren<Text>();
            achievementText.text = i;
            achievement.DOKill();
            achievement.GetComponentInChildren<RectTransform>().DOAnchorPos(new Vector2(0, -100), 0.5f).OnComplete(() =>
            { StartCoroutine(PopupAndDestroy(achievement, new Vector2(0, -100), new Vector2(0, 100), 0.5f, 2f)); });
        }
        public void TipPos(string i,int a)
        {
            foreach (var b in promptResponse)
                b.SetActive(false);
            promptResponse[a].SetActive(true);
            Text promptText;
            promptText = prompt.GetComponentInChildren<Text>();
            promptText.text = i;
            prompt.DOKill();
            prompt.DOAnchorPos(new Vector2(0,-100),0.5f).OnComplete(() =>
            { StartCoroutine(PopupAndDestroy(prompt, new Vector2(0, -100), new Vector2(0, 100), 0.5f, 2f)); });
        }
        public void CloudPos(int a)
        {
            foreach (var b in cloudResponse)
                b.SetActive(false);
            cloudResponse[a].SetActive(true);
            clouddata.DOKill();
            clouddata.DOAnchorPos(new Vector2(-100, -220), 0.5f).OnComplete(() =>
            { StartCoroutine(HoldCloud()); });
        }
        public IEnumerator HoldCloud()
        {
            yield return new WaitForSeconds(2);
            clouddata.DOAnchorPos(new Vector2(250, -220), 0.5f);
        }
        private IEnumerator PopupAndDestroy(RectTransform targetRect, Vector2 showPos, Vector2 hidePos,
                                    float showDuration, float holdTime)
        {
            yield return new WaitForSeconds(holdTime);
            targetRect.DOAnchorPos(hidePos, showDuration);
        }
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            StopAllCoroutines();

            achievement.anchoredPosition = new Vector2(0, 100);
            prompt.anchoredPosition = new Vector2(0, 100);
            clouddata.anchoredPosition = new Vector2(250, -220);
        }
    }
}
