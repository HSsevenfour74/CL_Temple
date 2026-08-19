using UnityEngine;

namespace ConceptGames.ConceptLineOrion.Level
{
	[RequireComponent(typeof(ParticleSystem))]
	public class ParticlesRateOverDistanceHelper : MonoBehaviour
	{
		private ParticleSystem ps;
		private ParticleSystem.EmissionModule em;
		private Transform trans;
		private Vector3 lastPos;
		private float dist;
		private float toEmit;
		private void Awake()
		{
			trans = transform;
			lastPos = trans.position;
			ps = GetComponent<ParticleSystem>();
			em = ps.emission;
		}

		private void Update()
		{
			if (ps.isEmitting && em.rateOverDistanceMultiplier > 0f)
			{
				if (LevelManager.GameState == GameStatus.Playing)
				{
					lastPos = trans.position;
					dist = toEmit = 0f;
					return;
				}
				dist += Vector3.Distance(trans.position, lastPos);
				toEmit += em.rateOverDistanceMultiplier * dist;
				if (toEmit >= 1f)
				{
					Emit(toEmit);
				}
				lastPos = trans.position;
			}
		}
		private void Emit(float emit)
		{
			dist = toEmit = 0f;
			ps.Emit(Mathf.RoundToInt(emit));
		}

	}
}
