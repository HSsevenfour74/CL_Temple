using UnityEngine;
using ConceptGames.ConceptLineOrion.Level;
using ConceptGames.ConceptLineOrion.Skin;
using System;

namespace ConceptGames.ConceptLineOrion.Trigger
{
    public class OffAllSkin : MonoBehaviour
    {
        private GameObject headphone;
        private GameObject headphoneromance;
        private GameObject cloud;
        private GameObject naheeda;
        private GameObject ball;
        private GameObject lights;
        private GameObject slither;
        private GameObject paper;
        private GameObject chaos;
        private GameObject zues;
        private GameObject module;
        private GameObject stalagmit;
        private GameObject arrow;
        private GameObject mountainnew;
        private GameObject mountainold;
        private GameObject chinesebrush;
        public bool original;

        public void Start()
        {
            headphone = GameObject.Find("LevelHolder/Objects/PlayerHolder/Player/Headphone");
            headphoneromance = GameObject.Find("LevelHolder/Objects/PlayerHolder/Player/HeadphoneRomance");
            cloud = GameObject.Find("LevelHolder/Objects/PlayerHolder/Player/Cloud");
            naheeda = GameObject.Find("LevelHolder/Objects/PlayerHolder/Player/Naheeda");
            ball = GameObject.Find("LevelHolder/Objects/PlayerHolder/Player/Ball");
            lights = GameObject.Find("LevelHolder/Objects/PlayerHolder/Player/Light");
            slither = GameObject.Find("LevelHolder/Objects/PlayerHolder/Player/Slither");
            paper = GameObject.Find("LevelHolder/Objects/PlayerHolder/Player/Paper");
            chaos = GameObject.Find("LevelHolder/Objects/PlayerHolder/Player/Chaos");
            zues = GameObject.Find("LevelHolder/Objects/PlayerHolder/Player/Zues");
            module = GameObject.Find("LevelHolder/Objects/PlayerHolder/Player/Module");
            stalagmit = GameObject.Find("LevelHolder/Objects/PlayerHolder/Player/Stalagmit");
            arrow = GameObject.Find("LevelHolder/Objects/PlayerHolder/Player/Arrow");
            mountainnew = GameObject.Find("LevelHolder/Objects/PlayerHolder/Player/MountainNew");
            mountainold = GameObject.Find("LevelHolder/Objects/PlayerHolder/Player/Mountain");
            chinesebrush = GameObject.Find("LevelHolder/Objects/PlayerHolder/Player/Brush");
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && original)
            {
                SetSkin();
            }
            if (other.CompareTag("Player") && !original)
            {
                OpenEmpty();
            }
                
        }
        public void SetSkin()
        {
            switch (PlayerPrefs.GetString("SkinID"))
            {
                case "Normal":
                    OpenNormal();
                    break;
                case "Headphone":
                    OpenHeadphone();
                    break;
                case "Cloud":
                    OpenCloud();
                    break;
                case "Naheeda":
                    OpenNaheeda();
                    break;
                case "Ball":
                    OpenBall();
                    break;
                case "Light":
                    OpenLight();
                    break;
                case "MountainNew":
                    OpenMountainNew();
                    break;
                case "Mountain":
                    OpenMountain();
                    break;
                case "Paper":
                    OpenPaper();
                    break;
                case "Chaos":
                    OpenChaos();
                    break;
                case "Zues":
                    OpenZues();
                    break;
                case "HeadphoneRomance":
                    OpenHeadphoneRomance();
                    break;
                case "Empty":
                    OpenEmpty();
                    break;
                case "Module":
                    OpenModule();
                    break;
                case "Stalagmit":
                    OpenStalagmit();
                    break;
                case "ChineseBrush":
                    OpenBrush();
                    break;
                default:
                    OpenNormal();
                    break;
            }
        }
        public void OffSkin()
        {
            headphone.SetActive(false);
            cloud.SetActive(false);
            naheeda.SetActive(false);
            ball.SetActive(false);
            lights.SetActive(false);
            paper.SetActive(false);
            zues.SetActive(false);
            module.SetActive(false);
            headphoneromance.SetActive(false);
            stalagmit.SetActive(false);
            chaos.SetActive(false);
            mountainold.SetActive(false);
            mountainnew.SetActive(false);
            chinesebrush.SetActive(false);
        }
        public void OpenEmpty()
        {
            OffSkin();
            ShowLineBody(false);
        }
        public void OpenNormal()
        {        
            OffSkin();
            ShowLineBody(true);
        }
        public void OpenHeadphone()
        {
            OffSkin();
            ShowLineBody(true);
            headphone.SetActive(true);
        }
        public void OpenChaos()
        {
            OffSkin();
            ShowLineBody(false);
            chaos.SetActive(true);
        }
        public void OpenZues()
        {
            OffSkin();
            ShowLineBody(false);
            zues.SetActive(true);
        }
        public void OpenCloud()
        {
            OffSkin();
            ShowLineBody(false);
            cloud.SetActive(true);
        }
        public void OpenNaheeda()
        {
            OffSkin();
            ShowLineBody(false);
            naheeda.SetActive(true);
        }
        public void OpenBall()
        {
            OffSkin();
            ShowLineBody(false);
            ball.SetActive(true);
        }
        public void OpenLight()
        {
            OffSkin();
            ShowLineBody(false);
            lights.SetActive(true);
        }
        public void OpenSlither()
        {
            OffSkin();
            ShowLineBody(false);
            slither.SetActive(true);
        }
        public void OpenPaper()
        {
            OffSkin();
            ShowLineBody(false);
            paper.SetActive(true);
        }
        public void OpenHeadphoneRomance()
        {
            OffSkin();
            ShowLineBody(true);
            headphoneromance.SetActive(true);
        }
        public void OpenMountain()
        {
            OffSkin();
            ShowLineBody(false);
            mountainold.SetActive(true);
        }
        public void OpenMountainNew()
        {
            OffSkin();
            ShowLineBody(false);
            mountainnew.SetActive(true);
        }
        public void OpenArrow()
        {
            OffSkin();
            ShowLineBody(false);
            arrow.SetActive(true);
        }
        public void OpenStalagmit()
        {
            OffSkin(); 
            ShowLineBody(false);
            stalagmit.SetActive(true);
        }
        public void OpenModule()
        {
            OffSkin();
            ShowLineBody(false);
            module.SetActive(true);
        }
        public void OpenBrush()
        {
            OffSkin();
            ShowLineBody(false);
            chinesebrush.SetActive(true);
        }
        public void ShowLineBody(bool active)
        {
            GameObject.Find("LevelHolder/Objects/PlayerHolder/Player").GetComponent<MeshRenderer>().enabled = active;
            Player.Instance.tailPrefab.GetComponent<MeshRenderer>().enabled = active;
        }
    }
}
