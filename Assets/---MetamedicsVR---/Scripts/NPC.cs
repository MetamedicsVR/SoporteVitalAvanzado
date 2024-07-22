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
        Rest,
        CheckConsciousness,
        CheckAirWay,
        Ventilations,
        CheckPulse,
        Compressions,
        CheckDefibrilator,
        ChargeDefibrilator,
        DischargeDefibrilator,
        Epinephrine,
        Lidocaine
    }

    public void GiveOrder(NPCAction action)
    {
        //Anim_Andar
        //Anim_GoToComprimir
        //Anim_Comprimir
        //Anim_CogerAlgo
        //Anim_GoToVentilar
        //Anim_Ventilar
        switch (action)
        {
            case NPCAction.Rest:
                animator.Play("Anim_idle");
                break;
            case NPCAction.CheckConsciousness:
                break;
            case NPCAction.CheckAirWay:
                break;
            case NPCAction.Ventilations:
                break;
            case NPCAction.CheckPulse:
                break;
            case NPCAction.Compressions:
                break;
            case NPCAction.CheckDefibrilator:
                break;
            case NPCAction.ChargeDefibrilator:
                break;
            case NPCAction.DischargeDefibrilator:
                break;
            case NPCAction.Epinephrine:
                break;
            case NPCAction.Lidocaine:
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

    public void GoToSpot(NPCSpot npcSpot)
    {
    
    }
}
