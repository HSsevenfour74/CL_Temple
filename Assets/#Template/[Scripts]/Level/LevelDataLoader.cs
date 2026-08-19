using System;
using UnityEngine;

namespace ConceptGames.ConceptLineOrion.Level
{
    public class LevelDataLoader : MonoBehaviour
    {
        public LevelData _1;
        public LevelData _2;

        void Start()
        {
            if (PlayerPrefs.GetInt($"Level_{Player.Instance.levelData.levelTitle}_ChangeMusic") == 0)
            {
                Player.Instance.levelData = _1;
            }
            else
            {
                Player.Instance.levelData = _2;
            }
        }
    }
}
