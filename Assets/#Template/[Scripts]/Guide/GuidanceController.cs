using ConceptGames.ConceptLineOrion.Level;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ConceptGames.ConceptLineOrion.Guide
{
    [DisallowMultipleComponent]
    public class GuidanceController : MonoBehaviour
    {
        private const string RuntimeLineHolderName = "GuidanceSegmentLines_Runtime";
        private const float MinimumLineLength = 0.01f;

        private static readonly float[] DefaultLineLengthPattern = { 0.5f, 0.2f, 2f, 0.2f };

        public static GuidanceController Instance { get; private set; }

        private Player player;
        private Transform playerTransform;

        [Title("Creating")]
        [SerializeField] private bool createBoxes = false;
        [SerializeField] private bool createLines = true;

        [Title("Settings")]
        [SerializeField] internal Transform boxHolder;
        [SerializeField] private List<Color> guidanceBoxColor = new List<Color>();
        [SerializeField, MinValue(0f)] private float lineGap = 0.2f;

        [Title("Segmented Lines")]
        [SerializeField, MinValue(0.01f)] private float lineWidth = 0.15f;
        [SerializeField, MinValue(0f)] private float lineVerticalOffset = 0.01f;
        [SerializeField, MinValue(0.1f)] private float lineVisibleDistance = 120f;
        [SerializeField, MinValue(0.02f)] private float lineFadeDistance = 0.1f;
        [SerializeField] private float[] lineLengthPattern = { 0.5f, 0.2f, 2f, 0.2f };

        private GameObject boxPrefab;
        private GameObject linePrefab;
        private Transform holder;
        private Transform runtimeLineHolder;
        private int id;
        private readonly List<GuidelineTap> boxes = new List<GuidelineTap>();
        private readonly List<RuntimeLineSegment> runtimeSegments = new List<RuntimeLineSegment>();
        private int nextSegmentIndex;
        private bool started;
        private bool originalBoxCreated;
        private bool linesInitialized;
        private bool hasPreviousPlayerPosition;
        private Vector3 previousPlayerPosition;
        private float travelledDistance;
        internal bool useGuideline;


        private void Awake()
        {
            Instance = this;

            id = 0;
            ResolveRuntimeAssets();

            if (createBoxes)
            {
                if (!boxHolder)
                {
                    holder = new GameObject("GuidanceBoxHolder").transform;
                    boxHolder = holder;
                }
                else
                {
                    holder = boxHolder;
                }
            }

            RefreshBoxes();
        }

        private void OnEnable()
        {
            Instance = this;
            ResolveRuntimeAssets();
            if (boxHolder && boxes.Count == 0)
                RefreshBoxes();

            LevelManager.revivePlayer -= HandlePlayerRevived;
            LevelManager.revivePlayer += HandlePlayerRevived;
            hasPreviousPlayerPosition = false;
        }

        private void Start()
        {
            if (boxHolder)
            {
                boxes.AddRange(boxHolder.GetComponentsInChildren<GuidelineTap>());
                for (int i = 0; i < boxes.Count; i++)
                {
                    boxes[i].InitBox();
                    boxes[i].SetColor(guidanceBoxColor);
                }
                ResolvePlayer();
            }

            if (createBoxes && TryCreateOriginalBox())
                RefreshBoxes();

            if (createLines)
                BuildSegmentedLines();
        }

        private void Update()
        {
            ResolvePlayer();

            if (createBoxes)
                UpdateBoxRecorder();

            if (createLines && !linesInitialized)
                BuildSegmentedLines();

            UpdateSegmentedLines();
        }

        private void UpdateBoxRecorder()
        {
            if (!player || !playerTransform)
                return;

            if (TryCreateOriginalBox())
            {
                RefreshBoxes();
                linesInitialized = false;
            }

            if (LevelManager.GameState == GameStatus.Playing && !started)
            {
                player.OnTurn.AddListener(CreateGuidanceBoxAtPlayer);
                started = true;
            }
        }

        private bool TryCreateOriginalBox()
        {
            if (originalBoxCreated || !playerTransform || !holder || !boxPrefab)
                return false;

            Transform existing = holder.Find("OriginalGuidanceBox");
            if (existing)
            {
                originalBoxCreated = true;
                return false;
            }

            GameObject box = Instantiate(
                boxPrefab,
                playerTransform.position - new Vector3(0f, 0.45f, 0f),
                Quaternion.Euler(90f, player.firstDirection.y, 0f),
                holder);

            box.name = "OriginalGuidanceBox";
            originalBoxCreated = true;
            return true;
        }

        private void CreateGuidanceBoxAtPlayer()
        {
            if (!player || !holder || !boxPrefab)
                return;

            float currentYaw = NormalizeAngle(player.transform.eulerAngles.y);
            float firstYaw = NormalizeAngle(player.firstDirection.y);
            float forward = Mathf.Abs(Mathf.DeltaAngle(currentYaw, firstYaw)) < 0.1f
                ? player.secondDirection.y
                : player.firstDirection.y;

            GameObject box = Instantiate(
                boxPrefab,
                player.transform.position - new Vector3(0f, 0.45f, 0f),
                Quaternion.Euler(90f, forward, 0f),
                holder);

            box.name = "GuidanceBox " + id;
            id++;

            RefreshBoxes();
            linesInitialized = false;
        }

        private void RefreshBoxes()
        {
            boxes.Clear();
            if (!boxHolder)
                return;
            boxes.AddRange(boxHolder.GetComponentsInChildren<GuidelineTap>());
            for (int i = 0; i < boxes.Count; i++)
            {
                boxes[i].InitBox();
                boxes[i].SetColor(guidanceBoxColor);
            }
        }

        private void BuildSegmentedLines()
        {
            ResolveRuntimeAssets();
            linesInitialized = true;
            nextSegmentIndex = 0;
            runtimeSegments.Clear();
            RemovePreviouslyGeneratedLines();

            if (!boxHolder || boxes.Count < 2)
            {
                RefreshBoxes();
                if (!boxHolder || boxes.Count < 2)
                    return;
            }

            if (!linePrefab)
            {
                Debug.LogWarning("[GuidanceController] Resources/Prefabs/GuidanceLine is missing.", this);
                return;
            }

            float[] pattern = GetValidatedLinePattern();
            GameObject lineHolderObject = new GameObject(RuntimeLineHolderName);
            runtimeLineHolder = lineHolderObject.transform;
            runtimeLineHolder.SetParent(boxHolder, false);

            float routeDistance = 0f;
            for (int sectionIndex = 0; sectionIndex < boxes.Count - 1; sectionIndex++)
            {
                GuidelineTap startBox = boxes[sectionIndex];
                GuidelineTap endBox = boxes[sectionIndex + 1];
                float sectionLength = startBox && endBox
                    ? DistanceXZ(startBox.transform.position, endBox.transform.position)
                    : 0f;

                GenerateSegmentedSection(
                    startBox,
                    endBox,
                    sectionIndex,
                    routeDistance,
                    sectionLength,
                    pattern);
                routeDistance += sectionLength;
            }

            hasPreviousPlayerPosition = false;
        }
        public void SetUseGuideline()
        {
            if (!boxHolder)
                return;
            boxHolder.gameObject.SetActive(useGuideline);
        }

        public void ResetAllTaps()
        {
            foreach (var VARIABLE in boxes)
            {
                VARIABLE.InitBox();
                VARIABLE.SetDisplay(true);
            }
        }
        private void GenerateSegmentedSection(
            GuidelineTap startBox,
            GuidelineTap endBox,
            int sectionIndex,
            float sectionRouteStart,
            float sectionRouteLength,
            float[] pattern)
        {
            if (!startBox || !endBox || !startBox.haveLine)
                return;

            Vector3 startPosition = startBox.transform.position;
            Vector3 endPosition = endBox.transform.position;
            Vector3 sectionVector = endPosition - startPosition;
            float sectionLength = sectionVector.magnitude;
            if (sectionLength <= MinimumLineLength)
                return;

            Vector3 direction = sectionVector / sectionLength;
            float startInset = GetBoxInset(startBox, direction) + lineGap;
            float endInset = GetBoxInset(endBox, -direction) + lineGap;
            float drawableLength = sectionLength - startInset - endInset;
            if (drawableLength <= MinimumLineLength)
                return;

            float cursor = 0f;
            int patternIndex = 0;
            int visiblePartIndex = 0;

            while (cursor < drawableLength - MinimumLineLength)
            {
                float requestedLength = pattern[patternIndex % pattern.Length];
                float partLength = Mathf.Min(requestedLength, drawableLength - cursor);

                if ((patternIndex & 1) == 0 && partLength > MinimumLineLength)
                {
                    float startDistance = startInset + cursor;
                    float endDistance = startDistance + partLength;
                    Vector3 partStart = startPosition + direction * startDistance + Vector3.up * lineVerticalOffset;
                    Vector3 partEnd = startPosition + direction * endDistance + Vector3.up * lineVerticalOffset;
                    float routeScale = sectionRouteLength / sectionLength;
                    CreateRuntimeLineSegment(
                        partStart,
                        partEnd,
                        endBox.Renderer,
                        sectionIndex,
                        visiblePartIndex,
                        sectionRouteStart + startDistance * routeScale,
                        sectionRouteStart + endDistance * routeScale);
                    visiblePartIndex++;
                }

                cursor += partLength;
                patternIndex++;
            }
        }

        private void CreateRuntimeLineSegment(
            Vector3 startPosition,
            Vector3 endPosition,
            SpriteRenderer targetBoxRenderer,
            int sectionIndex,
            int visiblePartIndex,
            float routeStartDistance,
            float routeEndDistance)
        {
            Vector3 direction = endPosition - startPosition;
            float length = direction.magnitude;
            if (length <= MinimumLineLength)
                return;

            Quaternion rotation = CreateLineRotation(direction / length);
            GameObject lineObject = Instantiate(
                linePrefab,
                (startPosition + endPosition) * 0.5f,
                rotation,
                runtimeLineHolder);

            lineObject.name = $"Guidance Segment {sectionIndex:000}-{visiblePartIndex:000}";
            SpriteRenderer renderer = lineObject.GetComponent<SpriteRenderer>();
            if (!renderer)
            {
                Destroy(lineObject);
                return;
            }

            Vector3 spriteSize = renderer.sprite ? renderer.sprite.bounds.size : Vector3.one;
            float spriteWidth = Mathf.Max(Mathf.Abs(spriteSize.x), MinimumLineLength);
            float spriteLength = Mathf.Max(Mathf.Abs(spriteSize.y), MinimumLineLength);
            SetWorldScale(
                lineObject.transform,
                new Vector3(lineWidth / spriteWidth, length / spriteLength, lineWidth));

            renderer.color = guidanceBoxColor[0];
            renderer.enabled = false;
            runtimeSegments.Add(new RuntimeLineSegment(
                renderer,
                targetBoxRenderer,
                sectionIndex,
                startPosition,
                endPosition,
                routeStartDistance,
                routeEndDistance,
                guidanceBoxColor[0]));
        }

        private void UpdateSegmentedLines()
        {
            if (!playerTransform || runtimeSegments.Count == 0)
                return;

            Vector3 currentPosition = playerTransform.position;
            if (!hasPreviousPlayerPosition)
            {
                if (LevelManager.GameState == GameStatus.Waiting)
                    ResetProgressToStart(currentPosition);
                else
                    SynchronizeProgressToPlayer(
                        currentPosition,
                        Mathf.Min(nextSegmentIndex, runtimeSegments.Count - 1));
                return;
            }

            bool isPlaying = LevelManager.GameState == GameStatus.Playing;
            if (isPlaying)
            {
                travelledDistance += DistanceXZ(previousPlayerPosition, currentPosition);
                ConsumePassedSegments();
            }

            ApplySegmentVisibility(isPlaying);
            previousPlayerPosition = currentPosition;
        }

        private void ConsumePassedSegments()
        {
            while (nextSegmentIndex < runtimeSegments.Count
                && travelledDistance >= runtimeSegments[nextSegmentIndex].RouteEndDistance)
            {
                runtimeSegments[nextSegmentIndex].SetConsumed(true);
                nextSegmentIndex++;
            }
        }

        private void ApplySegmentVisibility(bool applyFade)
        {
            float visibleRouteEnd = travelledDistance + lineVisibleDistance;
            for (int i = 0; i < runtimeSegments.Count; i++)
            {
                RuntimeLineSegment segment = runtimeSegments[i];
                if (segment.Consumed)
                {
                    segment.SetVisual(false, 0f);
                    continue;
                }

                bool visible = (segment.SectionIndex == 0 || segment.TargetBoxVisible)
                    && segment.RouteStartDistance <= visibleRouteEnd;

                float alpha = segment.CurrentAlpha;
                if (i != nextSegmentIndex)
                {
                    alpha = 1f;
                }
                else if (applyFade)
                {
                    float fadeStartDistance = Mathf.Max(
                        segment.RouteStartDistance,
                        segment.RouteEndDistance - lineFadeDistance);
                    float fadeProgress = Mathf.InverseLerp(
                        fadeStartDistance,
                        segment.RouteEndDistance,
                        travelledDistance);
                    alpha = Mathf.Min(segment.CurrentAlpha, 1f - fadeProgress);
                }

                segment.SetVisual(visible, alpha);
            }
        }

        private void ResetProgressToStart(Vector3 playerPosition)
        {
            travelledDistance = 0f;
            nextSegmentIndex = 0;
            hasPreviousPlayerPosition = true;
            previousPlayerPosition = playerPosition;

            for (int i = 0; i < runtimeSegments.Count; i++)
                runtimeSegments[i].SetConsumed(false);

            ApplySegmentVisibility(false);
        }

        private void SynchronizeProgressToPlayer(Vector3 playerPosition, int maximumSearchIndex)
        {
            hasPreviousPlayerPosition = true;
            previousPlayerPosition = playerPosition;

            if (runtimeSegments.Count == 0)
            {
                travelledDistance = 0f;
                nextSegmentIndex = 0;
                return;
            }

            float closestDistanceSqr = float.PositiveInfinity;
            float closestRouteDistance = 0f;
            int lastSearchIndex = Mathf.Clamp(maximumSearchIndex, 0, runtimeSegments.Count - 1);
            for (int i = 0; i <= lastSearchIndex; i++)
            {
                RuntimeLineSegment segment = runtimeSegments[i];
                float distanceSqr = PointToLineDistanceSqrXZ(
                    playerPosition,
                    segment.StartPosition,
                    segment.EndPosition,
                    out float interpolation);

                if (distanceSqr < closestDistanceSqr)
                {
                    closestDistanceSqr = distanceSqr;
                    closestRouteDistance = Mathf.Lerp(
                        segment.RouteStartDistance,
                        segment.RouteEndDistance,
                        interpolation);
                }
            }

            travelledDistance = closestRouteDistance;
            nextSegmentIndex = runtimeSegments.Count;
            for (int i = 0; i < runtimeSegments.Count; i++)
            {
                bool consumed = runtimeSegments[i].RouteEndDistance <= travelledDistance;
                runtimeSegments[i].SetConsumed(consumed);
                if (!consumed && nextSegmentIndex == runtimeSegments.Count)
                    nextSegmentIndex = i;
            }

            ApplySegmentVisibility(false);
        }

        private void HandlePlayerRevived()
        {
            ResolvePlayer();
            if (playerTransform && runtimeSegments.Count > 0)
            {
                int maximumSearchIndex = Mathf.Min(nextSegmentIndex, runtimeSegments.Count - 1);
                SynchronizeProgressToPlayer(playerTransform.position, maximumSearchIndex);
            }
            else
                hasPreviousPlayerPosition = false;
        }

        private void ResolvePlayer()
        {
            Player currentPlayer = Player.Instance;
            if (player == currentPlayer && playerTransform)
                return;

            if (player && started)
                player.OnTurn.RemoveListener(CreateGuidanceBoxAtPlayer);

            player = currentPlayer;
            playerTransform = player ? player.transform : null;
            started = false;
            hasPreviousPlayerPosition = false;

            if (playerTransform && runtimeSegments.Count > 0)
            {
                if (LevelManager.GameState == GameStatus.Waiting)
                    ResetProgressToStart(playerTransform.position);
                else
                    SynchronizeProgressToPlayer(playerTransform.position, runtimeSegments.Count - 1);
            }
        }

        private void ResolveRuntimeAssets()
        {
            if (!boxPrefab)
                boxPrefab = Resources.Load<GameObject>("Prefabs/GuidanceBox");
            if (!linePrefab)
                linePrefab = Resources.Load<GameObject>("Prefabs/GuidanceLine");
            if (createBoxes && !holder && boxHolder)
                holder = boxHolder;
        }

        private void RemovePreviouslyGeneratedLines()
        {
            runtimeLineHolder = null;
            if (!boxHolder)
                return;

            for (int i = boxHolder.childCount - 1; i >= 0; i--)
            {
                Transform child = boxHolder.GetChild(i);
                if (child.name != RuntimeLineHolderName)
                    continue;

                child.gameObject.SetActive(false);
                Destroy(child.gameObject);
            }

            for (int i = 0; i < boxes.Count; i++)
            {
                GuidelineTap box = boxes[i];
                if (!box)
                    continue;

                string legacyLineName = box.name + " - Line";
                for (int childIndex = box.transform.childCount - 1; childIndex >= 0; childIndex--)
                {
                    Transform child = box.transform.GetChild(childIndex);
                    if (child.name != legacyLineName)
                        continue;

                    child.gameObject.SetActive(false);
                    Destroy(child.gameObject);
                }
            }
        }

        private float[] GetValidatedLinePattern()
        {
            if (lineLengthPattern != null && lineLengthPattern.Length > 0)
            {
                bool valid = true;
                for (int i = 0; i < lineLengthPattern.Length; i++)
                {
                    float value = lineLengthPattern[i];
                    if (value <= MinimumLineLength || float.IsNaN(value) || float.IsInfinity(value))
                    {
                        valid = false;
                        break;
                    }
                }

                if (valid)
                    return lineLengthPattern;
            }

            Debug.LogWarning(
                "[GuidanceController] Invalid line length pattern. Using 0.5, 0.2, 2, 0.2.",
                this);
            return DefaultLineLengthPattern;
        }

        private static float GetBoxInset(GuidelineTap box, Vector3 direction)
        {
            SpriteRenderer renderer = box.Renderer;
            if (!renderer)
                return 0f;

            Vector3 extents = renderer.bounds.extents;
            return Mathf.Abs(direction.x) * extents.x
                + Mathf.Abs(direction.y) * extents.y
                + Mathf.Abs(direction.z) * extents.z;
        }

        private static Quaternion CreateLineRotation(Vector3 direction)
        {
            Vector3 surfaceNormal = Vector3.ProjectOnPlane(Vector3.down, direction);
            if (surfaceNormal.sqrMagnitude <= 0.0001f)
                surfaceNormal = Vector3.ProjectOnPlane(Vector3.forward, direction);
            if (surfaceNormal.sqrMagnitude <= 0.0001f)
                surfaceNormal = Vector3.right;

            return Quaternion.LookRotation(surfaceNormal.normalized, direction);
        }

        private static void SetWorldScale(Transform target, Vector3 worldScale)
        {
            target.localScale = Vector3.one;
            Vector3 currentWorldScale = target.lossyScale;
            target.localScale = new Vector3(
                worldScale.x / SafeScale(currentWorldScale.x),
                worldScale.y / SafeScale(currentWorldScale.y),
                worldScale.z / SafeScale(currentWorldScale.z));
        }

        private static float SafeScale(float value)
        {
            float absolute = Mathf.Abs(value);
            return absolute <= 0.0001f ? 1f : absolute;
        }

        private static float PointToLineDistanceSqrXZ(
            Vector3 point,
            Vector3 lineStart,
            Vector3 lineEnd,
            out float interpolation)
        {
            Vector2 point2D = ToXZ(point);
            Vector2 start2D = ToXZ(lineStart);
            Vector2 end2D = ToXZ(lineEnd);
            Vector2 line = end2D - start2D;
            float lengthSqr = line.sqrMagnitude;
            if (lengthSqr <= 0.0001f)
            {
                interpolation = 0f;
                return (point2D - start2D).sqrMagnitude;
            }

            interpolation = Vector2.Dot(point2D - start2D, line) / lengthSqr;
            interpolation = Mathf.Clamp01(interpolation);
            Vector2 projection = start2D + line * interpolation;
            return (point2D - projection).sqrMagnitude;
        }

        private static float DistanceXZ(Vector3 a, Vector3 b)
        {
            return Vector2.Distance(ToXZ(a), ToXZ(b));
        }

        private static Vector2 ToXZ(Vector3 value)
        {
            return new Vector2(value.x, value.z);
        }

        private static float NormalizeAngle(float angle)
        {
            return Mathf.Repeat(angle, 360f);
        }

        private void OnDisable()
        {
            LevelManager.revivePlayer -= HandlePlayerRevived;

            if (player && started)
                player.OnTurn.RemoveListener(CreateGuidanceBoxAtPlayer);

            started = false;
            hasPreviousPlayerPosition = false;
        }

        private void OnDestroy()
        {
            LevelManager.revivePlayer -= HandlePlayerRevived;

            if (player && started)
                player.OnTurn.RemoveListener(CreateGuidanceBoxAtPlayer);

            if (Instance == this)
                Instance = null;
        }
        private sealed class RuntimeLineSegment
        {
            private readonly SpriteRenderer renderer;
            private readonly SpriteRenderer targetBoxRenderer;
            private readonly Color baseColor;

            public Vector3 StartPosition { get; }
            public Vector3 EndPosition { get; }
            public int SectionIndex { get; }
            public float RouteStartDistance { get; }
            public float RouteEndDistance { get; }
            public bool Consumed { get; private set; }
            public float CurrentAlpha { get; private set; } = 1f;
            public bool TargetBoxVisible => targetBoxRenderer
                && targetBoxRenderer.enabled
                && targetBoxRenderer.gameObject.activeInHierarchy
                && targetBoxRenderer.color.a > 0.001f;

            public RuntimeLineSegment(
                SpriteRenderer renderer,
                SpriteRenderer targetBoxRenderer,
                int sectionIndex,
                Vector3 startPosition,
                Vector3 endPosition,
                float routeStartDistance,
                float routeEndDistance,
                Color baseColor)
            {
                this.renderer = renderer;
                this.targetBoxRenderer = targetBoxRenderer;
                this.baseColor = baseColor;
                SectionIndex = sectionIndex;
                StartPosition = startPosition;
                EndPosition = endPosition;
                RouteStartDistance = routeStartDistance;
                RouteEndDistance = routeEndDistance;
            }

            public void SetConsumed(bool consumed)
            {
                Consumed = consumed;
                CurrentAlpha = 1f;
                SetVisual(!consumed, 1f);
            }

            public void SetVisual(bool visible, float alpha)
            {
                if (!renderer)
                    return;

                bool shouldRender = visible && !Consumed;
                if (renderer.enabled != shouldRender)
                    renderer.enabled = shouldRender;

                CurrentAlpha = Mathf.Clamp01(alpha);
                Color color = baseColor;
                color.a *= CurrentAlpha;
                if (renderer.color != color)
                    renderer.color = color;
            }
        }
    }
}
