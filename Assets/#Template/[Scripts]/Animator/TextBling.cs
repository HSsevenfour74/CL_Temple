using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBling : MonoBehaviour
{
    private Text UIText;
    public float intime = 0f;
    public float outtime = 0f;
    [MinValue(0f), MaxValue(1f)] public float inrange = 1f;
    [MinValue(0f), MaxValue(1f)] public float outrange = 1f;

    private void Awake()
    {
        UIText = GetComponent<Text>();
    }

    void Start()
    {
        DG.Tweening.Sequence sq = DOTween.Sequence();
        sq.Append(UIText.DOFade(inrange, intime));
        sq.Append(UIText.DOFade(outrange, outtime));
        sq.SetLoops(-1);
    }
}
