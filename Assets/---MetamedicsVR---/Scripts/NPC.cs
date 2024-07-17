using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public string characterName;
    public Animator animator;
    
    public NPCSpot startingSpot;

    private NPCSpot currentSpot;

    private void Awake()
    {
        currentSpot = startingSpot;
    }

    private NPCAction currentAction;
    private NPCAction nextAction;

    public enum NPCAction
    {
        Idle,
        WalkToSpot,
        CheckConsciousness,
        OpenAirWay,
        Guedel,
        CheckPulse,
        Compressions,
        CheckDefibrilator,
        ChargeDefibrilator,
        DischargeDefibrilator,
        OpenLine,
        Ventilations,
        Shock,
        Epinephrine,
        Lidocaine,
        Defibrillator
    }

    public void GiveOrder(NPCAction action)
    {
        switch (action)
        {
            case NPCAction.Compressions:
                break;
            case NPCAction.OpenLine:
                break;
            case NPCAction.Ventilations:
                break;
            case NPCAction.Shock:
                break;
            case NPCAction.Epinephrine:
                break;
            case NPCAction.Lidocaine:
                break;
            case NPCAction.Defibrillator:
                break;
        }
    }

    public NPCAction GetCurrentAction()
    {
        return currentAction;
    }

    public bool CanPerformAction(NPCAction action)
    {
        return true;
    }
}
