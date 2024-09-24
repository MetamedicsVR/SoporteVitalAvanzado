using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpot : MonoBehaviour
{
    public SpotType type;

    public NPC npcInSpot;

    public enum SpotType
    {
        Generic,
        Compressions,
        Ventilations,
        Medication,
        Dropper,
        VitalSigns
    }

#if UNITY_EDITOR
    private float spotRadius = 0.5f;

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 1, 0);
        Gizmos.DrawSphere(transform.position, spotRadius);
        Gizmos.color = new Color(0, 0, 1);
        Gizmos.DrawSphere(transform.position + transform.forward * spotRadius, spotRadius/4);
    }
#endif
}
