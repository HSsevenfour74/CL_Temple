using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConceptGames.ConceptLineOrion.Animated
{
    public class FloatAnimator : MonoBehaviour
    {
        [SerializeField] public float amplitude = 0.5f; //这是上下浮动的幅度
        [SerializeField] public float frequency = 1f; //这是上下浮动的速度，填0不浮动
        [SerializeField] public float rotationSpeedY = 0f; //这是旋转速度，填0不旋转
        [SerializeField] public float rotationSpeedX = 0f;
        [SerializeField] public float rotationSpeedZ = 0f;

        private Vector3 initialPosition;

        private void Start()
        {
            initialPosition = transform.position;
        }

        private void Update()
        {
            float newY = initialPosition.y + amplitude * Mathf.Sin(Time.time * frequency);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            transform.Rotate(new Vector3(rotationSpeedX * Time.deltaTime, rotationSpeedY * Time.deltaTime, rotationSpeedZ * Time.deltaTime), Space.Self);
        }
    }
}
//这是一个动画脚本，直接放在你想要使它浮动的物体上