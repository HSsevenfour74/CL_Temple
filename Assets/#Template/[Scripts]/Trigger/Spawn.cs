using UnityEngine;

namespace ConceptGames.ConceptLineOrion.Trigger
{
    [DisallowMultipleComponent, RequireComponent(typeof(Collider))]
    public class Spawn : MonoBehaviour
    {
        [SerializeField] public GameObject spawnObj;
        [SerializeField] public Vector3 spawnPos = new Vector3();
        [SerializeField] public float duration = 0f;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Destroy(Instantiate(spawnObj, spawnPos, Quaternion.identity), duration);
            }
        }
    }
}
//这是一个触发器脚本，放在触发器上，然后填入生成物、生成的坐标、存在时间就行了
