using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class HandPoseInputs : MonoBehaviour
{
    [SerializeField]
    public InputAction pointingAction;
    public InputAction activateAction;

    public UnityEvent _pointingStarted;
    public UnityEvent _pointingEnded;
    public UnityEvent _activateStarted;
    public UnityEvent _activateEnded;

    public void PointingStart()
    {
        pointingAction.Enable();
        _pointingStarted.Invoke();
    }

    public void PointingEnd()
    {
        pointingAction.Disable();
        _pointingEnded.Invoke();
    }

    public void ActivateStart()
    {
        activateAction.Enable();
        _activateStarted.Invoke();
    }

    public void ActivateEnd()
    {
        activateAction.Disable();
        _activateEnded.Invoke();
    }
}
