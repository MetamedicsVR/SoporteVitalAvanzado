using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class VitalLine : MonoBehaviour
{
    public Transform lineStart;
    public Transform lineEnd;
    public Transform lineUp;
    public LineRenderer lineRenderer;

    public DisplayMode displayMode = DisplayMode.Move;
    public float totalTimeInLine = 3;
    public float timeNoise = 0.1f;
    public float pulseNoise = 0.1f;

    public List<Tuple<float, float>> normalPulse;
    public List<Tuple<float, float>> ventriculparFibrilation;
    public List<Tuple<float, float>> testPulse;

    private List<Tuple<float, float>> currentPoints = new List<Tuple<float, float>>();
    private PulseType currentPulseType = PulseType.Test;

    public enum PulseType
    {
        Dead,
        Normal,
        VentricularFibrillation,
        Random,
        Test
    }

    public enum DisplayMode
    {
        Move,
        Sweep
    }

    public void ChangePulseType(PulseType pulseType)
    {
        currentPulseType = pulseType;
    }


    private void Update()
    {
        if (Application.isPlaying)
        {
            switch (displayMode)
            {
                case DisplayMode.Move:
                    RemoveOldPoints();
                    AddNextPoint();
                    MovingLineRenderer();
                    break;
                case DisplayMode.Sweep:

                    break;
            }
        }
        else
        {
            PrintPulse();
        }
    }

    private void RemoveOldPoints()
    {
        while (currentPoints.Count > 0 && Time.time - currentPoints[0].item1 > totalTimeInLine)
        {
            currentPoints.RemoveAt(0);
        }
    }

    private void AddNextPoint()
    {
        switch (currentPulseType)
        {
            case PulseType.Dead:
                currentPoints.Add(new Tuple<float, float>(Time.time, 0.25f));
                break;
            case PulseType.Normal:
                AddPulsePoint(normalPulse);
                break;
            case PulseType.VentricularFibrillation:
                AddPulsePoint(ventriculparFibrilation);
                break;
            case PulseType.Random:
                if (currentPoints.Count > 0)
                {
                    float lastValue = currentPoints[currentPoints.Count - 1].item2;
                    currentPoints.Add(new Tuple<float, float>(Time.time, Mathf.Clamp(Random.Range(lastValue - 0.1f, lastValue + 0.1f), -1, 1)));
                }
                else
                {
                    currentPoints.Add(new Tuple<float, float>(Time.time, Random.Range(-1f, 1f)));
                }
                break;
            case PulseType.Test:
                AddPulsePoint(testPulse);
                break;
        }
    }

    private void AddPulsePoint(List<Tuple<float, float>> pulse)
    {
        float pulseTime = Time.time % pulse[pulse.Count - 1].item1;
        for (int i = 0; i < pulse.Count - 1; i++)
        {
            if (pulseTime >= pulse[i].item1 && pulseTime <= pulse[i + 1].item1)
            {
                float t = (pulseTime - pulse[i].item1) / (pulse[i + 1].item1 - pulse[i].item1);
                float pointValue = Mathf.Lerp(pulse[i].item2, pulse[i + 1].item2, t);
                //currentPoints.Add(new Tuple<float, float>(Time.time, Mathf.Clamp(Random.Range(pointValue - pulseNoise, pointValue + pulseNoise), -1, 1)));
                currentPoints.Add(new Tuple<float, float>(Time.time, pointValue));
                return;
            }
        }
    }

    private void MovingLineRenderer()
    {
        Vector3 upVector = GetClosestPointOnLine(lineStart.position, lineEnd.position, lineUp.position) - lineUp.position;
        Vector3[] positions = new Vector3[currentPoints.Count];
        for (int i = 0; i < currentPoints.Count; i++)
        {
            positions[i] = Vector3.Lerp(lineStart.position, lineEnd.position, (Time.time - currentPoints[i].item1) / totalTimeInLine) + upVector * currentPoints[i].item2;
        }
        lineRenderer.positionCount = positions.Length;
        lineRenderer.SetPositions(positions);
    }

    private void PrintPulse()
    {
        Vector3 upVector = GetClosestPointOnLine(lineStart.position, lineEnd.position, lineUp.position) - lineUp.position;
        Vector3[] positions = new Vector3[testPulse.Count];
        for (int i = 0; i < testPulse.Count; i++)
        {
            positions[i] = Vector3.Lerp(lineStart.position, lineEnd.position, testPulse[i].item1 / totalTimeInLine) + upVector * testPulse[i].item2;
        }
        lineRenderer.positionCount = positions.Length;
        lineRenderer.SetPositions(positions);
    }

    private Vector3 GetClosestPointOnLine(Vector3 A, Vector3 B, Vector3 P)
    {
        Vector3 AP = P - A;
        Vector3 AB = B - A;
        float magnitudeAB = AB.sqrMagnitude;
        float ABAPproduct = Vector3.Dot(AP, AB);
        float distance = ABAPproduct / magnitudeAB;
        return A + AB * distance;
    }
}
