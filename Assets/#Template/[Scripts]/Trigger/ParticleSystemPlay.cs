using UnityEngine;
using System.Collections;

public class ParticleSystemPlay : MonoBehaviour
{
    public ParticleSystem particlesystem;
    public float starttime;

    void Start()
    {
        StartCoroutine(ParticleSystemStart());
    }
    public IEnumerator ParticleSystemStart()
    {
        particlesystem.Play();
        yield return new WaitForSeconds(starttime);
        particlesystem.Pause();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            particlesystem.Play();
        }
    }
}
