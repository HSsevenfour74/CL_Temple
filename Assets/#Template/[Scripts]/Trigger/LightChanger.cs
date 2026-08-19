using DG.Tweening;
using ConceptGames.ConceptLineOrion.Level;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ConceptGames.ConceptLineOrion.Trigger
{
    public class LightChanger : MonoBehaviour
    {
        public enum LightType { Directional, Point, Spot }
        public enum ShadowType { NoShadow, HardShadow, SoftShadow }
        public Light Light;
        [EnumToggleButtons] public LightType Type = LightType.Point;
        [ShowIf("Type", LightType.Point)] public float LightRange = 10f;
        [ShowIf("Type", LightType.Spot)][Range(1f, 179f)] 
        public float SpotAngle = 30f;
        [ColorPalette] public Color LightColor = Color.white;
        public float Intensity = 1f;
        [EnumToggleButtons] public ShadowType shadowType = ShadowType.HardShadow;
        [HideIf("shadowType", ShadowType.NoShadow)][Range(0f, 1f)] public float ShadowStrength = 0.5f;
        public Ease Ease = Ease.InOutSine;
        public float Time;

        [Button("通过选择的光照进行赋值")]
        private void Get()
        {
            if (Light)
            {
                Type = Light.type switch
                {
                    UnityEngine.LightType.Directional => LightType.Directional,
                    UnityEngine.LightType.Point => LightType.Point,
                    UnityEngine.LightType.Spot => LightType.Spot,
                    _ => Type
                };
                LightRange = Light.range;
                SpotAngle = Light.spotAngle;
                LightColor = Light.color;
                Intensity = Light.intensity;
                shadowType = Light.shadows switch
                {
                    LightShadows.None => ShadowType.NoShadow,
                    LightShadows.Hard => ShadowType.HardShadow,
                    LightShadows.Soft => ShadowType.SoftShadow,
                    _ => shadowType
                };
                ShadowStrength = Light.shadowStrength;
            }
            else Debug.LogWarning("未选择光照！");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.GetComponent<Player>()) return;
            Light.type = Type switch
            {
                LightType.Directional => UnityEngine.LightType.Directional,
                LightType.Point => UnityEngine.LightType.Point,
                LightType.Spot => UnityEngine.LightType.Spot,
                _ => Light.type
            };
            DOTween.Kill(Light);
            DOTween.To(() => Light.range, a => Light.range = a, LightRange, Time).SetEase(Ease);
            DOTween.To(() => Light.spotAngle, a => Light.spotAngle = a, SpotAngle, Time).SetEase(Ease);
            DOTween.To(() => Light.color, a => Light.color = a, LightColor, Time).SetEase(Ease);
            DOTween.To(() => Light.intensity, a => Light.intensity = a, Intensity, Time).SetEase(Ease);
            Light.shadows = shadowType switch
            {
                ShadowType.NoShadow => LightShadows.None,
                ShadowType.HardShadow => LightShadows.Hard,
                ShadowType.SoftShadow => LightShadows.Soft,
                _ => Light.shadows
            };
            if (shadowType != ShadowType.NoShadow) DOTween.To(() => Light.shadowStrength, a => Light.shadowStrength = a, ShadowStrength, Time).SetEase(Ease);
        }
    }
}