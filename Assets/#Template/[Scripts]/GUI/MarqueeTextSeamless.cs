using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MarqueeTextSeamless : MonoBehaviour
{
    [Header("滚动总容器（放两个Text的父物体）")]
    public RectTransform textParent;

    [Header("滚动速度")]
    public float duration = 8f;

    private float totalWidth;

    void Start()
    {
        // 计算单个文本宽度
        RectTransform textA = textParent.GetChild(0).GetComponent<RectTransform>();
        totalWidth = textA.rect.width;

        // DOTween 匀速向左移动，移动距离等于单个文本宽度
        textParent.DOAnchorPosX(-totalWidth, duration)
            .SetEase(Ease.Linear) // 匀速，最重要
            .SetLoops(-1, LoopType.Restart); // 无限循环，走完立刻复位
    }

    // 外部控制暂停/继续
    public void PauseScroll()
    {
        textParent.DOPause();
    }

    public void ResumeScroll()
    {
        textParent.DOPlay();
    }
}