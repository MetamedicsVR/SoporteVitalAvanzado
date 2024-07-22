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
    public VoiceService[] voiceServices;
    public NPC[] npcs;

    private NPC selectedNPC;
    private Dictionary<NPC.NPCAction, List<string>> actionKeywords;

    protected override void OnInstance()
    {
        base.OnInstance();
        actionKeywords = new Dictionary<NPC.NPCAction, List<string>>();
        actionKeywords[NPC.NPCAction.Rest] = new List<string>();
        actionKeywords[NPC.NPCAction.Rest].Add("para");
        actionKeywords[NPC.NPCAction.CheckConsciousness] = new List<string>();
        actionKeywords[NPC.NPCAction.CheckConsciousness].Add("");
        actionKeywords[NPC.NPCAction.CheckAirWay] = new List<string>();
        actionKeywords[NPC.NPCAction.CheckAirWay].Add("");
        actionKeywords[NPC.NPCAction.Ventilations] = new List<string>();
        actionKeywords[NPC.NPCAction.Ventilations].Add("ventilacion");
        actionKeywords[NPC.NPCAction.CheckPulse] = new List<string>();
        actionKeywords[NPC.NPCAction.CheckPulse].Add("");
        actionKeywords[NPC.NPCAction.Compressions] = new List<string>();
        actionKeywords[NPC.NPCAction.Compressions].Add("");
        actionKeywords[NPC.NPCAction.CheckDefibrilator] = new List<string>();
        actionKeywords[NPC.NPCAction.CheckDefibrilator].Add("");
        actionKeywords[NPC.NPCAction.ChargeDefibrilator] = new List<string>();
        actionKeywords[NPC.NPCAction.ChargeDefibrilator].Add("");
        actionKeywords[NPC.NPCAction.DischargeDefibrilator] = new List<string>();
        actionKeywords[NPC.NPCAction.DischargeDefibrilator].Add("");
        actionKeywords[NPC.NPCAction.Epinephrine] = new List<string>();
        actionKeywords[NPC.NPCAction.Epinephrine].Add("epinefrina");
        actionKeywords[NPC.NPCAction.Epinephrine].Add("adrenalina");
        actionKeywords[NPC.NPCAction.Lidocaine] = new List<string>();
        actionKeywords[NPC.NPCAction.Lidocaine].Add("");
    }

    private void Start()
    {
        foreach (VoiceService service in voiceServices)
        {
            if (service)
            {
                service.VoiceEvents.OnFullTranscription.AddListener(OnTranscriptionReceived);
            }
        }
    }

    private void OnTranscriptionReceived(string transcription)
    {
        string cleanText = Regex.Replace(transcription.Normalize(NormalizationForm.FormD), @"[^a-zA-Z\s]", "").ToLower();
        string[] words = cleanText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        List<string> npcNames = CheckNPCNames(words);
        if (npcNames.Count > 0)
        {
            if (npcNames.Count > 1)
            {

            }
            SelectNPC(npcNames[0]);
        }
        else if (selectedNPC)
        {

        }
        else
        {
            //Instrucción sin NPC seleccionado
        }
        foreach (string word in words)
        {
            //if (NameMatch(word))
            {
                SelectNPC(word);
            }
            if (ActionMatch(word))
            {
                GiveOrder(word);
            }
        }
    }

    private List<string> CheckNPCNames(string[] words)
    {
        List<string> foundNames = new List<string>();
        string[] npcNames = NPCNames();
        for (int i = 0; i < words.Length; i++)
        {
            if (npcNames.Contains(words[i]))
            {
                foundNames.Add(words[i]);
            }
        }
        return foundNames;
    }

    public bool ActionMatch(string word)
    {
        return ((NPC.NPCAction[])Enum.GetValues(typeof(NPC.NPCAction))).Select(action => action.ToString().ToLower()).ToArray().Contains(word);
    }

    public void SelectNPC(string characterName)
    {
        NPC found = FindNPC(characterName);
        if (found)
        {
            selectedNPC = found;
        }
    }

    public NPC GetSelectedNPC()
    {
        return selectedNPC;
    }

    public NPC FindNPC(string name)
    {
        return npcs.FirstOrDefault(npc => npc.characterName == name);
    }

    public string[] NPCNames()
    {
        return npcs.Select(npc => npc.characterName.ToLower()).ToArray();
    }

    public void GiveOrder(string order)
    {
        if (selectedNPC)
        {
            //selectedNPC.GiveOrder(order);
        }
    }
}
