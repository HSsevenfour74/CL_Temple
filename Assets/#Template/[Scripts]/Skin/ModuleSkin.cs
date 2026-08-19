using System;
using UnityEngine;
public class ModuleSkin : MonoBehaviour
{
	public ParticleSystem[] pss;

	public Transform resultTransform;

	private void Update()
	{
		for (int i = 0; i < pss.Length; i++)
		{
			ParticleSystem.MainModule main = pss[i].main;
			main.startRotationYMultiplier = resultTransform.rotation.eulerAngles.y * (float)Math.PI / 180f;
		}
	}
}
