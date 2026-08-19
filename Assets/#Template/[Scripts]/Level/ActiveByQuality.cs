using Sirenix.OdinInspector;
using UnityEngine;

namespace ConceptGames.ConceptLineOrion.Level
{
    public enum ActiveType
    {
        Bigger,
        Smaller
    }

    public enum QualityLevel
    {
        Low,
        Medium,
        High,
        Ultra
    }

    [DisallowMultipleComponent]
    public class ActiveByQuality : MonoBehaviour
    {
        [SerializeField, EnumToggleButtons, InfoBox("$message"), DisableInPlayMode] private ActiveType activeType = ActiveType.Bigger;
        [SerializeField, EnumToggleButtons, DisableInPlayMode] private QualityLevel targetLevel = QualityLevel.Medium;

        private string message;

        public void Awake()
        {
            int i;

            switch (targetLevel)
            {
                case QualityLevel.Low: i = 0; break;
                case QualityLevel.Medium: i = 1; break;
                case QualityLevel.High: i = 2; break;
                case QualityLevel.Ultra: i = 3; break;
                default: i = -1; break;
            }
            if (activeType == ActiveType.Bigger) 
                if (QualitySettings.GetQualityLevel() > i) 
                    gameObject.SetActive(true); 
                else
                    Destroy(this.gameObject);
            if (activeType == ActiveType.Smaller)
                if (QualitySettings.GetQualityLevel() < i)
                    gameObject.SetActive(true);
                else
                    Destroy(this.gameObject);
        }

        private void OnValidate()
        {
            string text1;
            string text2;
            string text3;

            if (activeType == ActiveType.Bigger)
            {  
                text2 = "高于";
            }
            else
            {
                text2 = "低于";
            }
            text1 = "显示";
            text3 = targetLevel.ToString();

            message = "当画质" + text2 + text3 + "时" + text1;
        }
    }
}