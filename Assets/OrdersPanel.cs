using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OrdersPanel : MonoBehaviour
{
    public TextMeshProUGUI[] buttonTexts;

    private string selectedNPC;
    private List<string> possibleOrders = new List<string> { "Order 1", "Order 2", "Order 3", "Order 4", "Order 5", "Order 6", "Order 7", "Order 8" };

    public void OpenPanel()
    {
        for (int i = 0; i < buttonTexts.Length; i++)
        {
            if (i < NPCManager.GetInstance().npcs.Length)
            {
                buttonTexts[i].text = NPCManager.GetInstance().npcs[i].name;
            }
            else
            {
                buttonTexts[i].text = "";
            }
        }
    }

    public void SelectNPC(string npcName)
    {
        selectedNPC = npcName;
        List<string> shuffledOrders = new List<string>(possibleOrders);
        Shuffle(shuffledOrders);

        for (int i = 0; i < buttonTexts.Length; i++)
        {
            if (i < shuffledOrders.Count)
            {
                buttonTexts[i].text = shuffledOrders[i];
            }
            else
            {
                buttonTexts[i].text = "";
            }
        }
    }

    public void GiveOrder(int index)
    {

    }

    private void Shuffle(List<string> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            string temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public string GetOrderText(NPC.NPCAction action)
    {
        switch (action)
        {
            case NPC.NPCAction.Rest:
                return "";
            case NPC.NPCAction.CheckConsciousness:
                return "";
            case NPC.NPCAction.CheckAirWay:
                return "";
            case NPC.NPCAction.Ventilations:
                return "";
            case NPC.NPCAction.CheckPulse:
                return "";
            case NPC.NPCAction.Compressions:
                return "";
            case NPC.NPCAction.CheckDefibrilator:
                return "";
            case NPC.NPCAction.ChargeDefibrilator:
                return "";
            case NPC.NPCAction.DischargeDefibrilator:
                return "";
            case NPC.NPCAction.Epinephrine:
                return "";
            case NPC.NPCAction.Lidocaine:
                return "";
        }
        return "";
    }
}
