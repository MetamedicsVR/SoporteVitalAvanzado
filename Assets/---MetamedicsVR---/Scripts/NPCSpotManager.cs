using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPCSpotManager : MonoBehaviourInstance<NPCSpotManager>
{
    public NPCSpot[] spots;

    public NPCSpot GetSpot(NPCSpot.SpotType spotType)
    {
        return spots.First(spot => spot.type == spotType);
    }

    public NPCSpot GetAvailableSpot(NPCSpot.SpotType spotType)
    {
        return spots.First(spot => spot.type == spotType && spot.available);
    }

    public NPCSpot GetNearestAvailableSpot(NPCSpot.SpotType spotType, Vector3 position)
    {
        return spots.Where(spot => spot.type == spotType && spot.available).OrderBy(spot => Vector3.Distance(spot.transform.position, position)).FirstOrDefault();
    }
}
