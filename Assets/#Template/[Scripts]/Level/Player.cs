using ConceptGames.ConceptLineOrion.Trigger;
using ConceptGames.ConceptLineOrion.UI;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

namespace ConceptGames.ConceptLineOrion.Level
{
    [DisallowMultipleComponent, RequireComponent(typeof(BoxCollider), typeof(Rigidbody))]
    public class Player : MonoBehaviour
    {
        [HideInInspector]
        public Transform selfTransform;

        public static Player Instance { get; private set; }
        public static Rigidbody Rigidbody { get; private set; }

        private GameObject tailPrefab;
        private GameObject cubesPrefab;
        private GameObject dustParticle;
        private GameObject uiPrefab;
        private GameObject startPrefab;
        private GameObject loadingPrefab;

        [Title("Data")]
        public LevelData levelData;

        [Title("Settings")] public Camera sceneCamera;
        public Light sceneLight;
        public Material characterMaterial;
        public Color uicolor = Color.white;
        public Vector3 startPosition = Vector3.zero;
        public Vector3 firstDirection = new Vector3(0, 90, 0);
        public Vector3 secondDirection = Vector3.zero;
        [MinValue(-1)] public static int poolSize = 1000;
        public List<Animator> playedAnimators;
        public List<PlayableDirector> playedTimelines;
        public List<Crown> crowns = new List<Crown>();
        public bool allowTurn = true;
        public bool noDeath;
        public bool drawDirection;

        internal float Speed { get; set; }
        internal AudioSource Soundtrack { get; set; }
        internal int SoundTrackProgress { get; set; }
        internal int BlockCount { get; set; }
        internal int CrownCount { get; set; }
        internal UnityEvent OnTurn { get; private set; }
        internal List<Crown> Crowns { get; set; }
        internal List<Checkpoint> Checkpoints { get; set; }
        internal bool disallowInput { get; set; }

        private BoxCollider characterCollider;
        private Vector3 tailPosition;
        [HideInInspector]
        public Transform tailHolder;
        public ObjectPool<Transform> tailPool = new ObjectPool<Transform>();
        private List<float> animatorProgresses = new List<float>();
        private List<double> timelineProgresses = new List<double>();
        private StartPage startPage;
        private bool loading;

        [HideInInspector] public Transform tail;
        [HideInInspector] public bool allowCreateTail = true;
        [HideInInspector] public Component currentCheckpoint;
        [HideInInspector] public Crown lastCrown;
        [HideInInspector] public float rotationTime;
        private bool didCreateTail = false;
        private float TailDistance =>
            new Vector2(tailPosition.x - selfTransform.position.x, tailPosition.z - selfTransform.position.z).magnitude;

        public static bool previousFrameIsGrounded;
        private const float groundedRayDistance = 0.05f;
        private ValueTuple<Vector3, Ray>[] groundedTestRays;
        private RaycastHit[] groundedTestResults = new RaycastHit[1];

        public bool Falling
        {
            get
            {
                for (var i = 0; i < groundedTestRays.Length; i++)
                {
                    groundedTestRays[i].Item2.origin = selfTransform.position +
                                                       selfTransform.localRotation * groundedTestRays[i].Item1;
                    if (Physics.RaycastNonAlloc(groundedTestRays[i].Item2, groundedTestResults,
                            groundedRayDistance + 0.1f, -257, QueryTriggerInteraction.Ignore) > 0)
                        return false;
                }

                return true;
            }
        }

        [HideInInspector]
        public int frame;
        [HideInInspector]
        public float lastTime;
        [HideInInspector]
        public float fps;
        [HideInInspector]
        public float gameTime;

        public const float timeInterval = 0.1f;

        private GameEvents events;
        public GameEvents Events =>
            events ? events : (events = GetComponent<GameEvents>() ? GetComponent<GameEvents>() : null);

        public void Awake()
        {
            if (!levelData)
            {
                Debug.LogError("无法获取关卡信息，请确保关卡数据文件（Level Data）填选正确且不为空");
                LevelManager.DialogBox("警告", "无法获取关卡信息，请确保关卡数据文件（Level Data）填选正确且不为空", "确定", true);
                return;
            }

            DOTween.Clear();
            Instance = this;
            Rigidbody = GetComponent<Rigidbody>();
            loading = false;
            Checkpoints = new List<Checkpoint>();
            Crowns = new List<Crown>();
            OnTurn = new UnityEvent();
            selfTransform = transform;
            tailHolder = new GameObject("PlayerTailHolder").transform;
            disallowInput = false;

            characterCollider = GetComponent<BoxCollider>();
            groundedTestRays = new ValueTuple<Vector3, Ray>[]
            {
                new ValueTuple<Vector3, Ray>(
                    characterCollider.center - new Vector3(characterCollider.size.x * 0.5f,
                        characterCollider.size.y * 0.5f - 0.1f, characterCollider.size.z * 0.5f),
                    new Ray(Vector3.zero, selfTransform.localRotation * Vector3.down)),
                new ValueTuple<Vector3, Ray>(
                    characterCollider.center - new Vector3(characterCollider.size.x * -0.5f,
                        characterCollider.size.y * 0.5f - 0.1f, characterCollider.size.z * 0.5f),
                    new Ray(Vector3.zero, selfTransform.localRotation * Vector3.down)),
                new ValueTuple<Vector3, Ray>(
                    characterCollider.center - new Vector3(characterCollider.size.x * 0.5f,
                        characterCollider.size.y * 0.5f - 0.1f, characterCollider.size.z * -0.5f),
                    new Ray(Vector3.zero, selfTransform.localRotation * Vector3.down)),
                new ValueTuple<Vector3, Ray>(
                    characterCollider.center - new Vector3(characterCollider.size.x * -0.5f,
                        characterCollider.size.y * 0.5f - 0.1f, characterCollider.size.z * -0.5f),
                    new Ray(Vector3.zero, selfTransform.localRotation * Vector3.down))
            };
            previousFrameIsGrounded = Falling;

            foreach (var animator in playedAnimators) animator.speed = 0f;
            foreach (var director in playedTimelines) director.Pause();

            LoadingPage.Instance?.Fade(0f, 0.4f);

            lastTime = Time.realtimeSinceStartup;
        }

