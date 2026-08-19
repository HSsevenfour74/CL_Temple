using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ButtonGroup : MonoBehaviour
{
    public GameObject[] button;
    public bool isTrue;
    public Transform foldIcon;

    public void SetFold()
    {
        isTrue = !isTrue;
        foreach (var item in button)
            item.SetActive(isTrue);
        if (isTrue)
            foldIcon.DOLocalRotate(new Vector3(0, 0, 90), 0.2f);
        else
            foldIcon.DOLocalRotate(new Vector3(0, 0, -90), 0.2f);
    }
}
