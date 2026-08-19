using System;
using UnityEngine;

namespace ConceptGames.ConceptLineOrion.Level.Egypt
{
    public class Egypt : MonoBehaviour
    {
        public LevelData day;
        public LevelData night;

        void Start()
        {
            DateTime currentTime = DateTime.Now;
            int currentHour = currentTime.Hour;
            if (currentHour >= 18 || currentHour <= 8)
            {
                Player.Instance.sceneLight.transform.eulerAngles = new Vector3(20f, 45f, 0f);
                Player.Instance.sceneLight.color = Color.blue;
                Player.Instance.Speed = 10;
                Player.Instance.levelData = night;
            }
            else
            {
                Player.Instance.sceneLight.transform.eulerAngles = new Vector3(50f, -45f, 0f);
                Player.Instance.sceneLight.color = Color.white;
                Player.Instance.Speed = 9.55f;
                Player.Instance.levelData = day;
            }
        }
    }
}
