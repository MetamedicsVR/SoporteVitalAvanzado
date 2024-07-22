using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{
    public NPCManager.NPCName characterName;
    public NPCSpot startingSpot;

    [Header("Tools")]
    public GameObject ambu;

    private Animator animator;
    private NavMeshAgent navMeshAgent;
    private NPCSpot currentSpot;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        SetCurrentSpot(startingSpot);
    }

    private NPCAction currentAction;
    private Coroutine actionCoroutine;

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
        ChargeDefibrilator,
        DischargeDefibrilator,
        Epinephrine,
        Lidocaine
    }

    public void GiveOrder(NPCAction action)
    {
        if (actionCoroutine != null)
        {
            StopCoroutine(actionCoroutine);
        }
        actionCoroutine = StartCoroutine(FollowOrder(action));
    }

    private IEnumerator FollowOrder(NPCAction action)
    {
        NPCSpot.SpotType correctSpotType = GetCorrectSpotType(action);
        if (!currentSpot || currentSpot.type != correctSpotType)
        {
            animator.Play("Anim_Andar");
            SetCurrentSpot(NPCSpotManager.GetInstance().GetAvailableSpot(correctSpotType));
            navMeshAgent.SetDestination(currentSpot.transform.position);
            yield return new WaitUntil(() => Vector3.Distance(currentSpot.transform.position, transform.position) < 0.5f);
            float lerpDuration = 0.5f;
            Vector3 startPosition = transform.position;
            Quaternion startingRotation = transform.rotation;
            float elapsedTime = 0f;
            animator.CrossFade("Anim_idle", lerpDuration);
            while (elapsedTime < lerpDuration)
            {
                elapsedTime += Time.deltaTime;
                transform.position = Vector3.Lerp(startPosition, currentSpot.transform.position, Mathf.Clamp01(elapsedTime / lerpDuration));
                transform.rotation = Quaternion.Lerp(startingRotation, currentSpot.transform.rotation, Mathf.Clamp01(elapsedTime / lerpDuration));
                yield return null;
            }
            transform.position = currentSpot.transform.position;
        }
        switch (action)
        {
            case NPCAction.Rest:
                animator.Play("Anim_idle");
                break;
            case NPCAction.CheckConsciousness:
                animator.Play("Anim_ComprobarConsciencia");
                yield return new WaitForSeconds(1);
                NPCManager.GetInstance().patientAnimator.Play("Anim_PacienteComprobarConsciencia");
                break;
            case NPCAction.CheckAirWay:
                animator.Play("Anim_GoToComprobarRespiracion");
                yield return new WaitForSeconds(2);
                NPCManager.GetInstance().patientAnimator.Play("Anim_AbrirBocaPaciente");
                yield return new WaitForSeconds(15);
                NPCManager.GetInstance().patientAnimator.Play("Anim_CerrarBocaPaciente");
                yield return new WaitForSeconds(2);
                animator.CrossFade("Anim_idle", 1);
                break;
            case NPCAction.PutGuedel:
                //animator.Play(""); ???
                break;
            case NPCAction.CheckPulse:
                animator.Play("Anim_GoToComprobarPulso");
                break;
            case NPCAction.Compressions:
                animator.Play("Anim_GoToComprobarRespiracion");
                NPCManager.GetInstance().patientAnimator.Play("Anim_Comprimido");
                break;
            case NPCAction.Ventilations:
                ambu.SetActive(true);
                animator.Play("Anim_GoToVentilar");
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
        //TODO Añadir Crossfade a Idle después de cada animación
    }

    private void SetCurrentSpot(NPCSpot npcSpot)
    {
        if (currentSpot)
        {
            currentSpot.available = true;
        }
        currentSpot = npcSpot;
        if (currentSpot)
        {
            currentSpot.available = false;
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

    public NPCSpot.SpotType GetCorrectSpotType(NPCAction action)
    {
        switch (action)
        {
            case NPCAction.CheckConsciousness:
            case NPCAction.Ventilations:
            case NPCAction.PutGuedel:
                return NPCSpot.SpotType.Ventilations;
            case NPCAction.CheckAirWay:
            case NPCAction.CheckPulse:
            case NPCAction.Compressions:
            case NPCAction.CheckDefibrilator:
            case NPCAction.ChargeDefibrilator:
            case NPCAction.DischargeDefibrilator:
                return NPCSpot.SpotType.Compressions;
            case NPCAction.Epinephrine:
            case NPCAction.Lidocaine:
                return NPCSpot.SpotType.Medication;
        }
        return NPCSpot.SpotType.Generic;
    }
}
