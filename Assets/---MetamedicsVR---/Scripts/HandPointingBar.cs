using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPointingBar : MonoBehaviour
{
    //public XRRayInteractor rayInteractor;
    public HandPoseInputs handPoseInputs;
    public float barTotalTime = 3;

    private float PointingStartDelay = 0.5f;
    private float PointingEndDelay = 0.5f;

    private bool isPoking;
    private bool wasPoking;
    private bool isPointing;
    private bool wasPointing;
    private bool isActivating;
    private bool wasActivating;
    private float timePokingStarted;
    private float timePokingEnded;
    private float timePointingStarted;
    private float timePointingEnded;

    public void GestureStarted()
    {
        isPoking = true;
        timePointingStarted = Time.time;
    }

    public void GestureEnded()
    {
        isPoking = false;
        timePointingEnded = Time.time;
    }

    private void Update()
    {
        UpdatePoking();
        UpdatePointing();
        UpdateActivating();
    }

    private void UpdatePoking()
    {
        if (isPoking)
        {
            if (!isPointing && Time.time - timePokingStarted >= PointingStartDelay)
            {
                isPointing = true;
                timePointingStarted = Time.time;
                handPoseInputs.PointingStart();
            }
        }
        else
        {
            if (isPointing && Time.time - timePokingEnded >= PointingStartDelay)
            {
                //if (rayInteractor.interactablesSelected.Count == 0)
                {
                    isPointing = false;
                    timePointingEnded = Time.time;
                    handPoseInputs.PointingEnd();
                }
            }
        }
        wasPoking = isPoking;
    }

    private void UpdatePointing()
    {
        wasPointing = isPointing;
    }

    private void UpdateActivating()
    {
        wasActivating = isActivating;
    }

    private void OnDisable()
    {
        isPoking = false;
        isPointing = false;
        isActivating = false;
    }
}