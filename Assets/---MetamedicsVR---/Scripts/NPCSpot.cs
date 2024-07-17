using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpot : MonoBehaviour
{

    public SpotType type;

    public enum SpotType
    {
        Generic,
        Compressions,
        Ventilations,
    }
}
