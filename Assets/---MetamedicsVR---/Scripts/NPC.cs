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
    public GameObject guedel;

    private Animator animator;
    private NavMeshAgent navMeshAgent;
    private NPCSpot currentSpot;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        SetCurrentSpot(startingSpot);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            if (characterName.Equals(NPCManager.NPCName.Calvo))
            {
                StartCoroutine(FollowOrder(NPCAction.PutGuedel));
            }         
        }
        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            if (characterName.Equals(NPCManager.NPCName.Chica))
            {
                StartCoroutine(FollowOrder(NPCAction.PutGuedel));
            }
        }
        if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            if (characterName.Equals(NPCManager.NPCName.Negro))
            {
                StartCoroutine(FollowOrder(NPCAction.PutGuedel));
            }
        }
        if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            if (characterName.Equals(NPCManager.NPCName.Rubio))
            {
                StartCoroutine(FollowOrder(NPCAction.PutGuedel));
            }
        }
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
            transform.rotation = currentSpot.transform.rotation;
        }
        switch (action)
        {
            case NPCAction.Rest:
                print("Rest");

                break;
            case NPCAction.CheckConsciousness:

                animator.Play("Anim_ComprobarConciencia");
                NPCManager.GetInstance().patientAnimator.Play("Anim_ComprobarConcienciaPaciente");
                yield return new WaitForSeconds(7.5f);
                break;
            case NPCAction.CheckAirWay:

                animator.Play("Anim_ComprobarRespiracion");
                NPCManager.GetInstance().patientAnimator.Play("Anim_ComprobarRespiracionPaciente");
                yield return new WaitForSeconds(12);
                break;
            case NPCAction.PutGuedel:
                guedel.SetActive(true);
                animator.Play("Anim_IntroducirGuedel");
                NPCManager.GetInstance().patientAnimator.Play("Anim_IntroducirGuedelPaciente");
                yield return new WaitForSeconds(2f);
                guedel.SetActive(false);
                yield return new WaitForSeconds(9999);
                break;
            case NPCAction.CheckPulse:
                animator.Play("Anim_GoToComprobarPulso");
                yield return new WaitForSeconds(9999);
                break;
            case NPCAction.Compressions:
                animator.Play("Anim_GoToComprimir");
                yield return new WaitForSeconds(0.75f);
                NPCManager.GetInstance().patientAnimator.Play("Anim_Comprimido");
                yield return new WaitForSeconds(9999);
                break;
            case NPCAction.Ventilations:
                ambu.SetActive(true);
                animator.Play("Anim_GoToVentilar");
                yield return new WaitForSeconds(9999);
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
        animator.Play("Anim_idle");
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
      
            case NPCAction.Ventilations:
            case NPCAction.PutGuedel:
                return NPCSpot.SpotType.Ventilations;
            case NPCAction.CheckConsciousness:
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
