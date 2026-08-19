using UnityEngine;

namespace ConceptGames.ConceptLineOrion.Skin
{
    public class Headphones : MonoBehaviour
    {
        public ParticleSystem ps;
        public float NoteGaps = 0.3f;
        public float GateWayDecreaseStep = 1f;
        private ParticleSystem.EmitParams ep;
        private void Start()
        {
            ep = default(ParticleSystem.EmitParams);
        }
        public void EmmitParticle()
        {
            EmmitParticle(2 + (int)(Random.value * 5f), 1f, 1f);
        }
        private void EmmitParticle(int count, float lifeTime, float velocityY)
        {
            for (int i = 0; i < count; i++)
            {
                ep.startLifetime = ps.startLifetime + lifeTime / 10f;
                ep.startSize = 1f + lifeTime / 10f;
                ep.startColor = ps.startColor + Color.white * (lifeTime / 16f);
                ep.velocity = new Vector3(0f, velocityY + (float)i, 0f);
                ps.Emit(ep, 1);
            }

        }
    }
}
