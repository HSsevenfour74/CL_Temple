using ConceptGames.ConceptLineOrion.Level;
using UnityEngine;

namespace ConceptGames.ConceptLineOrion.Guide
{
    public class GuidelineTapGenerator : MonoBehaviour
    {
        [SerializeField] private Vector3 offset = new(0f, 0.4f, 0f);

        private void Start()
        {
#if UNITY_EDITOR
            var prefab = Resources.Load<GameObject>("Prefabs/GuidelineTap");
            var player = Player.Instance;
            var holder = new GameObject("GuidelineTapHolder-GeneratorCreated").transform;
            var firstBox = Instantiate(prefab, player.startPosition - offset, Quaternion.Euler(90, 0, 0))
                .GetComponent<GuidelineTap>();
            firstBox.transform.SetParent(holder);
            player.OnTurn.AddListener(() =>
            {
                var tap = Instantiate(prefab, player.transform.position - offset, Quaternion.Euler(90, 0, 0))
                    .GetComponent<GuidelineTap>();
                tap.triggerTime = AudioManager.Time;
                tap.transform.SetParent(holder);
            });
#endif
        }
    }
}