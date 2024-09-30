using Meta.WitAi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class NPCManager : MonoBehaviourInstance<NPCManager>
{
    public NPC[] npcs;

    private NPC selectedNPC;
    private Dictionary<NPCAction, List<string>> actionKeywords;

    public enum NPCName
    {
        Carla,
        David,
        Rubén,
        Jesús
    }

    public enum NPCAction
    {
        Rest,
        Walk,
        CheckConsciousness,
        CheckAirWay,
        PutGuedel,
        CheckPulse,
        Compressions,
        Ventilations,
        CheckDefibrilator,
        PlacePatches,
        ChargeDefibrilator,
        AllOut,
        DischargeDefibrilator,
        PlaceVVP,
        Epinephrine,
        Epinephrine2,
        Lidocaine
    }

    protected override void OnInstance()
    {
        base.OnInstance();
        actionKeywords = new Dictionary<NPCAction, List<string>>();
        actionKeywords[NPCAction.Rest] = new List<string>();
        actionKeywords[NPCAction.Rest].Add("para");
        actionKeywords[NPCAction.Rest].Add("descansa");
        actionKeywords[NPCAction.CheckConsciousness] = new List<string>();
        actionKeywords[NPCAction.CheckConsciousness].Add("conciencia");
        actionKeywords[NPCAction.CheckConsciousness].Add("conciente");
        actionKeywords[NPCAction.CheckAirWay] = new List<string>();
        actionKeywords[NPCAction.CheckAirWay].Add("aire");
        actionKeywords[NPCAction.CheckAirWay].Add("respira");
        actionKeywords[NPCAction.Ventilations] = new List<string>();
        actionKeywords[NPCAction.Ventilations].Add("ventilacion");
        actionKeywords[NPCAction.CheckPulse] = new List<string>();
        actionKeywords[NPCAction.CheckPulse].Add("pulso");
        actionKeywords[NPCAction.Compressions] = new List<string>();
        actionKeywords[NPCAction.Compressions].Add("compresiones");
        actionKeywords[NPCAction.Compressions].Add("comprime");
        actionKeywords[NPCAction.CheckDefibrilator] = new List<string>();
        actionKeywords[NPCAction.CheckDefibrilator].Add("enciende");
        actionKeywords[NPCAction.ChargeDefibrilator] = new List<string>();
        actionKeywords[NPCAction.AllOut] = new List<string>();
        actionKeywords[NPCAction.ChargeDefibrilator].Add("carga");
        actionKeywords[NPCAction.DischargeDefibrilator] = new List<string>();
        actionKeywords[NPCAction.DischargeDefibrilator].Add("descarga");
        actionKeywords[NPCAction.DischargeDefibrilator].Add("shock");
        actionKeywords[NPCAction.Epinephrine] = new List<string>();
        actionKeywords[NPCAction.Epinephrine].Add("epinefrina");
        actionKeywords[NPCAction.Epinephrine].Add("adrenalina");
        actionKeywords[NPCAction.Lidocaine] = new List<string>();
        actionKeywords[NPCAction.Lidocaine].Add("lidocaina");
    }

    public List<NPCName> CheckNPCNames(string[] words)
    {
        List<NPCName> foundNames = new List<NPCName>();
        string[] npcNames = Enum.GetNames(typeof(NPCName));
        for (int i = 0; i < words.Length; i++)
        {
            if (npcNames.Contains(words[i]))
            {
                foundNames.Add((NPCName)Enum.Parse(typeof(NPCName), words[i]));
            }
        }
        return foundNames;
    }

    public List<string> ActionMatches(string[] words)
    {
        List<string> matches = new List<string>();
        for (int i = 0; i < words.Length; i++)
        {
            if (ActionMatch(words[i]))
            {
                matches.Add(words[i]);
            }
        }
        return matches;
    }

    public bool ActionMatch(string word)
    {
        return ((NPCAction[])Enum.GetValues(typeof(NPCAction))).Select(action => action.ToString().ToLower()).ToArray().Contains(word);
    }

    public void SelectNPC(NPCName name)
    {
        NPC found = FindNPC(name);
        if (found)
        {
            selectedNPC = found;
        }
    }

    public NPC GetSelectedNPC()
    {
        return selectedNPC;
    }

    public NPC FindNPC(NPCName name)
    {
        return npcs.FirstOrDefault(npc => npc.characterName == name);
    }

    public void GiveOrder(NPCAction action)
    {
        if (selectedNPC)
        {
            selectedNPC.GiveOrder(action);
        }
    }

    public NPCSpot.SpotType GetCorrectSpotType(NPCAction action)
    {
        switch (action)
        {
            case NPCAction.AllOut:
            case NPCAction.Ventilations:
            case NPCAction.PutGuedel:
            case NPCAction.PlacePatches:
            case NPCAction.PlaceVVP:
                return NPCSpot.SpotType.Ventilations;
            case NPCAction.CheckConsciousness:
            case NPCAction.CheckAirWay:
            case NPCAction.CheckPulse:
            case NPCAction.Compressions:
                return NPCSpot.SpotType.Compressions;
            case NPCAction.Epinephrine:
            case NPCAction.Lidocaine:
                return NPCSpot.SpotType.Medication;
            case NPCAction.CheckDefibrilator:
            case NPCAction.ChargeDefibrilator:
            case NPCAction.DischargeDefibrilator:
                return NPCSpot.SpotType.VitalSigns;
            case NPCAction.Epinephrine2:
                return NPCSpot.SpotType.Dropper;
        }
        return NPCSpot.SpotType.Generic;
    }

    public List<NPCAction> GetCurrentNPCActions()
    {
        List<NPCAction> currentActions = new List<NPCAction>();
        for (int i = 0; i < npcs.Length; i++)
        {
            if (npcs[i].GetCurrentAction() == NPCAction.Walk)
            {
                currentActions.Add(npcs[i].GetNextAction());
            }
            else {
                currentActions.Add(npcs[i].GetCurrentAction());
            }
        }
        return currentActions;
    }

/*
#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            FindNPC(NPCName.Carla).GiveOrder(NPCAction.PutGuedel);
        }
        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            FindNPC(NPCName.Carla).GiveOrder(NPCAction.CheckPulse);
        }
        if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            FindNPC(NPCName.Rubén).GiveOrder(NPCAction.Compressions);
        }
        if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            FindNPC(NPCName.Rubén).GiveOrder(NPCAction.Ventilations);
        }
        if (Input.GetKeyUp(KeyCode.Alpha5))
        {
            FindNPC(NPCName.David).GiveOrder(NPCAction.CheckDefibrilator);
        }
        if (Input.GetKeyUp(KeyCode.Alpha6))
        {
            FindNPC(NPCName.Jesús).GiveOrder(NPCAction.ChargeDefibrilator);
        }
        if (Input.GetKeyUp(KeyCode.P))
        {
            FindNPC(NPCName.Jesús).GiveOrder(NPCAction.PlacePatches);
        }
        if (Input.GetKeyUp(KeyCode.Alpha7))
        {
            FindNPC(NPCName.David).GiveOrder(NPCAction.DischargeDefibrilator);
        }
        if (Input.GetKeyUp(KeyCode.Alpha8))
        {
            FindNPC(NPCName.David).GiveOrder(NPCAction.PlaceVVP);
        }
        if (Input.GetKeyUp(KeyCode.Alpha9))
        {
            FindNPC(NPCName.Carla).GiveOrder(NPCAction.Epinephrine);
        }
        if (Input.GetKeyUp(KeyCode.Alpha0))
        {
            FindNPC(NPCName.Jesús).GiveOrder(NPCAction.Lidocaine);
        }
    }
#endif
*/

}