using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Analytics : MonoBehaviourInstance<Analytics>
{
    protected List<PlayerOrder> data = new List<PlayerOrder>();

    public struct PlayerOrder
    {
        public string characterName;
        public NPC.NPCAction order;
        public bool isCorrect;
        public string explanation;
    }

    public void ResetData()
    {
        data = new List<PlayerOrder>();
    }

    public void InsertData(string characterName, NPC.NPCAction order, bool isCorrect, string explanation)
    {
        PlayerOrder playerOrder = new PlayerOrder();
        playerOrder.characterName = characterName;
        playerOrder.order = order;
        playerOrder.isCorrect = isCorrect;
        playerOrder.explanation = explanation;
        data.Add(playerOrder);
    }

    public string EmailString()
    {
        string analyticsString = "Results:\r\n";
        for (int i = 0; i < data.Count; i++)
        {
            analyticsString += data[i].characterName + ", " + ActionDescription(data[i].order) + " -> " + (data[i].isCorrect ? "Correct" : "Wrong") + ".\r\n";
        }
        return analyticsString;
    }

    public string ActionDescription(NPC.NPCAction action)
    {
        switch (action)
        {
        
        }
        return "";
    }
}
