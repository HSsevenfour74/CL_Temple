using UnityEngine;

public class StalagmitSkin : MonoBehaviour
{

    public float startSize = 1f;

    public ParticleSystem ps;

    private ParticleSystem.EmitParams changeDirParams;


    private void Awake()
    {
        changeDirParams = default(ParticleSystem.EmitParams);
        changeDirParams.startSize = startSize;
    }

    public void ChangedDirection()
    {
            ps.Emit(changeDirParams, 3);
    }
}
