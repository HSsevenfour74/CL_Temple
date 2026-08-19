using UnityEngine;

namespace ConceptGames.ConceptLineOrion.Level
{
    public class SyncCameraFarClipToFogEnd : MonoBehaviour
    {
        private Camera targetCamera;

        void Start()
        {
            targetCamera = Player.Instance.sceneCamera;
            targetCamera = Camera.main;
        }
        void Update()
        {
            if (targetCamera != null && RenderSettings.fog)
            {
                targetCamera.farClipPlane = RenderSettings.fogEndDistance + 50;
            }
        }
    }
}
