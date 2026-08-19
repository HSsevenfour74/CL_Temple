using ConceptGames.ConceptLineOrion.Level;
using UnityEngine;

namespace ConceptGames.ConceptLineOrion.Trigger
{
    public class SetCameraFarClip : MonoBehaviour
    {
        [Min(0)] public int farWin;
        [Min(0)] public int farAnd;
        public Camera targetCamera;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (!targetCamera)
                    targetCamera = Player.Instance.sceneCamera;
#if UNITY_STANDALONE_WIN
                    targetCamera.farClipPlane = farWin;
#elif UNITY_ANDROID
                    targetCamera.farClipPlane = farAnd;
#endif
            }
            else return;
        }
    }
}
