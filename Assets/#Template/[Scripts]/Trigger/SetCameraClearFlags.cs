using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCameraClearFlags : MonoBehaviour
{
    public new Camera camera;
    public CameraClearFlags flags = CameraClearFlags.SolidColor;
    public string Tag = "Player";

    /// <summary>
    /// OnTriggerEnter is called when the Collider other enters the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player")){
            camera.clearFlags = flags;
        }
    }
}
