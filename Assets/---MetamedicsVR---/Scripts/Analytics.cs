using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Analytics : MonoBehaviourInstance<Analytics>
{
    protected List<PlayerOrder> data = new List<PlayerOrder>();

    public struct PlayerOrder
    {
        public string timeStamp;
        public string characterName;
        public string order;
        public bool isCorrect;
        public string explanation;
    }

    public void ResetData()
    {
        data = new List<PlayerOrder>();
    }

    public void InsertData(string characterName, NPCManager.NPCAction order, bool isCorrect, string explanation)
    {
        float timeElapsed = Time.time;
        int minutes = (int)(timeElapsed / 60);
        int seconds = (int)(timeElapsed % 60);
        PlayerOrder playerOrder = new PlayerOrder();
        playerOrder.timeStamp = string.Format("{0:00}:{1:00}", minutes, seconds);
        playerOrder.characterName = characterName;
        playerOrder.order = ActionDescription(order);
        playerOrder.isCorrect = isCorrect;
        playerOrder.explanation = explanation;
        data.Add(playerOrder);
    }

    public string EmailString()
    {
        string analyticsString = "Results:\r\n";
        for (int i = 0; i < data.Count; i++)
        {
            analyticsString += data[i].characterName + ", " + data[i].order + " -> " + (data[i].isCorrect ? "Correct" : "Wrong") + ".\r\n";
        }
        return analyticsString;
    }

    public List<PlayerOrder> GetData()
    {
        return data;
    }

    public string ActionDescription(NPCManager.NPCAction action)
    {
        switch (action) 
        {
            case NPCManager.NPCAction.Rest:
                return "Descansar";
            case NPCManager.NPCAction.Walk:
                return "Caminar al punto";
            case NPCManager.NPCAction.CheckConsciousness:
                return "Comprobar consciencia";
            case NPCManager.NPCAction.CheckAirWay:
                return "Comprobar vía aerea";
            case NPCManager.NPCAction.PutGuedel:
                return "Colocar Güedel";
            case NPCManager.NPCAction.CheckPulse:
                return "Comprobar pulso";
            case NPCManager.NPCAction.Compressions:
                return "Ciclo de compresiones";
            case NPCManager.NPCAction.Ventilations:
                return "Ciclo de ventilacioenes";
            case NPCManager.NPCAction.CheckDefibrilator:
                return "Comprobar desfibrilador";
            case NPCManager.NPCAction.PlacePatches:
                return "Coloca parches";
            case NPCManager.NPCAction.ChargeDefibrilator:
                return "Carga desfibrilador";
            case NPCManager.NPCAction.AllOut:
                return "Todos fuera";
            case NPCManager.NPCAction.DischargeDefibrilator:
                return "Dar descarga a 150 J";
            case NPCManager.NPCAction.PlaceVVP:
                return "Colocar vía";
            case NPCManager.NPCAction.Epinephrine:
                return "Epirefrina 1 mg";
            case NPCManager.NPCAction.Epinephrine2:
                return "Epirefrina 1 mg";
            case NPCManager.NPCAction.Lidocaine:
                return "Lidocaina 300 miligramos en 20 mililitros";
            default:
                return "NONE";
        }

    }
}
