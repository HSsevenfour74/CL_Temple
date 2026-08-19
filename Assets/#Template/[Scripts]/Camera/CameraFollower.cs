using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ConceptGames.ConceptLineOrion.Level
{
    [DisallowMultipleComponent]
    public class CameraFollower : MonoBehaviour
    {
        private Transform selfTransform;

        public static CameraFollower Instance { get; private set; }
        public Camera thisCamera { get; set; }
        public List<Camera> extraCameras = new List<Camera>();

        public Transform target;

        [SerializeField] internal Vector3 followSpeed = new Vector3(1.5f, 1.5f, 1.5f);
        [SerializeField] public bool follow = true;
        [SerializeField] internal bool smooth = true;
        public bool isSkinPreview;

        public Transform CameraRoot;
        internal Transform rotator;
        internal Transform scale;

        public Tween offset;
        public Tween rotation;
        public Tween zoom;
        public Tween shake;
        public Tween fov;

        public float shakePower { get; set; }

        public void Awake()
        {
            Instance = this;
            selfTransform = transform;
        }

        public void Start()
        {
            rotator = selfTransform.Find("Rotator");
            scale = rotator.Find("Scale");
            thisCamera = scale.Find("Camera").GetComponent<Camera>();
        }

        public void Update()
        {
            var translation = target.position - selfTransform.position;
            if ((LevelManager.GameState != GameStatus.Playing || !follow) && !isSkinPreview) return;
            if (smooth)
                selfTransform.Translate(new Vector3(translation.x * followSpeed.x * Time.deltaTime,
                    translation.y * followSpeed.y * Time.deltaTime, translation.z * followSpeed.z * Time.deltaTime));
            else selfTransform.position = target.position;
        }

        internal void Trigger(Vector3 n_offset, Vector3 n_rotation, Vector3 n_scale, float n_fov, float duration,
            Ease ease,
            RotateMode mode, UnityEvent callback)
        {
            SetOffset(n_offset, duration, ease);
            SetRotation(n_rotation, duration, mode, ease);
            SetScale(n_scale, duration, ease);
            SetFov(n_fov, duration, ease);
            rotation.OnComplete(callback.Invoke);
        }

        internal void KillAll()
        {
            offset?.Kill();
            rotation?.Kill();
            zoom?.Kill();
            shake?.Kill();
            fov?.Kill();
        }

        public void SetOffset(Vector3 n_offset, float duration, Ease ease = Ease.InOutSine)
        {
            if (offset != null)
            {
                offset.Kill();
                offset = null;
            }

            if(CameraRoot)
                offset = CameraRoot.DOLocalMove(n_offset, duration).SetEase(ease);
            else
                offset = rotator.DOLocalMove(n_offset, duration).SetEase(ease);

        }

        public void SetRotation(Vector3 n_rotation, float duration, RotateMode mode, Ease ease = Ease.InOutSine)
        {
            if (rotation != null)
            {
                rotation.Kill();
                rotation = null;
            }

            if (CameraRoot)
                rotation = CameraRoot.DOLocalRotate(n_rotation, duration, mode).SetEase(ease);
            else
                rotation = rotator.DOLocalRotate(n_rotation, duration, mode).SetEase(ease);
        }

        public void SetScale(Vector3 n_scale, float duration, Ease ease = Ease.InOutSine)
        {
            if (zoom != null)
            {
                zoom.Kill();
                zoom = null;
            }
            if (CameraRoot)
                zoom = CameraRoot.DOScale(n_scale, duration).SetEase(ease);
            else
                zoom = scale.DOScale(n_scale, duration).SetEase(ease);
        }

        public void DoShake(float power = 1f, float duration = 3f)
        {
            if (shake != null)
            {
                shake.Kill();
                shake = null;
            }

            shake = DOTween.To(() => shakePower, x => shakePower = x, power, duration * 0.5f).SetEase(Ease.Linear);
            shake.SetLoops(2, LoopType.Yoyo);
            shake.OnUpdate(ShakeUpdate);
            shake.OnComplete(ShakeFinished);
        }

        private void ShakeUpdate()
        {
            scale.transform.localPosition = new Vector3(UnityEngine.Random.value * shakePower,
                UnityEngine.Random.value * shakePower, UnityEngine.Random.value * shakePower);
        }

        private void ShakeFinished()
        {
            scale.transform.localPosition = Vector3.zero;
        }

        private void SetFov(float n_fov, float duration, Ease ease = Ease.InOutSine)
        {
            if (fov != null)
            {
                fov.Kill();
                fov = null;
            }

            fov = thisCamera.DOFieldOfView(n_fov, duration).SetEase(ease);
            if(extraCameras != null)
            {
                foreach(var c in extraCameras)
                {
                    fov = c.DOFieldOfView(n_fov, duration).SetEase(ease);
                }
            }
        }
    }

    [Serializable]
    public class CameraSettings
    {
        public Vector3 offset;
        public Vector3 rotation;
        public Vector3 scale;
        public float fov;
        public bool follow;

        internal CameraSettings GetCamera()
        {
            var settings = new CameraSettings();
            var follower = CameraFollower.Instance;
            settings.offset = follower.rotator.localPosition;
            settings.rotation = follower.rotator.localEulerAngles;
            settings.scale = follower.scale.localScale;
            settings.fov = follower.thisCamera.fieldOfView;
            settings.follow = follower.follow;
            return settings;
        }

        internal void SetCamera()
        {
            var follower = CameraFollower.Instance;
            follower.rotator.localPosition = offset;
            follower.rotator.localEulerAngles = rotation;
            follower.scale.localScale = scale;
            follower.scale.localPosition = Vector3.zero;
            follower.thisCamera.fieldOfView = fov;
            follower.follow = follow;
        }
    }
}