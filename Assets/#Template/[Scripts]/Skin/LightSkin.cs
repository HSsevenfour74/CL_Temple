using UnityEngine;
using ConceptGames.ConceptLineOrion.Level;

namespace ConceptGames.ConceptLineOrion.Skin
{
public class LightSkin : MonoBehaviour
{
    private ParticleSystem lightskin;

    private void Start()
    {
        lightskin = this.GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (this.isActiveAndEnabled)
        {
            ParticleSystem.MainModule main = lightskin.main;
            main.startColor = Player.Instance.characterMaterial.color;
        }
    }
}
}
