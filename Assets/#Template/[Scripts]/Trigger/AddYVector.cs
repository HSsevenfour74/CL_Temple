using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using ConceptGames.ConceptLineOrion.Animated;

namespace JourneyInTheSkyScripts.Levels
{
    public class AddYVector : MonoBehaviour
    {
        public GameObject AnimationObject;
        public float UpDistance;
        public float UpPosTime;
        public float Delay;
        public Ease Ease = Ease.InOutSine;
        public bool AnchoredPosition;
        public Tween Tween;
        [HideInInspector] public Vector3 OriginalPos;
        [HideInInspector] public Vector3 NewPos;
        
        [Button("Set Self")]
        public void SetSelf()
        { AnimationObject = gameObject; }
        private void Awake()
        {
            if (AnchoredPosition) OriginalPos = AnimationObject.transform.localPosition;
            else OriginalPos = AnimationObject.transform.position;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player")) Trigger();
        }
        public void Trigger()
        {
            if (AnimationObject.GetComponent<LocalPosAnimator>())
                AnimationObject.GetComponent<LocalPosAnimator>().Trigger();
            else
            {
                Sequence sequence = DOTween.Sequence();
                if (AnchoredPosition) Tween = sequence.Append(AnimationObject.transform.DOLocalMove(OriginalPos + new Vector3(0, UpDistance, 0), UpPosTime).SetEase(Ease)).SetDelay(Delay);
                else Tween = sequence.Append(AnimationObject.transform.DOMove(OriginalPos + new Vector3(0, UpDistance, 0), UpPosTime).SetEase(Ease)).SetDelay(Delay);
            }

        }
    }
}