using ConceptGames.ConceptLineOrion.Level;
using UnityEngine;

namespace ConceptGames.ConceptLineOrion.Trigger
{
    [DisallowMultipleComponent, RequireComponent(typeof(Collider), typeof(MeshRenderer))]
    public class Gem : MonoBehaviour
    {
        [SerializeField] private bool fake;

        private Player player;
        private GameObject effectPrefab;
        private GameObject effect;
        private bool got;
        private int index;

        private MeshRenderer MeshRenderer => GetComponent<MeshRenderer>();

        private void Start()
        {
            MeshRenderer.enabled = true;
            effectPrefab = Resources.Load<GameObject>("Prefabs/GetGem");
            player = Player.Instance;
        }

        private void Update()
        {
            transform.Rotate(Vector3.up, Time.deltaTime * 60f);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player") || got || fake) return;
            got = true;
            player.Events?.Invoke(6);
            MeshRenderer.enabled = false;
            index = player.Checkpoints.Count;
            if (QualitySettings.GetQualityLevel() > 1)
                effect = Instantiate(effectPrefab, transform.position, Quaternion.Euler(-90, 0, 0));
            player.BlockCount++;
            LevelManager.revivePlayer += ResetData;
        }

        private void ResetData()
        {
            LevelManager.revivePlayer -= ResetData;
            LevelManager.CompareCheckpointIndex(index, () =>
            {
                got = false;
                MeshRenderer.enabled = true;
            });
            if (effect) Destroy(effect);
        }

        private void OnDestroy()
        {
            LevelManager.revivePlayer -= ResetData;
        }
    }
}