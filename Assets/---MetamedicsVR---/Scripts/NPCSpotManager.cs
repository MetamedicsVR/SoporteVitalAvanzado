using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPCSpotManager : MonoBehaviourInstance<NPCSpotManager>
{
    public NPCSpot[] spots;

    public NPCSpot GetNearestFreeSpot(NPCSpot.SpotType spotType, Vector3 position)
    {
        NPCSpot availableSpot = spots.Where(spot => spot.type == spotType && !spot.npcInSpot).OrderBy(spot => Vector3.Distance(spot.transform.position, position)).FirstOrDefault();
        if (availableSpot)
        {
            return availableSpot;
        }
        return spots.Where(spot => spot.type == spotType).OrderBy(spot => Vector3.Distance(spot.transform.position, position)).FirstOrDefault();
    }

    public NPCSpot GetAnyFreeSpot(NPCSpot.SpotType spotType)
    {
        return spots.Where(spot => spot.type == spotType && !spot.npcInSpot).FirstOrDefault();
    }

    public NPCSpot GetNearestSpot(NPCSpot.SpotType spotType, Vector3 position)
    {
        return spots.Where(spot => spot.type == spotType).OrderBy(spot => Vector3.Distance(spot.transform.position, position)).FirstOrDefault();
    }

    public NPCSpot GetAnySpot(NPCSpot.SpotType spotType)
    {
        return spots.Where(spot => spot.type == spotType).FirstOrDefault();
    }
}
