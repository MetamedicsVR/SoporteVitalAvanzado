using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Joystick : MonoBehaviour
{
    public Transform handle;
    public float maxDistance = 0.03f;

    private Vector3 handleRestPosition;

    private void Awake()
    {
        handleRestPosition = handle.localPosition;
    }

    public Vector2 GetValues()
    {
        Vector2 handleValue = new Vector2(handle.localPosition.x, handle.localPosition.z);
        handleValue /= Mathf.Max(handleValue.magnitude, maxDistance);
        return handleValue;
    }

    public void UnGrab()
    {
        handle.localPosition = handleRestPosition;
    }
}
