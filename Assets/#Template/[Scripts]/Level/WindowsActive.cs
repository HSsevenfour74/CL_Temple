using UnityEngine;
public class WindowsActive : MonoBehaviour
{
    void Start()
    {
        this.gameObject.SetActive(false);
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        this.gameObject.SetActive(true);
#else
        Destroy(this.gameObject);
#endif
    }
}
