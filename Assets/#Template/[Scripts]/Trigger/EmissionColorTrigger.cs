using DG.Tweening;
using UnityEngine;

namespace ConceptGames.ConceptLineOrion.Level
{
    [RequireComponent(typeof(Collider))]
    public class EmissionColorTrigger : MonoBehaviour
    {
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

        [Header("材质设置")]
        public Material targetMaterial;  // 需要修改的Standard材质

        [Header("颜色设置")]
        [ColorUsage(true, true)]
        public Color initialColor = Color.black;  // 初始颜色
        [ColorUsage(true, true)]
        public Color targetColor = Color.white * 2f;  // 目标颜色

        [Header("时间设置")]
        [Range(0.1f, 10f)]
        public float transitionTime = 1f;  // 过渡时间

        private Tween _colorTween;
        private Color _currentColor;

        private void Start()
        {
            // 初始化材质
            if (targetMaterial)
            {
                targetMaterial.EnableKeyword("_EMISSION");
                targetMaterial.SetColor(EmissionColor, initialColor);
                _currentColor = initialColor;
            }
            else
            {
                Debug.LogError("请指定需要修改的材质！");
                enabled = false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.GetComponent<Player>()) return;
            _colorTween?.Kill();
            _colorTween = DOTween.To(() => _currentColor, color =>
            {
                _currentColor = color;
                targetMaterial.SetColor(EmissionColor, _currentColor);
            }, targetColor, transitionTime).SetEase(Ease.Linear);
        }

        private void OnDestroy()
        {
            if (targetMaterial) targetMaterial.SetColor(EmissionColor, initialColor);
        }
    }
}