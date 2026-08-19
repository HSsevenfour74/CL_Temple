#if UNITY_EDITOR
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using ConceptGames.ConceptLineOrion.Guide;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ConceptGames.ConceptLineOrion.Level
{
    [DisallowMultipleComponent]
    public class PlayerTrailRenderer : MonoBehaviour
    {
        [SerializeField] private GuidelineManager controller;
        [SerializeField] private int maxDistance = 36000;
        [SerializeField] private Color trailColor = Color.blue;
        [SerializeField] private Vector3 trailOffset = new(0f, 0.4f, 0f);
        [SerializeField] private bool renderTrail;
        [SerializeField] private bool renderTime;

        private List<Transform> trans = new();

        [Button("Reload Trail Data", ButtonSizes.Large)]
        private void Reload()
        {
            trans.Clear();
            OnValidate();
        }

        private void OnValidate()
        {
            if (!controller.boxHolder)
                return;
            trans = controller.boxHolder.GetComponentsInChildren<Transform>().ToList();
            trans.RemoveRange(0, 1);
        }

        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
                return;
            if (!renderTrail && !renderTime)
                return;
            if (controller == null)
            {
                renderTrail = false;
                renderTime = false;
                Debug.LogError("引导线控制器未选择");
                return;
            }

            if (controller.boxHolder == null)
            {
                renderTrail = false;
                renderTime = false;
                Debug.LogError("引导线父物体未选择");
                return;
            }

            var rendererCamera = SceneView.lastActiveSceneView.camera;
            Gizmos.color = trailColor;
            Handles.color = trailColor;
            for (var i = 0; i < trans.Count; i++)
            {
                if (!((trans[i].position - rendererCamera.transform.position).sqrMagnitude <= maxDistance))
                    continue;
                if (renderTrail)
                {
                    if (i < trans.Count - 1)
                        Handles.DrawLine(trans[i].position + trailOffset, trans[i + 1].position + trailOffset,
                            3f);

                    Gizmos.DrawCube(trans[i].position + trailOffset, Vector3.one * 0.3f);
                }

                if (!renderTime)
                    continue;
                var textureBackground = new Texture2D(1, 1);
                textureBackground.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.6f));
                textureBackground.Apply();
                var style = new GUIStyle
                {
                    normal =
                    {
                        textColor = Color.white,
                        background = textureBackground
                    },
                    fontSize = 15
                };
                if (i <= 0)
                    continue;
                var text = $"[{i}] {trans[i].GetComponent<GuidelineTap>().triggerTime}";
                Handles.Label(trans[i].position + trailOffset, text, style);
            }
        }
    }
}
#endif