using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using ConceptGames.ConceptLineOrion.Level;
using System;

namespace ConceptGames.ConceptLineOrion.UI
{
    public class Share : MonoBehaviour
    {
        public Text Percentage_Text;
        public Text Level_Title;
        public Text Diamont_Got_Text;
        public Image Percentage_Fill;
        public GameObject[] Crowns;

        public Camera ScreenShotCamera;

        private Camera MainCamera;

        public RawImage image3dRender;

        private Texture2D screenshotTexture2D;

        private Texture2D screenshotTexture3D;

        [SerializeField]
        private RenderTexture screenshotRenderTexture;

        public static Share Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public void TakeScreenShot()
        {
            MainCamera = Camera.main;
            for(int i = 0;  i < Player.Instance.CrownCount; i++)
            {
                Crowns[i].SetActive(true);
            }
            Percentage_Text.text = LevelUI.Instance.percentage.text;
            Level_Title.text = LevelUI.Instance.title.text;
            Diamont_Got_Text.text = LevelUI.Instance.block.text;
            Percentage_Fill.fillAmount = float.Parse(LevelUI.Instance.percentage.text.Substring(0, LevelUI.Instance.percentage.text.Length - 1)) / 100;
            MainCamera.targetTexture = ScreenShotCamera.targetTexture;
            TakeSnapshotBuiltIn(ScreenShotCamera);
        }

        public void ResetCamera()
        {
            MainCamera.targetTexture = null;
        }


        public int width = 1024;
        public int height = 1024;
        public void TakeSnapshotBuiltIn(Camera targetCamera)
        {
            if (targetCamera == null) return;
            RenderTexture originalRT = targetCamera.targetTexture;

            // 2. 创建临时 RT 并赋值给相机
            RenderTexture rt = RenderTexture.GetTemporary(width, height, 24);
            targetCamera.targetTexture = rt;

            // 3. 手动调用渲染
            targetCamera.Render();

            // 4. 读取像素
            RenderTexture.active = rt;
            Texture2D screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);
            screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            screenShot.Apply();

            // 5. 【完全还原】恢复相机原本的参数和激活状态
            targetCamera.targetTexture = originalRT;
            RenderTexture.active = null;

            // 6. 保存文件
            byte[] bytes = screenShot.EncodeToPNG();
            string exeFolder;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            exeFolder = $"{Path.GetDirectoryName(Application.dataPath)}\\GameScreenShot\\{Player.Instance.levelData.levelTitle}";
            if (!Directory.Exists(exeFolder)) 
                Directory.CreateDirectory(exeFolder);
#endif
#if UNITY_ANDROID
            exeFolder = Path.GetDirectoryName(Application.dataPath);
#endif
            string savePath = Path.Combine(exeFolder, $"Level{Player.Instance.levelData.levelTitle}.png");
            File.WriteAllBytes(savePath, bytes);
            // 7. 释放内存
            Destroy(screenShot);
            RenderTexture.ReleaseTemporary(rt);
        }

    }
}