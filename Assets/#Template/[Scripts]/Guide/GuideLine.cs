using DG.Tweening;
using UnityEngine;

namespace ConceptGames.ConceptLineOrion.Guide
{
    public class GuideLine : MonoBehaviour
    {

        [SerializeField]
        private SpriteRenderer line;

        public float visibleTime = -100f;

        public float EndPosTime;

        public int ColorIndex;

        public Transform endPos;

        private SpriteRenderer[] spRenderers;

        public float[] lengthPattern;

        public void Start()
        {
            if (base.transform.localScale.x > 0f && lengthPattern != null && lengthPattern.Length > 0)
            {
                float num = Vector3.Distance(base.transform.position, endPos.position);
                if (lengthPattern[0] < num)
                {
                    GameObject gameObject = null;
                    Vector3 pos = SetLineFromSetting(line.gameObject, lengthPattern[0], endPos.localPosition);
                    int num2 = 1;
                    while (pos.x < 0f)
                    {
                        num2 %= lengthPattern.Length;
                        if (num2 % 2 == 0)
                        {
                            gameObject = Object.Instantiate(line.gameObject, line.transform.parent);
                            gameObject.transform.DOScaleY(line.transform.localScale.y, 0f);
                            pos = SetLineFromSetting(gameObject, lengthPattern[num2], pos);
                        }
                        else
                        {
                            pos.x += lengthPattern[num2] / base.transform.localScale.x;
                        }
                        num2++;
                    }
                    if (gameObject != null)
                    {
                        Vector3 localScale = gameObject.transform.localScale;
                        pos = gameObject.transform.localPosition;
                        pos.x -= localScale.x / 2f;
                        localScale.x = Mathf.Abs(pos.x);
                        pos.x /= 2f;
                        gameObject.transform.localScale = localScale;
                        gameObject.transform.localPosition = pos;
                    }
                }
            }
            spRenderers = GetComponentsInChildren<SpriteRenderer>();
        }

        private Vector3 SetLineFromSetting(GameObject go, float xScale, Vector3 pos)
        {
            Vector3 localScale = line.transform.localScale;
            localScale.x = 1f;
            go.transform.localScale = Vector3.one;
            go.transform.localScale = new Vector3(xScale / go.transform.lossyScale.x, localScale.y / go.transform.lossyScale.y, localScale.z / go.transform.lossyScale.z);
            pos.x += xScale / base.transform.localScale.x / 2f;
            go.transform.localPosition = pos;
            pos.x += xScale / base.transform.localScale.x / 2f;
            return pos;
        }
    }
}
