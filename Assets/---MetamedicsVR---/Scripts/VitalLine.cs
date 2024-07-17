using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VitalLine : MonoBehaviour
{
    public Transform lineStart;
    public Transform lineEnd;
    public Transform lineUp;
    public LineRenderer lineRenderer;

    private List<Tuple<float, float>> points = new List<Tuple<float, float>>();
    public float totalTimeInLine = 3;

    public Vector3[] pulse;

    private void Update()
    {
        UpdateLineTime();
        AddRandomPoint();
        UpdateLineRenderer();
    }

    private void UpdateLineTime()
    {
        while (points.Count > 0 && Time.time - points[0].getItem1() > totalTimeInLine)
        {
            points.RemoveAt(0);
        }
    }

    private void AddRandomPoint()
    {
        if (points.Count > 0)
        {
            float lastValue = points[points.Count - 1].getItem2();
            points.Add(new Tuple<float, float>(Time.time, Mathf.Clamp(Random.Range(lastValue - 0.1f, lastValue + 0.1f), -1, 1)));
        }
        else
        {
            points.Add(new Tuple<float, float>(Time.time, Random.Range(-1f, 1f)));
        }
    }

    private void UpdateLineRenderer()
    {
        Vector3 upVector = GetClosestPointOnLine(lineStart.position, lineEnd.position, lineUp.position) - lineUp.position;
        Vector3[] positions = new Vector3[points.Count];
        for (int i = 0; i < points.Count; i++)
        {
            positions[i] = Vector3.Lerp(lineStart.position, lineEnd.position, (Time.time - points[i].getItem1())/totalTimeInLine) + upVector * points[i].getItem2();
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
