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
    public GameObject patches;
    public GameObject vvp;
    public GameObject syringe;
    public GameObject medication;

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
            if (characterName.Equals(NPCManager.NPCName.Calvo))
            {
                StartCoroutine(FollowOrder(NPCAction.CheckPulse));
            }
        }
        if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            if (characterName.Equals(NPCManager.NPCName.Calvo))
            {
                StartCoroutine(FollowOrder(NPCAction.Compressions));
            }
        }
        if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            if (characterName.Equals(NPCManager.NPCName.Chica))
            {
                StartCoroutine(FollowOrder(NPCAction.Ventilations));
            }
        }
        if (Input.GetKeyUp(KeyCode.Alpha5))
        {
            if (characterName.Equals(NPCManager.NPCName.Negro))
            {
                StartCoroutine(FollowOrder(NPCAction.CheckDefibrilator));
            }
        }
        if (Input.GetKeyUp(KeyCode.Alpha6))
        {
            if (characterName.Equals(NPCManager.NPCName.Negro))
            {
                StartCoroutine(FollowOrder(NPCAction.ChargeDefibrilator));
            }
        }
        if (Input.GetKeyUp(KeyCode.Alpha7))
        {
            if (characterName.Equals(NPCManager.NPCName.Negro))
            {
                StartCoroutine(FollowOrder(NPCAction.PlacePatches));
            }
        }
        if (Input.GetKeyUp(KeyCode.Alpha7))
        {
            if (characterName.Equals(NPCManager.NPCName.Rubio))
            {
                StartCoroutine(FollowOrder(NPCAction.DischargeDefibrilator));
            }
        }
        if (Input.GetKeyUp(KeyCode.Alpha8))
        {
            if (characterName.Equals(NPCManager.NPCName.Rubio))
            {
                StartCoroutine(FollowOrder(NPCAction.PlaceVVP));
            }
        }
        if (Input.GetKeyUp(KeyCode.Alpha9))
        {
            if (characterName.Equals(NPCManager.NPCName.Calvo))
            {
                StartCoroutine(FollowOrder(NPCAction.Epinephrine));
            }
        }
        if (Input.GetKeyUp(KeyCode.Alpha0))
        {
            if (characterName.Equals(NPCManager.NPCName.Calvo))
            {
                StartCoroutine(FollowOrder(NPCAction.Lidocaine));
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
        PlacePatches,
        ChargeDefibrilator,
        DischargeDefibrilator,
        PlaceVVP,
        Epinephrine,
        Epinephrine2,
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
            NPCSpot targetSpot = NPCSpotManager.GetInstance().GetNearestSpot(correctSpotType, transform.position);
            if (targetSpot.npcInSpot)
            {
                targetSpot.npcInSpot.GiveOrder(NPCAction.Rest);
            }
            SetCurrentSpot(targetSpot);
            animator.Play("Anim_Andar");

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
                yield return new WaitForSeconds(15);
                break;
            case NPCAction.PutGuedel:
                guedel.SetActive(true);
                animator.Play("Anim_IntroducirGuedel");
                NPCManager.GetInstance().patientAnimator.Play("Anim_IntroducirGuedelPaciente");
                yield return new WaitForSeconds(3f);
                guedel.SetActive(false);
                yield return new WaitForSeconds(7);
                break;
            case NPCAction.CheckPulse:
                animator.Play("Anim_ComprobarPulso");
                yield return new WaitForSeconds(11.2f);
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
            
                for (int i = 0; i < NPCSpotManager.GetInstance().spots.Length; i++)
                {
                    if (NPCSpotManager.GetInstance().spots[i].type == NPCSpot.SpotType.Compressions)
                    {
                        if (NPCSpotManager.GetInstance().spots[i].npcInSpot != null)
                        {
                            NPCManager.GetInstance().patientAnimator.CrossFade("Anim_IdlePaciente", 0.7f);
                            NPCSpotManager.GetInstance().spots[i].npcInSpot.GetComponent<Animator>().CrossFade("Anim_idle", 0.7f);
                        }
                    }
                }
                yield return new WaitForSeconds(7.5f);
        
                for (int i = 0; i < NPCSpotManager.GetInstance().spots.Length; i++)
                {
                    if (NPCSpotManager.GetInstance().spots[i].type == NPCSpot.SpotType.Compressions)
                    {
                        if (NPCSpotManager.GetInstance().spots[i].npcInSpot != null)
                        {
                            NPCManager.GetInstance().patientAnimator.Play("Anim_Comprimido");
                            NPCSpotManager.GetInstance().spots[i].npcInSpot.GetComponent<Animator>().Play("Anim_Comprimir");
                        }
                    }
                }
                yield return new WaitForSeconds(2f);
                animator.CrossFade("Anim_idle", 1);
                yield return new WaitForSeconds(3f);
                break;
            case NPCAction.CheckDefibrilator:
                animator.Play("Anim_TocarBotonesDea");
                yield return new WaitForSeconds(2);
                NPCManager.GetInstance().vitalSignsMonitor.GetComponent<VitalLine>().enabled = true;
                yield return new WaitForSeconds(6);
                break;
            case NPCAction.PlacePatches:
                animator.Play("Anim_ColocarParchesDea");
                yield return new WaitForSeconds(1.5f);
                NPCManager.GetInstance().patchesInPatient[0].SetActive(true);
                yield return new WaitForSeconds(0.5f);
                NPCManager.GetInstance().patchesInPatient[1].SetActive(true);
                yield return new WaitForSeconds(5);
                break;
            case NPCAction.ChargeDefibrilator:
                animator.Play("Anim_TocarBotonesDea");
                yield return new WaitForSeconds(2);
                NPCManager.GetInstance().vitalSignsMonitor.GetComponent<VitalLine>().enabled = true;
                yield return new WaitForSeconds(6);
                break;
            case NPCAction.DischargeDefibrilator:
                animator.Play("Anim_TocarBotonesDea");
                yield return new WaitForSeconds(2);
                NPCManager.GetInstance().vitalSignsMonitor.GetComponent<VitalLine>().enabled = true;
                yield return new WaitForSeconds(3.5f);
                NPCManager.GetInstance().patientAnimator.Play("Anim_RecibeShock");
                break;
            case NPCAction.PlaceVVP:
                animator.Play("Anim_Colocaraguja");
                //yield return new WaitForSeconds(0.1f);
                NPCManager.GetInstance().patientAnimator.Play("Anim_PacienteColocarAguja");
                yield return new WaitForSeconds(7.5f);
                NPCManager.GetInstance().cableSuero.gameObject.SetActive(true);
                yield return new WaitForSeconds(9999);
                break;
            case NPCAction.Epinephrine:
                animator.Play("Anim_ExtraerMedicamento");
                yield return new WaitForSeconds(1);
                syringe.SetActive(true);
                medication.SetActive(true);
                yield return new WaitForSeconds(10);
                syringe.SetActive(false);
                medication.SetActive(false);
                NPCSpot targetSpot = NPCSpotManager.GetInstance().GetNearestSpot(correctSpotType, transform.position);
                targetSpot.npcInSpot.GiveOrder(NPCAction.Epinephrine2);
                break;
            case NPCAction.Epinephrine2:
                animator.Play("Anim_ExtraerMedicamentoParte2");
                yield return new WaitForSeconds(6);
                animator.Play("Anim_PincharSuero");
                yield return new WaitForSeconds(5);
                syringe.SetActive(false);
                medication.SetActive(false);
                break;
            case NPCAction.Lidocaine:
                syringe.SetActive(true);
                medication.SetActive(true);
                animator.Play("");
                yield return new WaitForSeconds(9999);
                syringe.SetActive(false);
                medication.SetActive(false);
                break;
            
        }
        animator.Play("Anim_idle");
        actionCoroutine = null;

    }

    private void SetCurrentSpot(NPCSpot npcSpot)
    {
        if (currentSpot)
        {
            currentSpot.npcInSpot = null;
        }
        currentSpot = npcSpot;
        if (currentSpot)
        {
          currentSpot.npcInSpot = this;
        }
    }


    public NPCAction GetCurrentAction()
    {
        return currentAction;
    }

    public bool CanPerformAction(NPCAction action)
    {
        return actionCoroutine != null;
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
                return NPCSpot.SpotType.Dropper;
        }
        return NPCSpot.SpotType.Generic;
    }
}
