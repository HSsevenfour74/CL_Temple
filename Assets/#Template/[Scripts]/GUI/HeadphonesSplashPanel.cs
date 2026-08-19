using DG.Tweening;
using UnityEngine;
using System.Collections;
public class HeadphonesSplashPanel : MonoBehaviour
{
    public CanvasGroup panel;

    private void Start()
    {
        if (PlayerPrefs.GetInt("HSGS") != 1)
            StartCoroutine(DoSomethingAfterDelay());
    }
    IEnumerator DoSomethingAfterDelay()
    {
        panel.alpha = 1f;
        yield return new WaitForSeconds(2f);
        Panelalpha();
        PlayerPrefs.SetInt("HSGS", 1);
    }

    public void Panelalpha()
    {
        panel.DOFade(0f, 2f).SetEase(Ease.OutSine);
    }
}