        private void Start()
        {
            loadingPrefab = Resources.Load<GameObject>("Prefabs/LoadingPage");
            if (!LoadingPage.Instance) DontDestroyOnLoad(Instantiate(loadingPrefab));
            Cursor.visible = true;
            levelData.SetLevelData();
            firstDirection = firstDirection.Convert();
            secondDirection = secondDirection.Convert();
            tailPool.Size = poolSize;
            LevelManager.InitPlayerPosition(this, startPosition, false);
            cubesPrefab = Resources.Load<GameObject>("Prefabs/Remain");
            dustParticle = Resources.Load<GameObject>("Prefabs/Dust");
            uiPrefab = Resources.Load<GameObject>("Prefabs/LevelUI");
            startPrefab = Resources.Load<GameObject>("Prefabs/StartPage"); 
            tailPrefab = Resources.Load<GameObject>("Prefabs/Tail");
            selfTransform.GetComponent<MeshRenderer>().material = characterMaterial;
            selfTransform.eulerAngles = firstDirection;
            LevelManager.GameState = GameStatus.Waiting;
            Instantiate(uiPrefab);
            startPage = Instantiate(startPrefab).GetComponent<StartPage>();       
            Events?.Invoke(0);
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R) && !loading && LevelManager.GameState == GameStatus.Playing)
            {
                loading = true;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
#if UNITY_ANDROID || !UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Escape) && !loading && LevelManager.GameState == GameStatus.Playing)
            {
                loading = true;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
#endif
            GetFrame();
            if (allowTurn && !LevelManager.IsPointedOnUI())
            {
                switch (LevelManager.GameState)
                {
                    case GameStatus.Waiting:
                        if (LevelManager.Clicked && !Falling)
                        {
                            LevelManager.GameState = GameStatus.Playing;
                            if (!Soundtrack) Soundtrack = AudioManager.PlayTrack(levelData.soundTrack, 1f);
                            else AudioManager.Play();
                            foreach (Animator a in playedAnimators) a.speed = 1f;
                            foreach (PlayableDirector p in playedTimelines) p.Play();
                            foreach (PlayAnimator p in FindObjectsOfType<PlayAnimator>(true)) foreach (SingleAnimator s in p.animators) if (s.played) s.PlayAnimator();
                            foreach (FakePlayer f in FindObjectsOfType<FakePlayer>(true)) if (f.playing) f.state = FakePlayerState.Moving;
                            CreateTail();
                            Events?.Invoke(1);
                            if (startPage)
                            {
                                startPage.Hide();
                                startPage = null;
                            }
                            if (currentCheckpoint != null)
                            {
                                if (currentCheckpoint.GetComponent<Crown>())
                                {
                                    currentCheckpoint.GetComponent<Crown>().AnimateCrown(false);
                                }
                            }
                        }
                        break;
                    case GameStatus.Playing:
                        if (LevelManager.Clicked && !disallowInput && !Falling)
                            Turn();
                        gameTime += Time.deltaTime;
                        break;
                }
            }
            if (LevelManager.GameState == GameStatus.Playing || LevelManager.GameState == GameStatus.Moving)
            {
                selfTransform.Translate(Vector3.forward * ((float)Speed * Time.deltaTime), Space.Self);
                if (tail && !Falling)
                {
                    tail.position = (tailPosition + selfTransform.position) * 0.5f;
                    tail.localScale = new Vector3(tail.localScale.x, tail.localScale.y, TailDistance);
                    tail.position = new Vector3(tail.position.x, selfTransform.position.y, tail.position.z);
                    tail.LookAt(selfTransform);
                }
                if (previousFrameIsGrounded != Falling)
                {
                    previousFrameIsGrounded = Falling;
                    if (Falling)
                    {
                        tail = null;
                        Events?.Invoke(3);
                    }
                    else
                    {
                        CreateTail();
                        Destroy(
                            Instantiate(dustParticle,
                                new Vector3(selfTransform.localPosition.x,
                                    selfTransform.localPosition.y - selfTransform.lossyScale.y * 0.5f + 0.2f,
                                    selfTransform.localPosition.z), Quaternion.Euler(90f, 0f, 0f)), 2f);
                        Events?.Invoke(4);
                    }
                }
                
            }

            if (LevelManager.GameState == GameStatus.Playing)
            {
                if (levelData.useMusicTime)
                    SoundTrackProgress = Soundtrack ? (int)(AudioManager.Progress * 100) : 0;
                else
                    SoundTrackProgress = (int)(gameTime / levelData.levelTime * 100) >= 100 ? 100 : (int)(gameTime / levelData.levelTime * 100);
            }
            else
            {
                if (!didCreateTail)
                {
                    allowCreateTail = true;
                    CreateTail();
                    didCreateTail = true;
                }
            }
        }
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("Obstacle") && !noDeath && LevelManager.GameState == GameStatus.Playing)
            {
                if (Checkpoints.Count <= 0 && Crowns.Count <= 0)
                {
                    LevelManager.PlayerDeath(this, DieReason.Hit, cubesPrefab, collision);
                }
                else
                {
                    if (Checkpoints.Count > 0)
                    {
                        LevelManager.PlayerDeath(this, DieReason.Hit, cubesPrefab, collision, true);
                    }
                    else if (Crowns.Count > 0)
                    {
                        LevelManager.PlayerDeath(this, DieReason.Hit, cubesPrefab, collision, true);
                    }
                }
            }
        }
        internal void Turn()
        {
            selfTransform.eulerAngles = selfTransform.eulerAngles == firstDirection ? secondDirection : firstDirection;
            CreateTail();
            OnTurn.Invoke();
            Events?.Invoke(2);
        }
        public void CreateTail()
        {
            if(!this.GetComponent<MeshRenderer>().enabled)
            {
                tail = null;
                return;
            }

            var now = Quaternion.Euler(selfTransform.localEulerAngles);
            var offset = tailPrefab.transform.localScale.z * 0.5f;

            if (tail)
            {
                var last = Quaternion.Euler(tail.transform.localEulerAngles);
                var angle = Quaternion.Angle(last, now);
                if (angle is >= 0f and <= 90f) offset = 0.5f * Mathf.Tan(Mathf.PI / 180f * angle * 0.5f);
                else offset = -0.5f * Mathf.Tan(Mathf.PI / 180f * ((180f - angle) * 0.5f));
                var end = tailPosition + last * Vector3.forward * (TailDistance + offset);
                tail.position = (tailPosition + end) * 0.5f;
                tail.position = new Vector3(tail.position.x, selfTransform.position.y, tail.position.z);
                tail.localScale =
                    new Vector3(tail.localScale.x, tail.localScale.y, Vector3.Distance(tailPosition, end));
                tail.LookAt(selfTransform.position);
            }

            tailPosition = selfTransform.position + now * Vector3.back * Mathf.Abs(offset);
            if (!tailPool.Full)
            {
                tail = Instantiate(tailPrefab, selfTransform.position, selfTransform.rotation).transform;
                tail.parent = tailHolder;
                tailPool.Add(tail);
            }
            else
            {
                tail = tailPool.First();
                tailPool.Add(tail);
            }
        }
        public void ClearPool()
        {
            tailPool.ClearAll();
            tail = null;
        }
        internal void GetAnimatorProgresses()
        {
            animatorProgresses.Clear();
            foreach (var a in playedAnimators) animatorProgresses.Add(a.GetCurrentAnimatorStateInfo(0).normalizedTime);
        }
        internal void SetAnimatorProgresses(float progress = 0f)
        {
            for (int a = 0; a < playedAnimators.Count; a++)
            {
                if (progress != 0 && a == 0)
                {
                    playedAnimators[a].Play(playedAnimators[a].GetCurrentAnimatorStateInfo(0).fullPathHash, 0, progress);
                }
                else
                {
                    playedAnimators[a].Play(playedAnimators[a].GetCurrentAnimatorStateInfo(0).fullPathHash, 0, animatorProgresses[a]);
                }
            }
        }
        internal void GetTimelineProgresses()
        {
            timelineProgresses.Clear();
            foreach (var p in playedTimelines) timelineProgresses.Add(p.time);
        }
        internal void SetTimelineProgresses(float progress = 0f)
        {
            for (int a = 0; a < playedTimelines.Count; a++)
            {
                if (progress != 0 && a == 0)
                {
                    playedTimelines[a].time = progress;
                    playedTimelines[a].Evaluate();
                }
                else
                {
                    playedTimelines[a].time = timelineProgresses[a];
                    playedTimelines[a].Evaluate();
                }
            }
        }
        public void GetFrame()
        {
            frame++;
            if (Time.realtimeSinceStartup - lastTime < timeInterval) return;

            var time = Time.realtimeSinceStartup - lastTime;
            fps = frame / time;

            lastTime = Time.realtimeSinceStartup;
            frame = 0;
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (drawDirection) LevelManager.DrawDirection(transform, 4);
        }

        [Button("Get Start Position", ButtonSizes.Large)]
        private void GetStartPosition()
        {
            startPosition = transform.position;
        }
#endif
    }
}