using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConceptGames.ConceptLineOrion.UI
{
    public class ShareButton : MonoBehaviour
    {
        public GameObject ScreenShotController;

        public void OnClicked()
        {
            if(!Share.Instance)
            Instantiate(ScreenShotController);
            Share.Instance.TakeScreenShot();
            StartCoroutine(DelayTake());
        }
        public IEnumerator DelayTake()
        {
            yield return new WaitForSeconds(0.1f);
            Share.Instance.TakeScreenShot();
            yield return new WaitForSeconds(0.1f);
            Share.Instance.ResetCamera();

        }
    }
}