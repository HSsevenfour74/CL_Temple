using ConceptGames.ConceptLineOrion.Level;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ConceptGames.ConceptLineOrion.Trigger
{
    [DisallowMultipleComponent, RequireComponent(typeof(Collider))]
    public class CameraShakeTrigger : MonoBehaviour
    {
        [SerializeField] private float power = 1f;
        [SerializeField] private float duration = 2f;
        [SerializeField] private bool useCameraFollower = true;

        public Tween shakeTween;
        [ShowIf("@!useCameraFollower"),Title("如果需要震动的相机用的帧动画就给相机创建一个父物体拖进来，不是则拖需要震动的相机")]
        [ShowIf("@!useCameraFollower")]public Transform shakeTransform;
        private float currentShakePower; 
        private Vector3 originalLocalPos; 

        public float shakePower { get; set; }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
                if (!useCameraFollower && shakeTransform)
                    DoShake(power, duration);
                else if (CameraFollower.Instance)
                    CameraFollower.Instance.DoShake(power, duration);
        }

        public void DoShake(float power = 1f, float duration = 3f)
        {
            if (shakeTween != null && shakeTween.IsActive())
            {
                shakeTween.Kill();
                shakeTween = null;
            }

            shakeTween = DOTween.To(() => shakePower, x => shakePower = x, power, duration * 0.5f).SetEase(Ease.Linear);
            shakeTween.SetLoops(2, LoopType.Yoyo);
            originalLocalPos = shakeTransform.transform.localPosition;
            currentShakePower = 0f;
            shakeTween = DOTween.To(
                () => currentShakePower,
                x => currentShakePower = x,
                power,
                duration * 0.5f
            )
            .SetEase(Ease.Linear)
            .SetLoops(2, LoopType.Yoyo)
            .OnUpdate(ShakeUpdate)
            .OnComplete(ShakeFinished)
            .SetLink(shakeTransform.gameObject);
        }

        private void ShakeUpdate()
        {
            if (shakeTransform == null) return;

            float randomX = (Random.value - 0.5f) * 2 * currentShakePower;
            float randomY = (Random.value - 0.5f) * 2 * currentShakePower;
            float randomZ = (Random.value - 0.5f) * 2 * currentShakePower;

            shakeTransform.transform.localPosition = originalLocalPos + new Vector3(randomX, randomY, randomZ);
        }

        private void ShakeFinished()
        {
            if (shakeTransform == null) return;

            shakeTransform.transform.DOLocalMove(originalLocalPos, 0.1f).SetEase(Ease.OutQuad);
            currentShakePower = 0f;
            shakeTween = null;
        }
    }
}