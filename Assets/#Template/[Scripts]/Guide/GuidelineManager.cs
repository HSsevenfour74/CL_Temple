using Sirenix.OdinInspector;
using System.Collections.Generic;
using ConceptGames.ConceptLineOrion.Level;
using UnityEngine;
using DG.Tweening;

namespace ConceptGames.ConceptLineOrion.Guide
{
    [DisallowMultipleComponent]
    public class GuidelineManager : MonoBehaviour
    {
        public static GuidelineManager Instance { get; private set; }
        [Title("Guideline Generator Setting"), SerializeField]
        internal Transform boxHolder;

        [SerializeField] internal List<Color> colors;
        [SerializeField, MinValue(0f)] internal float lineGap = 0.2f;

        [Title("Road Generator Setting"), SerializeField]
        private GameObject roadPrefab;

        [SerializeField] private float width = 2f;
        [SerializeField] private Vector3 offset;


        public GameObject linePrefab;
        public readonly List<GuidelineTap> boxes = new();

        internal bool useGuideline;

        private void Start()
        {
            if (!boxHolder)
                return;
            boxes.AddRange(boxHolder.GetComponentsInChildren<GuidelineTap>());
            linePrefab = Resources.Load<GameObject>("Prefabs/Guideline");
            for (int i = 0; i < boxes.Count; i++)
            {
                if (i < boxes.Count - 1 && boxes[i].haveLine)
                    GenerateLine(boxes[i].transform, boxes[i + 1].transform);
                boxes[i].InitBox();
                boxes[i].SetColor(colors);
            }
            SetUseGuideline();
        }

        private void GenerateLine(Transform box1, Transform box2)
        {
            var difference = box2.position - box1.position;
            var length = difference.magnitude - lineGap * 2 - 1.5f;
            if (!(length > 0))
                return;
            var middlePosition = (box1.position + box2.position) / 2;
            var targetRotation = Quaternion.LookRotation(difference) * Quaternion.Euler(90, 0, 90);
            Transform line;
            float epsilon = Mathf.Epsilon;
            if (Mathf.Abs(difference.x) > epsilon)
                line = Instantiate(linePrefab, new Vector3(box2.position.x - 1, box2.position.y, box2.position.z), Quaternion.Euler(-90, 0, 0)).transform;
            else if (Mathf.Abs(difference.y) > epsilon)
                line = Instantiate(linePrefab, new Vector3(box2.position.x, box2.position.y, box2.position.z - 1), Quaternion.Euler(-90, 0, 0)).transform;
            else
                line = Instantiate(linePrefab, new Vector3(box2.position.x, box2.position.y, box2.position.z - 1), Quaternion.Euler(-90, 0, 0)).transform;
            line.localScale = new Vector3(length, 0.15f, 1f);
            line.rotation = targetRotation;
            line.SetParent(boxHolder);
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
#if UNITY_EDITOR
        [Button("Create Road By Guideline Taps", ButtonSizes.Large)]
        private void CreateRoad()
        {
            if (!boxHolder)
                Debug.LogError("引导线父物体未选择");
            else
            {
                var taps = boxHolder.GetComponentsInChildren<GuidelineTap>();
                var holder = new GameObject("RoadHolder").transform;
                for (var i = 0; i < taps.Length; i++)
                {
                    if (i + 1 >= taps.Length)
                        continue;
                    var difference = taps[i + 1].transform.position - taps[i].transform.position;
                    var length = difference.magnitude;
                    var middlePosition = (taps[i].transform.position + taps[i + 1].transform.position) * 0.5f;
                    var targetRotation = Quaternion.LookRotation(difference) * Quaternion.Euler(0, 90, 0);
                    var road = Instantiate(roadPrefab, middlePosition - offset, Quaternion.Euler(Vector3.zero))
                        .transform;

                    road.localScale = new Vector3(length + width, 1f, width);
                    road.rotation = targetRotation;
                    road.transform.SetParent(holder);
                }
            }
        }
#endif
    }
}