using System.Collections;
using System.Collections.Generic;
using ConceptGames.ConceptLineOrion.Level;
using ConceptGames.ConceptLineOrion.Trigger;
using ConceptGames.ConceptLineOrion.UI;
using Unity.VisualScripting;
using UnityEngine;

namespace ConceptGames.ConceptLineOrion.Guide
{
    [DisallowMultipleComponent]
    public class GuidelineTap : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Material material;
        [SerializeField] private Sprite sprite;
        public float triggerTime;
        [SerializeField] internal float triggerDistance = 1f;
        [SerializeField] internal int colorIndex;
        [SerializeField] internal bool haveLine = true;

        private GameObject triggerEffect;
        private readonly List<SpriteRenderer> sprites = new();

        private float DistanceSqr => (transform.position - Player.Instance.transform.position).sqrMagnitude;

        public SpriteRenderer Renderer
        {
            get
            {
                if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
                return spriteRenderer;
            }
        }


        public void SetColor(List<Color> colors)
        {
            spriteRenderer.color = colors[colorIndex];
        }

        public void InitBox()
        {
            Player.Instance.OnTurn.RemoveListener(Trigger);
            Player.Instance.OnTurn.AddListener(Trigger);
            sprites.AddRange(GetComponentsInChildren<SpriteRenderer>());
            triggerEffect = Resources.Load<GameObject>("Prefabs/GuidelineTapEffect");
            spriteRenderer.material = material;
            spriteRenderer.sprite = sprite;
            SetDisplay(true);
        }

        private void Trigger()
        {
            if (!(DistanceSqr <= triggerDistance) || !isActiveAndEnabled)
                return;
            SetDisplay(false);
            StartCoroutine(DisplayEffect());
            Player.Instance.OnTurn.RemoveListener(Trigger);
        }

        public void SetDisplay(bool active)
        {
            foreach (var VARIABLE in sprites)
            {
                VARIABLE.enabled = active;
            }
        }

        public IEnumerator DisplayEffect()
        {
            var color = Color.white;
            var scale = transform.localScale;
            var scaleVector = Vector3.one * 1.03f;
            var effect = Instantiate(triggerEffect, transform.position, Quaternion.Euler(-90, Player.Instance.firstDirection.y, 0)).transform;
            var component = effect.GetComponent<SpriteRenderer>();
            while (color.a > 0f)
            {
                yield return new WaitForSeconds(0.016f);
                color.a -= 0.03f;
                scale.Scale(scaleVector);
                component.color = color;
                effect.localScale = scale;
            }
            Destroy(effect.gameObject);
            yield return null;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player") || Player.Instance.Falling)
                return;
            var playerPosition = Player.Instance.transform.position;
            var position = transform.position;
            var normalizedPlayerPosition = new Vector3(playerPosition.x, 0, playerPosition.z);
            var normalizedPosition = new Vector3(position.x, 0, position.z);
            var time = Mathf.Abs((normalizedPlayerPosition - normalizedPosition).magnitude) / Player.Instance.Speed;
            if (time > 0)
                Invoke(nameof(TurnPlayer), (float)time);
            else TurnPlayer();
        }

        private void TurnPlayer()
        {
            Player.Instance.Turn();
        }
       
    }
}