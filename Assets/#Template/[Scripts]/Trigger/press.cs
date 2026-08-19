using ConceptGames.ConceptLineOrion.Level;
using UnityEngine;

namespace ConceptGames.ConceptLineOrion.Trigger
{
    public class press : MonoBehaviour
    {
        public Transform targetObject;

        // 要检测的按键
        public KeyCode targetKey = KeyCode.E;

        // 区域半径
        public float areaRadius = 5f;

        // 按键按下时要执行的事件
        public UnityEngine.Events.UnityEvent onKeyPressedInArea;

        void Update()
        {
            // 检查目标对象是否在区域内
            bool isInArea = IsInArea();

            // 当在区域内且按下目标按键时执行操作
            if (isInArea && Input.GetKeyDown(targetKey))
            {
                onKeyPressedInArea?.Invoke();
                Debug.Log($"在世界区域内按下了{targetKey}键");
            }
        }

        // 检查目标对象是否在区域内
        private bool IsInArea()
        {
            if (targetObject == null) return false;

            float distance = Vector3.Distance(transform.position, targetObject.position);
            return distance <= areaRadius;
        }

        // 在Scene视图中绘制区域范围
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, areaRadius);
        }
    }
}