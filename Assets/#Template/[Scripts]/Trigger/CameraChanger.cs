using UnityEngine;
using DG.Tweening;
using ConceptGames.ConceptLineOrion.Level;
using Sirenix.OdinInspector;

namespace ConceptGames.ConceptLineOrion.Trigger
{
    public class CameraChanger : MonoBehaviour
    {
        public enum ClearFlags { Skybox, SolidColor }
        public enum Projection { Perspective, Orthographic }
        [Title("绑定摄像机")] public UnityEngine.Camera Camera;

        [Space(10)]
        [Title("摄像机选项设置")]
        [EnumToggleButtons] public ClearFlags clearFlags = ClearFlags.SolidColor;
        [ShowIf("clearFlags", ClearFlags.SolidColor)] public Color BackgroundColor = Color.white;
        [EnumToggleButtons] public Projection projection = Projection.Perspective;

        [ShowIf("projection", Projection.Perspective)]
        [Tooltip("仅Perspective像机下可使用")]
        [Range(0f, 179f)]
        public float FieldOfView = 60f;

        [ShowIf("projection", Projection.Orthographic)]
        [Tooltip("仅Orthographic像机下可使用")]
        public float CameraSize = 17.5f;

        [Space(10)]
        [Title("渐变属性设置")]
        public Ease Ease = Ease.InOutSine;
        public float Time;

        [Button("通过摄像机进行赋值")]
        void Get()
        {
            if (Camera != null)
            {
                clearFlags = Camera.clearFlags == CameraClearFlags.Skybox ? ClearFlags.Skybox : ClearFlags.SolidColor;
                BackgroundColor = Camera.backgroundColor;
                projection = Camera.orthographic ? Projection.Orthographic : Projection.Perspective;
                FieldOfView = Camera.fieldOfView;
                CameraSize = Camera.orthographicSize;
            }
            else
            {
                Debug.LogWarning("未选择摄像机！");
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Player>())
            {
                switch (clearFlags)
                {
                    case ClearFlags.Skybox:
                        Camera.clearFlags = CameraClearFlags.Skybox;
                        break;
                    case ClearFlags.SolidColor:
                        Camera.clearFlags = CameraClearFlags.SolidColor;
                        break;
                }
                DOTween.To(() => Camera.backgroundColor, a => Camera.backgroundColor = a, BackgroundColor, Time).SetEase(Ease);
                switch (projection)
                {
                    case Projection.Perspective:
                        Camera.orthographic = false;
                        break;
                    case Projection.Orthographic:
                        Camera.orthographic = true;
                        break;
                }
                DOTween.To(() => Camera.fieldOfView, a => Camera.fieldOfView = a, FieldOfView, Time).SetEase(Ease);
                DOTween.To(() => Camera.orthographicSize, a => Camera.orthographicSize = a, CameraSize, Time).SetEase(Ease);
            }
        }
    }
}