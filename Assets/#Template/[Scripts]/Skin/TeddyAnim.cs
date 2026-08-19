using UnityEngine;
using ConceptGames.ConceptLineOrion.Level;

namespace ConceptGames.ConceptLineOrion.Skin
{
    public class TeddyAnim : MonoBehaviour
    {
        Animator anim;
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
            if (LevelManager.GameState == GameStatus.Waiting)
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
