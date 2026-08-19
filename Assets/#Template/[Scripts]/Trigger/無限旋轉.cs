using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SDOW.SB腳本
{
    public class 無限旋轉 : MonoBehaviour
    {
        public float 速度;
        public 向 方向;
        public enum 向{
            上,下,左,右,前,後
        }
        public Vector3 方方向向{
            
            get{
                Vector3 V = new Vector3();
                if(方向 == 向.上){
                    V =  Vector3.up;
                }
                else if(方向 ==向.下){
                    V =  Vector3.down;
                }
                else if(方向 ==向.左){
                    V =  Vector3.left;
                }
                else if(方向 ==向.右){
                    V =  Vector3.right;
                }
                else if(方向 ==向.前){
                    V =  Vector3.forward;
                }else if(方向 ==向.後)
                {
                    V =  Vector3.back;
                }
                return V;
            }
        }
        void Update()
        {
            transform.Rotate(方方向向, 速度*Time.deltaTime);
        }
    }

}
