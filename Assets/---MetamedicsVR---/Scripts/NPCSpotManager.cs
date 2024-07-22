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
}
