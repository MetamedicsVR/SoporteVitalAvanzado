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
    public Animator patientAnimator;
    public GameObject[] patchesInPatient;
    public GameObject vitalSignsMonitor;
    public GameObject cableSuero;

    private NPC selectedNPC;
    private Dictionary<NPC.NPCAction, List<string>> actionKeywords;

    public enum NPCName
    {
        Chica,
        Rubio,
        Negro,
        Calvo
    }

    protected override void OnInstance()
    {
        base.OnInstance();
        actionKeywords = new Dictionary<NPC.NPCAction, List<string>>();
        actionKeywords[NPC.NPCAction.Rest] = new List<string>();
        actionKeywords[NPC.NPCAction.Rest].Add("para");
        actionKeywords[NPC.NPCAction.CheckConsciousness] = new List<string>();
        actionKeywords[NPC.NPCAction.CheckConsciousness].Add("conciencia");
        actionKeywords[NPC.NPCAction.CheckAirWay] = new List<string>();
        actionKeywords[NPC.NPCAction.CheckAirWay].Add("aire");
        actionKeywords[NPC.NPCAction.Ventilations] = new List<string>();
        actionKeywords[NPC.NPCAction.Ventilations].Add("ventilacion");
        actionKeywords[NPC.NPCAction.CheckPulse] = new List<string>();
        actionKeywords[NPC.NPCAction.CheckPulse].Add("pulso");
        actionKeywords[NPC.NPCAction.Compressions] = new List<string>();
        actionKeywords[NPC.NPCAction.Compressions].Add("compresiones");
        actionKeywords[NPC.NPCAction.CheckDefibrilator] = new List<string>();
        actionKeywords[NPC.NPCAction.CheckDefibrilator].Add("comprueba");
        actionKeywords[NPC.NPCAction.ChargeDefibrilator] = new List<string>();
        actionKeywords[NPC.NPCAction.ChargeDefibrilator].Add("carga");
        actionKeywords[NPC.NPCAction.DischargeDefibrilator] = new List<string>();
        actionKeywords[NPC.NPCAction.DischargeDefibrilator].Add("descarga");
        actionKeywords[NPC.NPCAction.Epinephrine] = new List<string>();
        actionKeywords[NPC.NPCAction.Epinephrine].Add("epinefrina");
        actionKeywords[NPC.NPCAction.Epinephrine].Add("adrenalina");
        actionKeywords[NPC.NPCAction.Lidocaine] = new List<string>();
        actionKeywords[NPC.NPCAction.Lidocaine].Add("lidocaina");
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
        return ((NPC.NPCAction[])Enum.GetValues(typeof(NPC.NPCAction))).Select(action => action.ToString().ToLower()).ToArray().Contains(word);
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

    public void GiveOrder(NPC.NPCAction action)
    {
        if (selectedNPC)
        {
            selectedNPC.GiveOrder(action);
        }
    }

    public void GotTranscription(string s)
    {
        print("El jugador ha dicho: " + s);
    }
}