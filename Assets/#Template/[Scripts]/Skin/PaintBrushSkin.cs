using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using ConceptGames.ConceptLineOrion.Trigger;

namespace ConceptGames.ConceptLineOrion.Level
{
    [DisallowMultipleComponent]
    public sealed class PaintBrushSkin : MonoBehaviour
    {
        private const string BrushAnimationObjectName = "BrushAnimParent";
        private const string DeathAnimationObjectName = "DeadAnimParent";
        private const string TrailParentObjectName = "TrailParent";
        private const string RotateObjectName = "BrushRotateParent";
        private const float BrushAnimationStepDuration = 0.2f;
        private const float DeathAnimationDuration = 1f;

        private static readonly Vector3 BrushIdleRotation = new Vector3(0f, 0f, -23f);
        private static readonly Vector3 BrushLeftRotation = new Vector3(-5f, 0f, -23f);
        private static readonly Vector3 BrushRightRotation = new Vector3(5f, 0f, -23f);
        private static readonly Vector3 BrushIdlePosition = Vector3.zero;
        private static readonly Vector3 BrushForwardPosition = new Vector3(0f, 0f, 0.215f);
        private static readonly Vector3 BrushBackwardPosition = new Vector3(0f, 0f, -0.215f);
        private static readonly Vector3 DeathEndRotation = new Vector3(0f, 0f, 111f);

        [SerializeField] private Transform brushAnimationTransform;
        [SerializeField] private Transform deathAnimationTransform;
        [SerializeField] private TrailRenderer[] paintTrails;
        [SerializeField] private Transform trailsParent;
        [SerializeField] private Transform rotateTransform;
        [SerializeField] private float trailTimeFrom = 0.4f;
        [SerializeField] private float trailTimeTo = 0.7f;
        [SerializeField] private float trailTimeSpeed;
        [SerializeField] private float rotateSpeed = 2f;

        private readonly List<Transform> usedTrailTransforms = new List<Transform>();
        private Transform[] trailTransforms;
        private int currentTrailIndex = -1;
        private float trailTimeFactor;
        private int trailTimeFactorSign = 1;
        private float recycleTrailsAfter;
        private bool presentationRunning;
        private Transform cachedRotateTransform;
        private Quaternion rotateLocalRotation;
        private Quaternion previousRotateWorldRotation;
        private Sequence brushAnimationSequence;
        private Tween deathAnimationTween;

        private void Awake()
        {
            ResolveReferences();
            ResetPresentation();
        }

        private void Update()
        {
            UpdateTurnRotation();
            UpdateTrailLifetime();
            RecycleFinishedTrails();
        }

        public void OnBound()
        {
            ResolveReferences();
            ResetPresentation();
        }

        public void OnUnbound()
        {
            ResetPresentation();
        }

        public void OnStarted()
        {
            if (presentationRunning) return;
            ResolveReferences();
            presentationRunning = true;
            RestoreRotation();
            previousRotateWorldRotation = rotateTransform != null
                ? rotateTransform.rotation
                : Quaternion.identity;
            StopDeathAnimation(true);
            StartBrushAnimation();
            BeginTrailSegment();
        }

        public void OnStopped()
        {
            presentationRunning = false;
            EndTrailSegment();
            StopBrushAnimation(true);
            StopDeathAnimation(true);
        }

        public void OnTurned()
        {
            if (!presentationRunning || rotateTransform == null) return;

            // Keep the brush in its previous world rotation, then let Update ease it
            // toward the Player/DemoLine's new direction like DL2's PaintBrush.
            rotateTransform.rotation = previousRotateWorldRotation;
        }

        public void OnAirborne()
        {
            EndTrailSegment();
        }

        public void OnGrounded()
        {
            if (!presentationRunning) return;
            BeginTrailSegment();
        }

        public void OnDied()
        {
            presentationRunning = false;
            EndTrailSegment();
            StopBrushAnimation(true);
            StartDeathAnimation();
        }

        public void OnReset()
        {
            ResetPresentation();
        }

        public void OnDestroy()
        {
            ResetPresentation();
        }

