using UnityEngine;
public class Follow : MonoBehaviour
{
    public GameObject mainObject;
    public float smoothTime = 0.5f;
    private Vector3 Velocity = Vector3.zero;
    public Vector3 Vector = Vector3.zero;
    public Vector3 Rotation = Vector3.zero;
    [HideInInspector] public Vector3 Pos;
    void Start()
    {
        transform.Rotate(Rotation);
    }
    void Update()
    {
        Pos = Vector3.SmoothDamp(transform.position, mainObject.transform.position + Vector, ref Velocity, smoothTime);
        transform.position = Pos;
    }
}