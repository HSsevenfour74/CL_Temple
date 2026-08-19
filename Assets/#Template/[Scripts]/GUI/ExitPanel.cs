using DG.Tweening;
using UnityEngine;

public class ExitPanel : MonoBehaviour
{
    public CanvasGroup exitPanel;

    void Start()
    {
        exitPanel = this.GetComponent<CanvasGroup>();
        OpenPanel();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            exitPanel.blocksRaycasts = true;
            exitPanel.interactable = true;
            exitPanel.DOFade(1f, 0.3f).SetEase(Ease.OutSine);
        }
    }
    public void OpenPanel()
    {
        exitPanel.blocksRaycasts = false;
        exitPanel.interactable = false;
        exitPanel.alpha = 0f;
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
