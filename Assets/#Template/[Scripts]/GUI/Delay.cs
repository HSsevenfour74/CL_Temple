using System;
using UnityEngine;
using UnityEngine.UI;

public class Delay : MonoBehaviour
{
    [Range(-1f, 1f)] public float delaynum = 0;
    public Text num;
    public Slider slider;
    public static Delay Instance { get; private set; }

    public void Awake()
    {
        delaynum = PlayerPrefs.GetFloat("DelayNum");
        num.text = Math.Round(PlayerPrefs.GetFloat("DelayNum"), 2).ToString("F2");
        slider.value = delaynum;
    }
    public void LeftButton()
    {
        delaynum -= 0.01f;
        if (delaynum <= -1f)
            delaynum = -1f;
        num.text = Math.Round(delaynum, 2).ToString("F2");
        PlayerPrefs.SetFloat("DelayNum", (float)Math.Round(delaynum, 2));
        slider.value = delaynum;
    }
    public void RightButton()
    {
        delaynum += 0.01f;
        if (delaynum >= 1f)
            delaynum = 1f;
        num.text = Math.Round(delaynum, 2).ToString("F2");
        PlayerPrefs.SetFloat("DelayNum", (float)Math.Round(delaynum, 2));
        slider.value = delaynum;
    }
    
    public void ValueChange()
    {
        delaynum = slider.value;
        num.text = Math.Round(delaynum, 2).ToString("F2");
        PlayerPrefs.SetFloat("DelayNum", (float)Math.Round(delaynum, 2));
    }
}
