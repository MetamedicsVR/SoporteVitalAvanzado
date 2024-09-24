using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPCSpotManager : MonoBehaviourInstance<NPCSpotManager>
{
    public NPCSpot[] spots;

    public NPCSpot GetNearestSpot(NPCSpot.SpotType spotType, Vector3 position)
    {
        NPCSpot availableSpot = spots .Where(spot => spot.type == spotType && spot.npcInSpot == null) .OrderBy(spot => Vector3.Distance(spot.transform.position, position)) .FirstOrDefault();
        if (availableSpot)
        {
            return availableSpot;
        }
        return spots.Where(spot => spot.type == spotType).OrderBy(spot => Vector3.Distance(spot.transform.position, position)).FirstOrDefault();
    }
}