        private void ResolveReferences()
        {
            if (paintTrails == null || paintTrails.Length == 0)
                paintTrails = GetComponentsInChildren<TrailRenderer>(true);

            Transform[] transforms = GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < transforms.Length; i++)
            {
                Transform candidate = transforms[i];
                if (candidate == null) continue;
                if (brushAnimationTransform == null && candidate.name == BrushAnimationObjectName)
                    brushAnimationTransform = candidate;
                if (deathAnimationTransform == null && candidate.name == DeathAnimationObjectName)
                    deathAnimationTransform = candidate;
                if (trailsParent == null && candidate.name == TrailParentObjectName)
                    trailsParent = candidate;
                if (rotateTransform == null && candidate.name == RotateObjectName)
                    rotateTransform = candidate;
            }

            CacheTrailTransforms();
            CacheRotateTransform();
        }

        private void BeginTrailSegment()
        {
            if (paintTrails == null || paintTrails.Length == 0 || currentTrailIndex >= 0) return;

            CacheTrailTransforms();
            for (int index = 0; index < paintTrails.Length; index++)
            {
                TrailRenderer trail = paintTrails[index];
                Transform trailTransform = trailTransforms != null && index < trailTransforms.Length
                    ? trailTransforms[index]
                    : null;
                if (trail == null || trailTransform == null || usedTrailTransforms.Contains(trailTransform))
                    continue;

                currentTrailIndex = index;
                usedTrailTransforms.Add(trailTransform);
                if (trailsParent != null)
                {
                    trailTransform.SetParent(trailsParent, false);
                    trailTransform.localPosition = Vector3.zero;
                }

                trailTimeFactor = 0f;
                trailTimeFactorSign = 1;
                trail.emitting = false;
                trail.enabled = true;
                trail.Clear();
                trail.emitting = true;
                trail.Clear();
                recycleTrailsAfter = Time.unscaledTime + 0.5f;
                return;
            }
        }

        private void EndTrailSegment()
        {
            if (paintTrails == null || currentTrailIndex < 0 || currentTrailIndex >= paintTrails.Length)
            {
                currentTrailIndex = -1;
                return;
            }

            Transform trailTransform = trailTransforms != null && currentTrailIndex < trailTransforms.Length
                ? trailTransforms[currentTrailIndex]
                : null;
            if (trailTransform != null)
                trailTransform.SetParent(null, true);

            currentTrailIndex = -1;
            recycleTrailsAfter = Time.unscaledTime + 0.5f;
        }

        private void ResetPresentation()
        {
            ResolveReferences();
            presentationRunning = false;
            currentTrailIndex = -1;
            trailTimeFactor = 0f;
            trailTimeFactorSign = 1;
            recycleTrailsAfter = 0f;
            StopBrushAnimation(true);
            StopDeathAnimation(true);
            RestoreRotation();

            if (paintTrails == null) return;
            for (int i = 0; i < paintTrails.Length; i++)
            {
                ClearTrail(i);
            }

            usedTrailTransforms.Clear();
        }

        private void UpdateTurnRotation()
        {
            if (!presentationRunning || rotateTransform == null || rotateTransform.parent == null) return;

            Quaternion targetRotation = rotateTransform.parent.rotation * rotateLocalRotation;
            rotateTransform.rotation = Quaternion.Lerp(
                rotateTransform.rotation,
                targetRotation,
                Mathf.Max(0f, rotateSpeed) * Time.deltaTime);
            previousRotateWorldRotation = rotateTransform.rotation;
        }

        private void UpdateTrailLifetime()
        {
            if (!presentationRunning || trailTimeSpeed <= 0f || paintTrails == null) return;
            if (currentTrailIndex < 0 || currentTrailIndex >= paintTrails.Length) return;

            TrailRenderer trail = paintTrails[currentTrailIndex];
            if (trail == null) return;

            if (trailTimeFactor >= 1f) trailTimeFactorSign = -1;
            if (trailTimeFactor <= 0f) trailTimeFactorSign = 1;
            trailTimeFactor += Time.smoothDeltaTime * trailTimeSpeed * trailTimeFactorSign;
            trail.time = Mathf.Lerp(trailTimeFrom, trailTimeTo, trailTimeFactor);
        }

        private void RecycleFinishedTrails()
        {
            if (paintTrails == null || Time.unscaledTime < recycleTrailsAfter) return;

            for (int i = 0; i < paintTrails.Length; i++)
            {
                if (i == currentTrailIndex) continue;
                TrailRenderer trail = paintTrails[i];
                if (trail == null || !trail.enabled || trail.isVisible) continue;
                ClearTrail(i);
            }
        }

