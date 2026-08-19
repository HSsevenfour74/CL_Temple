using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Collections;
using DG.Tweening;

namespace ConceptGames
{
    public class CGapi : MonoBehaviour
    {
        private string url = "api.conceptgames.de5.net";
        public string version;
        public CanvasGroup canvasGroup;
        public Text text;
        public void GetNotice()
        {
            StartCoroutine(HttpGet());
        }

        IEnumerator HttpGet()
        {
            using (UnityWebRequest req = UnityWebRequest.Get(url))
            {
                yield return req.SendWebRequest();

                if (req.result == UnityWebRequest.Result.Success)
                {
                    string json = req.downloadHandler.text;
                    ApiData data = JsonUtility.FromJson<ApiData>(json);
                    Debug.Log("Notice Get Succeed");
                    if (text)
                        text.text = data.notice;
                    if (data.latest_version != version)
                    {
                        if (canvasGroup)
                        {
                            canvasGroup.interactable = true;
                            canvasGroup.blocksRaycasts = true;
                            canvasGroup.DOFade(1f, 0.3f).SetEase(Ease.OutSine);
                        }
                        if (text)
                        {
                            text.text = $"当前版本：{version}\n最新版本：{data.latest_version}";
                        }
                    }             
                }
                else
                {
                    Debug.LogError("网络请求失败：" + req.error);
                }
            }
        }
        public void ExitGame()
        {
            Application.Quit();
        }    

    }
    [Serializable]
    public class ApiData
    {
        public string notice;
        public string latest_version;
    }
}