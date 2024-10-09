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
        CheckConsciousness,
        CheckAirWay,
        PutGuedel,
        CheckPulse,
        Compressions,
        Ventilations,
        CheckDefibrilator,
        PlacePatches,
        ChargeDefibrilator,
        OutNow,
        DischargeDefibrilator,
        PlaceVVP,
        Epinephrine,
        Epinephrine2,
        Lidocaine,
        Lidocaine2,
        OutAfterEnd
    }

    protected override void OnInstance()
    {
        base.OnInstance();
        actionKeywords = new Dictionary<NPCAction, List<string>>();
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
        actionKeywords[NPCAction.ChargeDefibrilator].Add("carga");
        actionKeywords[NPCAction.DischargeDefibrilator] = new List<string>();
        actionKeywords[NPCAction.DischargeDefibrilator].Add("descarga");
        actionKeywords[NPCAction.DischargeDefibrilator].Add("shock");
        actionKeywords[NPCAction.OutNow] = new List<string>();
        actionKeywords[NPCAction.OutNow].Add("fuera");
        actionKeywords[NPCAction.OutNow].Add("alejaos");
        actionKeywords[NPCAction.Epinephrine] = new List<string>();
        actionKeywords[NPCAction.Epinephrine].Add("epinefrina");
        actionKeywords[NPCAction.Epinephrine].Add("adrenalina");
        actionKeywords[NPCAction.Lidocaine] = new List<string>();
        actionKeywords[NPCAction.Lidocaine].Add("lidocaina");
    }

    public List<NPCName> CheckNPCNames(List<string> words)
    {
        List<NPCName> foundNames = new List<NPCName>();
        string[] npcNames = Enum.GetNames(typeof(NPCName));
        for (int i = 0; i < words.Count; i++)
        {
            if (npcNames.Contains(words[i]))
            {
                foundNames.Add((NPCName)Enum.Parse(typeof(NPCName), words[i]));
            }
        }
        return foundNames;
    }

    public List<string> ActionMatches(List<string> words)
    {
        List<string> matches = new List<string>();
        for (int i = 0; i < words.Count; i++)
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
            case NPCAction.Lidocaine2:
                return NPCSpot.SpotType.Dropper;
        }
        return NPCSpot.SpotType.Generic;
    }

    public List<NPCAction> GetCurrentNPCActions()
    {
        List<NPCAction> currentActions = new List<NPCAction>();
        for (int i = 0; i < npcs.Length; i++)
        {
            currentActions.Add(npcs[i].GetCurrentAction());
        }
        return currentActions;
    }
}