        private void ClearTrail(int index)
        {
            if (paintTrails == null || index < 0 || index >= paintTrails.Length) return;

            TrailRenderer trail = paintTrails[index];
            Transform trailTransform = trailTransforms != null && index < trailTransforms.Length
                ? trailTransforms[index]
                : null;
            if (trail != null)
            {
                trail.emitting = false;
                trail.Clear();
                trail.enabled = false;
            }

            if (trailTransform != null)
            {
                if (trailsParent != null)
                {
                    trailTransform.SetParent(trailsParent, false);
                    trailTransform.localPosition = Vector3.zero;
                }
                usedTrailTransforms.Remove(trailTransform);
            }
        }

        private void CacheTrailTransforms()
        {
            if (paintTrails == null)
            {
                trailTransforms = null;
                return;
            }

            bool rebuild = trailTransforms == null || trailTransforms.Length != paintTrails.Length;
            if (!rebuild)
            {
                for (int i = 0; i < paintTrails.Length; i++)
                {
                    Transform expected = paintTrails[i] != null ? paintTrails[i].transform : null;
                    if (trailTransforms[i] != expected)
                    {
                        rebuild = true;
                        break;
                    }
                }
            }

            if (!rebuild) return;
            trailTransforms = new Transform[paintTrails.Length];
            for (int i = 0; i < paintTrails.Length; i++)
                trailTransforms[i] = paintTrails[i] != null ? paintTrails[i].transform : null;
            usedTrailTransforms.Clear();
            currentTrailIndex = -1;
        }

        private void CacheRotateTransform()
        {
            if (rotateTransform == null || cachedRotateTransform == rotateTransform) return;
            cachedRotateTransform = rotateTransform;
            rotateLocalRotation = rotateTransform.localRotation;
            previousRotateWorldRotation = rotateTransform.rotation;
        }

        private void RestoreRotation()
        {
            CacheRotateTransform();
            if (rotateTransform == null) return;
            rotateTransform.localRotation = rotateLocalRotation;
            previousRotateWorldRotation = rotateTransform.rotation;
        }

        private void StartBrushAnimation()
        {

            StopBrushAnimation(true);
            if (brushAnimationTransform == null) return;

            Sequence sequence = DOTween.Sequence();
            AppendBrushPose(sequence, BrushLeftRotation, BrushForwardPosition, Ease.OutSine);
            AppendBrushPose(sequence, BrushIdleRotation, BrushIdlePosition, Ease.InSine);
            AppendBrushPose(sequence, BrushRightRotation, BrushBackwardPosition, Ease.OutSine);
            AppendBrushPose(sequence, BrushIdleRotation, BrushIdlePosition, Ease.InSine);
            sequence
                .SetAutoKill(false)
                .SetUpdate(false)
                .SetLoops(-1, LoopType.Restart)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
            brushAnimationSequence = sequence;
        }

        private void AppendBrushPose(Sequence sequence, Vector3 rotation, Vector3 position, Ease ease)
        {
            sequence.Append(brushAnimationTransform
                .DOLocalRotate(rotation, BrushAnimationStepDuration, RotateMode.Fast)
                .SetEase(ease));
            sequence.Join(brushAnimationTransform
                .DOLocalMove(position, BrushAnimationStepDuration)
                .SetEase(ease));
        }

        private void StopBrushAnimation(bool reset)
        {
            brushAnimationSequence?.Kill(false);
            brushAnimationSequence = null;
            if (!reset || brushAnimationTransform == null) return;

            brushAnimationTransform.localPosition = BrushIdlePosition;
            brushAnimationTransform.localRotation = Quaternion.Euler(BrushIdleRotation);
        }

        private void StartDeathAnimation()
        {
            StopDeathAnimation(true);
            if (deathAnimationTransform == null) return;

            deathAnimationTween = deathAnimationTransform
                .DOLocalRotate(DeathEndRotation, DeathAnimationDuration, RotateMode.Fast)
                .SetEase(Ease.OutBounce)
                .SetAutoKill(false)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
        }

        private void StopDeathAnimation(bool reset)
        {
            deathAnimationTween?.Kill(false);
            deathAnimationTween = null;
            if (reset && deathAnimationTransform != null)
                deathAnimationTransform.localRotation = Quaternion.identity;
        }
    }
}
