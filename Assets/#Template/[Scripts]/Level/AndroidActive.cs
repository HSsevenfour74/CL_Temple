using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndroidActive : MonoBehaviour
{
    public void Start()
    { 
        this.gameObject.SetActive(false);
#if UNITY_ANDROID
        this.gameObject.SetActive(true);
#else
        Destroy(this.gameObject);
#endif
    }
}
