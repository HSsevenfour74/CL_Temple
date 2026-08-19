using System;
using System.Collections.Generic;
using ConceptGames.ConceptLineOrion.Level;
using ConceptGames.ConceptLineOrion.Trigger;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ConceptGames.ConceptLineOrion.Skin
{
    public class Zues : MonoBehaviour
    {
        public GameObject tailPrefab;
        private Transform tail;
        private Vector3 tailPosition;
        private float TailDistance =>
            new Vector2(tailPosition.x - Player.Instance.selfTransform.position.x, tailPosition.z - Player.Instance.selfTransform.position.z).magnitude;

        private void Start()
        {
            CreateTail();
        }
        private void Update()
        {
            if (LevelManager.GameState == GameStatus.Playing)
            {
                if (tail)
                {
                    tail.position = (tailPosition + Player.Instance.selfTransform.position) * 0.5f;
                    tail.localScale = new Vector3(tail.localScale.x, tail.localScale.y, TailDistance);
                    tail.position = new Vector3(tail.position.x, Player.Instance.selfTransform.position.y, tail.position.z);
                    tail.LookAt(Player.Instance.selfTransform);
                }
            }

        }
        internal void CreateTail()
        {
            var now = Quaternion.Euler(Player.Instance.selfTransform.localEulerAngles);
            var offset = tailPrefab.transform.localScale.z * 0.5f;

            if (tail)
            {
                var last = Quaternion.Euler(tail.transform.localEulerAngles);
                var angle = Quaternion.Angle(last, now);
                if (angle is >= 0f and <= 90f) offset = 0.5f * Mathf.Tan(Mathf.PI / 180f * angle * 0.5f);
                else offset = -0.5f * Mathf.Tan(Mathf.PI / 180f * ((180f - angle) * 0.5f));
                var end = tailPosition + last * Vector3.forward * (TailDistance + offset);
                tail.position = (tailPosition + end) * 0.5f;
                tail.position = new Vector3(tail.position.x, Player.Instance.selfTransform.position.y, tail.position.z);
                tail.localScale =
                    new Vector3(tail.localScale.x, tail.localScale.y, Vector3.Distance(tailPosition, end));
                tail.LookAt(Player.Instance.selfTransform.position);
            }
            tailPosition = Player.Instance.selfTransform.position + now * Vector3.back * Mathf.Abs(offset);
        }
    }
}
