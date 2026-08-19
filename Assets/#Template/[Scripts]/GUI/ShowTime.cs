using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowTime : MonoBehaviour
{
    /// 显示时间的文本
    public Text TxtCurrentTime;
    /// 是否允许显示时间
    public bool isShowTime = true;

    void Update()
    {
        if (!isShowTime) return;
        //获取系统当前时间
        DateTime NowTime = DateTime.Now.ToLocalTime();
        //将时间格式化输出
        TxtCurrentTime.text = NowTime.ToString("yyyy-MM-dd HH:mm:ss");
    }
}