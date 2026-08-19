using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using ConceptGames.ConceptLineOrion.Level;
public class MoveOnePos : MonoBehaviour
{
    public GameObject[] Object= {null};//物体
    public enum pos { X,Y,Z}//选择一个轴
    public Ease Ease = Ease.InOutSine;//缓动
    public pos PosChoose;
    public float value, posTime;//值和移动时间
    // Start is called before the first frame update
    public void OnTriggerEnter(Collider collider)
    {
        if (collider.GetComponent<Player>())
        {
            if(PosChoose==pos.X)
            {
                for(int i=0;i<Object.Length;i++)
                {
                    Object[i].transform.DOMoveX(value,posTime).SetEase(Ease);
                }
                
            }
            else if(PosChoose==pos.Y)
            {
                for (int i = 0; i < Object.Length; i++)
                {
                    Object[i].transform.DOMoveY(value, posTime).SetEase(Ease);
                }
            }
            else if (PosChoose == pos.Z)
            {
                for (int i = 0; i < Object.Length; i++)
                {
                    Object[i].transform.DOMoveZ(value, posTime).SetEase(Ease);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
