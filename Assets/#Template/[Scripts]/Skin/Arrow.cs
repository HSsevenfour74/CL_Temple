using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class Arrow : MonoBehaviour
{
	public float distanceOffset;

	private ParticleSystem ps;

	private ParticleSystem.EmissionModule em;

	private Transform trans;

	private Vector3 lastPos;

	private float dist;

	private float toEmit;

	private void Awake()
	{
		trans = base.transform;
		lastPos = trans.position;
		ps = base.gameObject.GetComponent<ParticleSystem>();
		em = ps.emission;
	}

	private void OnEnable()
	{
		Clear();
	}

	private void Update()
	{
		if (ps.isEmitting && em.rateOverDistanceMultiplier > 0f)
		{
			dist += Vector3.Distance(trans.position, lastPos);
			toEmit += em.rateOverDistanceMultiplier * dist;
			if (toEmit >= 1f + distanceOffset)
			{
				Emit(toEmit);
			}
			lastPos = trans.position;
		}
		else
		{
			Clear();
		}
	}

	public void Clear()
	{
		lastPos = trans.position;
		dist = (toEmit = 0f);
	}

	private void Emit(float emit)
	{
		dist = (toEmit = 0f);
		ps.Emit(Mathf.RoundToInt(emit));
	}
}
