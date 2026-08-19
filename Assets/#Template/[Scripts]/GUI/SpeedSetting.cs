using ConceptGames.ConceptLineOrion.UI;
using UnityEngine;
using UnityEngine.UI;
namespace ConceptGames.ConceptLineOrion.Level
{
    public class SpeedSetting : MonoBehaviour
    {
        public float speed = 1f;
        public int level = 0;
        public GameObject pro;
        GameObject a;
        private Text b;
        public static SpeedSetting Instance { get; private set; }
        private void Awake()
        {
            Instance = this;
        }
        private void Start()
        {
            a = Instantiate(pro);
            b = a.GetComponentInChildren<Text>();
            ChangeSpeed();
        }
        public void ChangeSpeed()
        {
            switch (level)
            {
                case -3:
                    speed = 0.25f;
                    a.SetActive(true);
                    break;
                case -2:
                    speed = 0.5f;
                    a.SetActive(true);
                    break;
                case -1:
                    speed = 0.75f;
                    a.SetActive(true);
                    break;
                case 0:
                    speed = 1f;
                    a.SetActive(false);
                    break;
                case 1:
                    speed = 1.25f;
                    a.SetActive(true);
                    break;
                case 2:
                    speed = 1.5f;
                    a.SetActive(true);
                    break;
                case 3:
                    speed = 2f;
                    a.SetActive(true);
                    break;
            }
            Time.timeScale = speed;
            b.text = speed + "xSpeed";
        }
        public void SetLevel(bool a)
        {
            if (a)
            {
                if (level < 3)
                    level += 1;
            }
            else
            {
                if (level > -3)
                    level -= 1;
            }
            ChangeSpeed();
        }
    }
}