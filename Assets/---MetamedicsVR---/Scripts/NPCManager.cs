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
    private Dictionary<string, NPCAction> actionKeywords = new Dictionary<string, NPCAction>();

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
        actionKeywords["conciencia"] = NPCAction.CheckConsciousness;
        actionKeywords["conciente"] = NPCAction.CheckConsciousness;
        actionKeywords["aire"] = NPCAction.CheckAirWay;
        actionKeywords["aerea"] = NPCAction.CheckAirWay;
        actionKeywords["respira"] = NPCAction.CheckAirWay;
        actionKeywords["canula"] = NPCAction.PutGuedel;
        actionKeywords["ventilacion"] = NPCAction.Ventilations;
        actionKeywords["ventilaciones"] = NPCAction.Ventilations;
        actionKeywords["pulso"] = NPCAction.CheckPulse;
        actionKeywords["compresion"] = NPCAction.Compressions;
        actionKeywords["compresiones"] = NPCAction.Compressions;
        actionKeywords["comprime"] = NPCAction.Compressions;
        actionKeywords["enciende"] = NPCAction.CheckDefibrilator;
        actionKeywords["carga"] = NPCAction.ChargeDefibrilator;
        actionKeywords["descarga"] = NPCAction.DischargeDefibrilator;
        actionKeywords["shock"] = NPCAction.DischargeDefibrilator;
        actionKeywords["parches"] = NPCAction.PlacePatches;
        actionKeywords["fuera"] = NPCAction.OutNow;
        actionKeywords["alejaos"] = NPCAction.OutNow;
        actionKeywords["apartaos"] = NPCAction.OutNow;
        actionKeywords["intravenosa"] = NPCAction.PlaceVVP;
        actionKeywords["epinefrina"] = NPCAction.Epinephrine;
        actionKeywords["adrenalina"] = NPCAction.Epinephrine;
        actionKeywords["lidocaina"] = NPCAction.Lidocaine;
        actionKeywords["amiodarona"] = NPCAction.Lidocaine;
    }

    public List<NPCName> CheckNames(List<string> words)
    {
        NPCName[] nameValues = (NPCName[])Enum.GetValues(typeof(NPCName));
        string[] nameStrings = nameValues.Select(name => Regex.Replace(name.ToString().Normalize(NormalizationForm.FormD), @"[^a-zA-Z\s]", "").ToLower()).ToArray();
        List<NPCName> nameFounds = new List<NPCName>();
        for (int i = 0; i < words.Count; i++)
        {
            if (nameStrings.Contains(words[i]))
            {
                nameFounds.Add(nameValues[Array.IndexOf(nameStrings, words[i])]);
            }
        }
        return nameFounds;
    }

    public List<NPCAction> CheckActions(List<string> words)
    {
        List<NPCAction> actionFounds = new List<NPCAction>();
        for (int i = 0; i < words.Count; i++)
        {
            if (actionKeywords.ContainsKey(words[i]))
            {
                actionFounds.Add(actionKeywords[words[i]]);
            }
        }
        return actionFounds;
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