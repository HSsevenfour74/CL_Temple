using UnityEngine;
using ConceptGames.ConceptLineOrion.Level;

namespace ConceptGames.ConceptLineOrion.Skin
{
    public class NaheedaAnim : MonoBehaviour
    {
        private Animator anim;
        public bool isPreview = false;
        void Start()
        {
            anim = GetComponent<Animator>();
        }
        void Update()
        {
            UpdateAnim();
        }
        public void UpdateAnim()
        {
            if (isPreview)
            {
                anim.SetBool("Normal", false);
                anim.SetBool("Falling", false);
                anim.SetBool("Moving", true);
            }
            if (LevelManager.GameState == GameStatus.Waiting && !isPreview)
            {
                anim.SetBool("Normal", true);
                anim.SetBool("Falling", false);
                anim.SetBool("Moving", false);
            }
            if (LevelManager.GameState == GameStatus.Playing)
            {
                anim.SetBool("Normal", false);
                anim.SetBool("Falling", false);
                anim.SetBool("Moving", true);
            }
            if (LevelManager.GameState == GameStatus.Playing && Player.previousFrameIsGrounded == true)
            {
                anim.SetBool("Normal", false);
                anim.SetBool("Falling", true);
                anim.SetBool("Moving", true);
            }
        }
    }
}